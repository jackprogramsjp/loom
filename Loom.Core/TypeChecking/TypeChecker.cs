using System.Diagnostics.CodeAnalysis;
using Loom.Core.Diagnostics;
using Loom.Core.FlowAnalysis;
using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Core.Text;
using Loom.Core.TypeChecking.Types;
using ArrayType = Loom.Core.Parsing.AST.ArrayType;
using FunctionType = Loom.Core.Parsing.AST.FunctionType;
using IntersectionType = Loom.Core.Parsing.AST.IntersectionType;
using LiteralType = Loom.Core.Parsing.AST.LiteralType;
using OptionalType = Loom.Core.Parsing.AST.OptionalType;
using PrimitiveType = Loom.Core.Parsing.AST.PrimitiveType;
using TypeName = Loom.Core.Parsing.AST.TypeName;
using TypeParameter = Loom.Core.Parsing.AST.TypeParameter;
using UnionType = Loom.Core.Parsing.AST.UnionType;

namespace Loom.Core.TypeChecking;

using Type = Types.Type;

public sealed partial class TypeChecker
    : Visitor<Type>
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<Node, FlowState> _exitStates = [];
    private readonly Stack<List<FlowState>> _loopExitScopes = [];
    private readonly SemanticModel _semanticModel;
    private readonly FlowAnalyzer _flowAnalyzer;
    private readonly TypeInferrer _inferrer;
    private readonly TypeNarrower _narrower;
    private FlowState _flowState;

    public TypeChecker(SemanticModel semanticModel, FlowAnalyzer flowAnalyzer)
        : base(_ => Types.PrimitiveType.Never)
    {
        _semanticModel = semanticModel;
        _flowAnalyzer = flowAnalyzer;
        _inferrer = new TypeInferrer(Visit);
        _narrower = new TypeNarrower(semanticModel);
        _flowState = null!;
    }

    public TypeCheckerResult Check()
    {
        var tree = _semanticModel.Tree;
        var type = BindType(tree, VisitTree(tree));
        _semanticModel.TypeSolver.SolveConstraints();

        var diagnostics = DiagnosticBag.Concat([_semanticModel.TypeSolver.Diagnostics, _diagnostics]);
        return new TypeCheckerResult(type, diagnostics);
    }

    protected override Type Visit(Node node) => Visit(node, _flowState);

    private Type Visit(Node node, FlowState? state)
    {
        var lastState = _flowState;
        if (state != null)
        {
            _flowState = state;
        }
        else
        {
            var baseState = _flowAnalyzer.GetState(node);
            _flowState = new FlowState(baseState.DefinitelyInitialized, baseState.MaybeInitialized, baseState.IsUnreachable, lastState.NarrowedTypes);
        }

        var result = node.Accept(this);
        _flowState = lastState;

        return result;
    }

    public override Type VisitTree(Tree tree)
    {
        _flowState = _flowAnalyzer.GetState(tree);
        var types = CheckStatements(tree, tree.Statements);
        return BindType(tree, types.LastOrDefault(Types.PrimitiveType.Void));
    }

    public override Type VisitExpressionStatement(ExpressionStatement expressionStatement) => BindType(expressionStatement, Visit(expressionStatement.Expression));
    public override Type VisitBlock(Block block) => BindType(block, CheckStatements(block, block.Statements).LastOrDefault(Types.PrimitiveType.Void));

    public override Type VisitBreak(Break @break)
    {
        _loopExitScopes.Peek().Add(_flowState);
        return BindType(@break, Types.PrimitiveType.Void);
    }

    public override Type VisitContinue(Continue @continue)
    {
        _loopExitScopes.Peek().Add(_flowState);
        return BindType(@continue, Types.PrimitiveType.Void);
    }

    public override Type VisitFor(For @for)
    {
        var collectionType = Visit(@for.CollectionExpression);
        if (collectionType is InstantiatedType i)
            collectionType = i.Expand();

        _semanticModel.TypeSolver.AddConstraint(collectionType, ObjectType.Empty, @for.CollectionExpression);
        var isRange = collectionType.Equals(Intrinsics.Range);
        var elementType = isRange ? Types.PrimitiveType.Number : GetObjectValueType(collectionType);
        var maxNames = isRange ? 1 : 2;
        if (@for.Names.Count > maxNames)
        {
            _diagnostics.NotImplemented(
                @for.Names[maxNames],
                isRange
                    ? "Iterating over a range only produces one value, so only one name is permitted."
                    : "Functional iterators are not supported yet, so more than two names is not permitted."
            );

            return BindType(@for, Types.PrimitiveType.Never);
        }

        switch (collectionType)
        {
            case var _ when collectionType.Equals(Intrinsics.Range):
                BindType(@for.Names[0], elementType);
                break;
            case Types.ArrayType:
                BindType(@for.Names[0], elementType);
                if (@for.Names.Count > 1)
                    BindType(@for.Names[1], Types.PrimitiveType.Number);

                break;
            case InterfaceType or ObjectType:
            {
                var objectType = collectionType is InterfaceType interfaceType ? interfaceType.ObjectType : (ObjectType)collectionType;
                BindType(@for.Names[0], objectType.KeyUnion());
                if (@for.Names.Count > 1)
                    BindType(@for.Names[1], elementType);

                break;
            }
        }

        _loopExitScopes.Push([]);
        var bodyType = CheckBody(@for.Body, _flowState);
        AssignLoopExitState(@for);

        return BindType(@for, bodyType);
    }

    public override Type VisitAfter(After after)
    {
        var durationType = Visit(after.Duration);
        _semanticModel.TypeSolver.AddConstraint(durationType, Types.PrimitiveType.Number, after.Duration);

        return BindType(after, Visit(after.Body));
    }

    public override Type VisitWhile(While @while)
    {
        var conditionType = Visit(@while.Condition);
        _semanticModel.TypeSolver.AddConstraint(conditionType, Types.PrimitiveType.Bool, @while.Condition);

        _loopExitScopes.Push([]);
        var (trueState, _) = _narrower.ComputeBranchStates(@while.Condition, _flowState);
        var bodyType = CheckBody(@while.Body, trueState);
        AssignLoopExitState(@while);

        return BindType(@while, bodyType);
    }

    public override Type VisitIf(If @if)
    {
        var conditionType = Visit(@if.Condition);
        _semanticModel.TypeSolver.AddConstraint(conditionType, Types.PrimitiveType.Bool, @if.Condition);

        var (trueState, falseState) = _narrower.ComputeBranchStates(@if.Condition, _flowState);
        var thenType = CheckBody(@if.ThenBranch, trueState);
        var thenExit = _exitStates.GetValueOrDefault(@if.ThenBranch, trueState);
        var elseExit = @if.ElseBranch != null ? _exitStates.GetValueOrDefault(@if.ElseBranch, falseState) : falseState;
        var elseType = @if.ElseBranch != null ? CheckBody(@if.ElseBranch, falseState) : Types.PrimitiveType.None;

        _exitStates[@if] = MergeExitStates(thenExit, elseExit);
        return BindType(@if, TypeSimplifier.Simplify(new Types.UnionType([thenType, elseType])));
    }

    public override Type VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var typeParameters = functionDeclaration.TypeParameters?.ParameterList.ConvertAll(VisitTypeParameter) ?? [];
        var parameterTypes = functionDeclaration.Parameters?.ParameterList.ConvertAll(Visit) ?? [];
        MaybeVisit(functionDeclaration.ReturnType);

        var returnType = GetReturnType(functionDeclaration);
        var functionType = BindType(functionDeclaration, new Types.FunctionType(typeParameters, parameterTypes, returnType));
        Visit(functionDeclaration.Body);

        return functionType;
    }

    public override Type VisitTypeAlias(TypeAlias typeAlias)
    {
        if (typeAlias.TypeParameters == null)
        {
            var type = Visit(typeAlias.EqualsTypeClause);
            _semanticModel.TypeSolver.CheckCircular(ref type, typeAlias.Name);

            return BindType(typeAlias, TypeSimplifier.Simplify(type));
        }

        var parameters = typeAlias.TypeParameters.ParameterList.ConvertAll(VisitTypeParameter);
        var underlyingType = Visit(typeAlias.EqualsTypeClause);
        _semanticModel.TypeSolver.CheckCircular(ref underlyingType, typeAlias.Name);

        var genericType = new GenericType(typeAlias, parameters, TypeSimplifier.Simplify(underlyingType));
        return BindType(typeAlias, genericType);
    }

    public override Type VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        Type? declaredType = null;
        if (variableDeclaration.ColonTypeClause != null)
            declaredType = Visit(variableDeclaration.ColonTypeClause);

        var initializerType = variableDeclaration.EqualsValueClause != null
            ? Visit(variableDeclaration.EqualsValueClause)
            : declaredType ?? Types.PrimitiveType.Unknown;

        Type finalType = Types.PrimitiveType.None;
        if (declaredType != null)
        {
            if (variableDeclaration.EqualsValueClause != null)
                _semanticModel.TypeSolver.AddConstraint(initializerType, declaredType, variableDeclaration.EqualsValueClause.Value);

            finalType = declaredType;
        }
        else if (variableDeclaration.EqualsValueClause != null)
        {
            finalType = initializerType;
        }

        if (variableDeclaration.Keyword.Kind == SyntaxKind.MutKeyword)
            finalType = finalType.Widen();

        return BindType(variableDeclaration, TypeSimplifier.Simplify(finalType));
    }

    public override Type VisitDeclare(Declare declare)
    {
        var type = declare.Signature switch
        {
            InterfaceDeclaration interfaceDeclaration => Visit(interfaceDeclaration),
            DeclareVariableSignature variableSignature => Visit(variableSignature.ColonTypeClause),
            DeclareFunctionSignature functionSignature => Visit(functionSignature),
            _ => Types.PrimitiveType.Never
        };

        BindType(declare.Signature, type);
        return BindType(declare, type);
    }

    public override Type VisitDeclareFunctionSignature(DeclareFunctionSignature declareFunctionSignature) =>
        new Types.FunctionType(
            declareFunctionSignature.TypeParameters?.ParameterList.ConvertAll(VisitTypeParameter) ?? [],
            declareFunctionSignature.Parameters?.ParameterList.ConvertAll(Visit) ?? [],
            Visit(declareFunctionSignature.ReturnType)
        );

    public override Type VisitParameter(Parameter parameter)
    {
        var declaredType = MaybeVisit(parameter.ColonTypeClause);
        var initializerType = MaybeVisit(parameter.EqualsValueClause);
        if (declaredType != null && parameter.EqualsValueClause != null)
            _semanticModel.TypeSolver.AddConstraint(initializerType!, declaredType, parameter.EqualsValueClause.Value);

        return BindType(parameter, declaredType ?? initializerType!);
    }

    public override Type VisitAsExpression(AsExpression asExpression)
    {
        var expressionType = Visit(asExpression.Expression);
        var castedType = TypeSimplifier.Simplify(Visit(asExpression.Type));
        if (Type.IsNotUnknown(expressionType) && Type.IsNotNever(castedType) && Type.IsNotUnknown(castedType))
            _semanticModel.TypeSolver.AddConstraint(expressionType, castedType, asExpression);

        return BindType(asExpression, castedType);
    }

    public override Type VisitNameOf(NameOf nameOf) => BindType(nameOf, new Types.LiteralType(nameOf.Name.ToString()));

    public override Type VisitInvocation(Invocation invocation)
    {
        var type = Visit(invocation.Expression);
        if (type is not Types.FunctionType functionType)
        {
            _diagnostics.Error(invocation, InternalCodes.InvalidInvocation, $"Cannot call value of type '{type}'.");
            return BindType(invocation, Types.PrimitiveType.Never);
        }

        var declaration = _semanticModel.GetSymbol(invocation.Expression)?.Declaration as DeclareFunctionSignature;
        var argumentTypes = invocation.Arguments.ArgumentList.ConvertAll(Visit);
        if (functionType.TypeParameters.Count == 0)
            return BindNonGenericInvocation(invocation, argumentTypes, functionType, declaration);

        var expectedReturnType = GetContextualType(invocation);
        var substitution = ResolveTypeArguments(invocation, functionType, argumentTypes, expectedReturnType);
        if (substitution == null)
            return BindType(invocation, Types.PrimitiveType.Never);

        var substitutedParameterTypes = SubstituteTypeParameters(functionType.ParameterTypes, substitution);
        var substitutedReturnType = SubstituteTypeParameters(functionType.ReturnType, substitution);
        var instantiated = new Types.FunctionType([], substitutedParameterTypes, substitutedReturnType);

        CheckArity(invocation.Arguments, argumentTypes, substitutedParameterTypes, declaration);
        AddArgumentConstraints(invocation.Arguments, argumentTypes, substitutedParameterTypes);
        return BindType(invocation, instantiated.ReturnType);
    }

    public override Type VisitQualifiedName(QualifiedName qualifiedName) => GetTypeOfNamedAccess(qualifiedName, qualifiedName.Identifier, qualifiedName.Names);
    public override Type VisitPropertyAccess(PropertyAccess propertyAccess) => GetTypeOfNamedAccess(propertyAccess, propertyAccess.Expression, propertyAccess.Names);

    public override Type VisitElementAccess(ElementAccess elementAccess)
    {
        if (TryGetNarrowedType(elementAccess, out var narrowedType))
            return BindType(elementAccess, narrowedType);

        var type = Visit(elementAccess.Expression);
        var indexType = Visit(elementAccess.IndexExpression);
        if (type is Types.ArrayType && indexType.IsAssignableTo(Intrinsics.Range))
        {
            CheckInvalidAccessAssignment(elementAccess, type, indexType);
            return BindType(elementAccess, type);
        }

        var indexIsRangeOrNumber = indexType.IsAssignableTo(Intrinsics.Range) || indexType.IsAssignableTo(Types.PrimitiveType.Number);
        if (!indexIsRangeOrNumber || !type.IsAssignableTo(Types.PrimitiveType.String))
            return IndexType(elementAccess, type, indexType, $"Cannot index value of type '{type}'.");

        CheckInvalidAccessAssignment(elementAccess, type, indexType);
        return BindType(elementAccess, Types.PrimitiveType.String);
    }

    private Type IndexType(Node node, Type type, Type indexType, string errorMessage)
    {
        if (type is InstantiatedType instantiated)
            type = instantiated.Expand();

        switch (type)
        {
            case Types.UnionType union:
            {
                var results = new List<Type>();
                foreach (var member in union.Types)
                {
                    var memberType = GetTypeAtIndexSingle(node, member, indexType);
                    if (Type.IsNever(memberType))
                    {
                        _diagnostics.Error(
                            node,
                            InternalCodes.InvalidAccess,
                            $"Indexing '{indexType}' is not valid for union member '{member}'."
                        );

                        continue;
                    }

                    if (!Type.IsUnknown(memberType))
                        results.Add(memberType);
                }

                if (results.Count == 0)
                    return BindType(node, Types.PrimitiveType.Never);

                return BindType(
                    node,
                    TypeSimplifier.Simplify(new Types.UnionType(results))
                );
            }

            case ObjectType or InterfaceType:
                return GetTypeAtIndex(node, type, indexType);
        }

        _diagnostics.Error(node, InternalCodes.InvalidAccess, errorMessage);
        return BindType(node, Types.PrimitiveType.Never);
    }

    public override Type VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Operator.Kind != SyntaxKind.Equals)
            return base.VisitBinaryOperator(assignmentOperator);

        var targetType = Visit(assignmentOperator.Left);
        var valueType = Visit(assignmentOperator.Right);
        if (assignmentOperator.Left is ElementAccess or PropertyAccess or QualifiedName)
        {
            var expression = assignmentOperator.Left switch
            {
                ElementAccess access => access.Expression,
                PropertyAccess propertyAccess => propertyAccess.Expression,
                QualifiedName name => name.Identifier,
                _ => null!
            };

            var expressionType = _semanticModel.GetType(expression);
            var indexType = assignmentOperator.Left switch
            {
                ElementAccess access => _semanticModel.GetType(access.IndexExpression),
                PropertyAccess propertyAccess => new Types.LiteralType(propertyAccess.Names.First().Name.Text),
                QualifiedName name => new Types.LiteralType(name.Names.First().Name.Text),
                _ => null!
            };

            var objectType = expressionType switch
            {
                ObjectType o => o,
                InterfaceType i => i.ObjectType,
                _ => null
            };

            var names = (assignmentOperator.Left switch
            {
                PropertyAccess propertyAccess => propertyAccess.Names,
                QualifiedName name => name.Names,
                _ => []
            }).ToList();

            if (objectType != null)
            {
                if (names.Count > 1)
                {
                    foreach (var property in names.SkipLast(1).Select(name => objectType.GetProperty(name.Name.Text)!))
                    {
                        objectType = property.ValueType is InterfaceType i ? i.ObjectType : (ObjectType)property.ValueType;
                    }

                    indexType = new Types.LiteralType(names.Last().Name.Text);
                }

                var (bodyType, _) = objectType.GetTypeAtIndex(indexType, expressionType);
                if (bodyType is { IsMutable: false })
                {
                    var display = bodyType switch
                    {
                        ObjectProperty property => $"property '{property.Name}'.",
                        ObjectIndexer indexer => $"index '{indexer.KeyType}'.",
                        _ => ""
                    };

                    _diagnostics.Error(assignmentOperator, InternalCodes.AssignToImmutable, $"Cannot assign to immutable {display}");
                }
            }
        }

        _semanticModel.TypeSolver.AddConstraint(valueType, targetType, assignmentOperator.Right);
        return BindType(assignmentOperator, valueType);
    }

    public override Type VisitTernaryOperator(TernaryOperator ternaryOperator)
    {
        var conditionType = Visit(ternaryOperator.Condition);
        _semanticModel.TypeSolver.AddConstraint(conditionType, Types.PrimitiveType.Bool, ternaryOperator.Condition);

        var (trueState, falseState) = _narrower.ComputeBranchStates(ternaryOperator.Condition, _flowState);
        var thenBranchType = Visit(ternaryOperator.ThenBranch, trueState);
        var elseBranchType = Visit(ternaryOperator.ElseBranch, falseState);
        var union = new Types.UnionType([thenBranchType, elseBranchType]);
        return BindType(ternaryOperator, TypeSimplifier.Simplify(union));
    }

    public override Type VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        var leftType = Visit(binaryOperator.Left);
        Type rightType;
        switch (binaryOperator.Operator.Kind)
        {
            case SyntaxKind.AmpersandAmpersand or SyntaxKind.AmpersandAmpersandEquals:
                var (trueState, _) = _narrower.ComputeBranchStates(binaryOperator.Left, _flowState);
                rightType = Visit(binaryOperator.Right, trueState);
                break;
            case SyntaxKind.PipePipe or SyntaxKind.PipePipeEquals:
                var (_, falseState) = _narrower.ComputeBranchStates(binaryOperator.Left, _flowState);
                rightType = Visit(binaryOperator.Right, falseState);
                break;
            default:
                rightType = Visit(binaryOperator.Right);
                break;
        }

        var rule = BinaryOperatorBinder.GetRule(binaryOperator, leftType, rightType);
        if (rule != null)
        {
            _semanticModel.TypeSolver.AddConstraint(leftType, rule.LeftType, binaryOperator.Left);
            _semanticModel.TypeSolver.AddConstraint(rightType, rule.RightType, binaryOperator.Right);
            return BindType(binaryOperator, rule.ReturnType);
        }

        if (binaryOperator.Operator.Kind is SyntaxKind.QuestionQuestion or SyntaxKind.QuestionQuestionEquals)
        {
            if (!Type.IsOptional(leftType))
            {
                _diagnostics.Warn(
                    binaryOperator,
                    InternalCodes.RedundantCode,
                    $"Null coalescing has no effect since '{leftType}' is not optional."
                );
            }

            return BindType(binaryOperator, TypeSimplifier.Simplify(new Types.UnionType([leftType, rightType]).NonNullable()));
        }

        var suggestion = BinaryOperatorBinder.GetSuggestion(binaryOperator, leftType, rightType);
        var hint = Diagnostic.FormatBinaryHint(binaryOperator, leftType, rightType, suggestion);
        _diagnostics.Error(
            binaryOperator,
            InternalCodes.InvalidBinaryOp,
            $"No binary operation for '{leftType.Widen()}' {binaryOperator.Operator.Text} '{rightType.Widen()}'.",
            hint
        );

        return BindType(binaryOperator, Types.PrimitiveType.Never);
    }

    public override Type VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var operandType = Visit(unaryOperator.Operand);
        var rule = UnaryOperatorBinder.GetRule(unaryOperator, operandType);
        if (rule != null)
            return rule.ReturnType;

        var suggestion = UnaryOperatorBinder.GetSuggestion(unaryOperator, operandType);
        var hint = Diagnostic.FormatUnaryHint(unaryOperator, operandType, suggestion);
        _diagnostics.Error(unaryOperator, InternalCodes.InvalidUnaryOp, $"No unary operation for {unaryOperator.Operator.Text}{operandType.Widen()}.", hint);

        return BindType(unaryOperator, Types.PrimitiveType.Never);
    }

    public override Type VisitRangeLiteral(RangeLiteral rangeLiteral)
    {
        var minimumType = Visit(rangeLiteral.Minimum);
        var maximumType = Visit(rangeLiteral.Maximum);
        _semanticModel.TypeSolver.AddConstraint(minimumType, Types.PrimitiveType.Number, rangeLiteral.Minimum);
        _semanticModel.TypeSolver.AddConstraint(maximumType, Types.PrimitiveType.Number, rangeLiteral.Maximum);

        return BindType(rangeLiteral, Intrinsics.Range);
    }

    public override Type VisitArrayLiteral(ArrayLiteral arrayLiteral)
    {
        // TODO: array literal types for immutable arrays assigned to immutable names
        var expressionTypes = arrayLiteral.Expressions.ConvertAll(Visit).ConvertAll(t => t.Widen());
        var elementType = TypeSimplifier.Simplify(new Types.UnionType(expressionTypes));
        var isMutable = arrayLiteral.MutKeyword != null;
        var type = new Types.ArrayType(elementType, isMutable);
        return BindType(arrayLiteral, isMutable ? type.Widen() : type);
    }

    public override Type VisitLiteral(Literal literal) => BindType(literal, new Types.LiteralType(literal.Value));

    public override Type VisitParenthesized(Parenthesized parenthesized) => BindType(parenthesized, Visit(parenthesized.Expression));

    private bool TryGetNarrowedType(Expression expression, [MaybeNullWhen(false)] out Type narrowedType) =>
        _narrower.TryGetNarrowedType(expression, _flowState, out narrowedType);

    public override Type VisitIdentifier(Identifier identifier)
    {
        if (TryGetNarrowedType(identifier, out var narrowedType))
            return BindType(identifier, narrowedType);

        var symbol = _semanticModel.GetSymbol(identifier);
        if (symbol != null)
        {
            if (symbol is not PropertyVariableSymbol propertyVariableSymbol)
                return BindType(identifier, GetType(symbol));

            var interfaceType = (InterfaceType)_semanticModel.GetType(propertyVariableSymbol.From.Declaration);
            return GetTypeAtIndexInInterface(identifier, interfaceType, new Types.LiteralType(propertyVariableSymbol.Name));
        }

        _diagnostics.Error(identifier, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of variable '{identifier.Name.Text}'.");
        return BindType(identifier, Types.PrimitiveType.Never);
    }

    public override Type VisitIntersectionType(IntersectionType intersectionType) =>
        BindType(intersectionType, new Types.IntersectionType(intersectionType.Types.ConvertAll(Visit)));

    public override Type VisitUnionType(UnionType unionType) => BindType(unionType, new Types.UnionType(unionType.Types.ConvertAll(Visit)));

    public override Type VisitFunctionType(FunctionType functionType) =>
        BindType(
            functionType,
            new Types.FunctionType(
                functionType.TypeParameters?.ParameterList.ConvertAll(VisitTypeParameter) ?? [],
                functionType.Parameters?.ParameterList.ConvertAll(Visit) ?? [],
                Visit(functionType.ReturnType)
            )
        );

    public override Type VisitKeyOf(KeyOf keyOf)
    {
        var targetType = Visit(keyOf.Type);
        if (targetType is InstantiatedType instantiated)
            targetType = instantiated.Expand();

        if (targetType is not (ObjectType or InterfaceType))
        {
            _diagnostics.Error(keyOf, InternalCodes.InvalidKeyOf, $"Cannot access keys of type '{targetType.Widen()}'.");
            return BindType(keyOf, Types.PrimitiveType.Never);
        }

        var objectType = targetType is InterfaceType interfaceType ? interfaceType.ObjectType : (ObjectType)targetType;
        var type = objectType.KeyUnion();
        return BindType(keyOf, type);
    }

    public override Type VisitIndexedType(IndexedType indexedType)
    {
        var targetType = Visit(indexedType.Type);
        var indexType = Visit(indexedType.IndexType);
        if (targetType is InstantiatedType instantiated)
            targetType = instantiated.Expand();

        if (targetType is not (ObjectType or InterfaceType))
        {
            _diagnostics.Error(indexedType, InternalCodes.InvalidAccess, $"Type '{indexType}' cannot be used to index type '{targetType}'.");
            return BindType(indexedType, Types.PrimitiveType.Never);
        }

        var type = GetTypeAtIndex(indexedType, targetType, indexType);
        return BindType(indexedType, type);
    }

    public override Type VisitArrayType(ArrayType arrayType) => BindType(arrayType, new Types.ArrayType(Visit(arrayType.ElementType), arrayType.MutKeyword != null));
    public override Type VisitOptionalType(OptionalType optionalType) => BindType(optionalType, new Types.OptionalType(Visit(optionalType.NonNullableType)));
    public override Type VisitPrimitiveType(PrimitiveType primitiveType) => BindType(primitiveType, new Types.PrimitiveType(primitiveType.Kind));
    public override Type VisitLiteralType(LiteralType literalType) => BindType(literalType, new Types.LiteralType(literalType.Value));

    public override Type VisitTypeName(TypeName typeName)
    {
        var symbol = _semanticModel.GetSymbol(typeName);
        if (symbol != null)
        {
            var declaredType = GetType(symbol);
            if (symbol is { Kind: SymbolKind.EnumType } && declaredType is ObjectType objectType)
                return BindType(typeName, typeName.Parent is IndexedType or KeyOf ? objectType : objectType.PropertyUnion());

            if (declaredType is GenericType genericType)
                return InstantiateGenericType(typeName, typeName.TypeArguments, genericType);

            if (typeName.TypeArguments == null)
                return BindType(typeName, declaredType);

            _diagnostics.Error(typeName, InternalCodes.NotGeneric, $"Type '{typeName.Name.Text}' is not generic and cannot receive type arguments.");
            return BindType(typeName, Types.PrimitiveType.Never);
        }

        _diagnostics.Error(typeName, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of type '{typeName.Name.Text}'.");
        return BindType(typeName, Types.PrimitiveType.Never);
    }

    public override Types.TypeParameter VisitTypeParameter(TypeParameter typeParameter)
    {
        var defaultType = MaybeVisit(typeParameter.EqualsTypeClause);
        var constraint = MaybeVisit(typeParameter.ColonTypeClause);
        if (defaultType != null)
        {
            _semanticModel.TypeSolver.CheckCircular(ref defaultType, typeParameter.Name);
            if (constraint != null)
                _semanticModel.TypeSolver.AddConstraint(defaultType, constraint, typeParameter.EqualsTypeClause!);
        }

        var parameter = new Types.TypeParameter(typeParameter.Name.Text, constraint, defaultType);
        return BindType(typeParameter, parameter);
    }

    private Type? GetContextualType(Expression expression) =>
        expression.Parent switch
        {
            EqualsValueClause equalsValueClause when equalsValueClause.Value == expression
                && equalsValueClause.Parent is VariableDeclaration { ColonTypeClause: not null } variableDeclaration =>
                _semanticModel.GetType(variableDeclaration.ColonTypeClause),

            EqualsValueClause equalsValueClause when equalsValueClause.Value == expression
                && equalsValueClause.Parent is Parameter { ColonTypeClause: not null } variableDeclaration =>
                _semanticModel.GetType(variableDeclaration.ColonTypeClause),

            Return @return when @return.Expression == expression =>
                GetEnclosingDeclaredReturnType(@return),

            AssignmentOperator { Operator.Kind: SyntaxKind.Equals } assignment
                when assignment.Right == expression =>
                _semanticModel.GetType(assignment.Left),

            _ => null
        };

    private Type? GetEnclosingDeclaredReturnType(Return @return)
    {
        var enclosingFunction = @return.FirstAncestorOfType<FunctionDeclaration>();
        return enclosingFunction?.ReturnType != null
            ? ((Types.FunctionType)_semanticModel.GetType(enclosingFunction)).ReturnType
            : null;
    }

    private Type GetTypeOfNamedAccess(Expression accessExpression, Expression targetExpression, List<DotName> names)
    {
        if (TryGetNarrowedType(accessExpression, out var narrowedType))
            return BindType(accessExpression, narrowedType);

        var type = Visit(targetExpression);
        foreach (var indexType in names.Select(name => new Types.LiteralType(name.Name.Text)))
        {
            type = IndexType(accessExpression, type, indexType, $"Cannot access property '{indexType.Value}' on type '{type}'.");
            if (Type.IsNever(type))
                return type;
        }

        return BindType(accessExpression, type);
    }

    private Type GetTypeAtIndex(Node node, Type type, Type indexType)
    {
        if (type is Types.UnionType union)
        {
            var results = union.Types
                .ConvertAll(member => GetTypeAtIndexSingle(node, member, indexType))
                .FindAll(memberResult => !Type.IsNever(memberResult) && !Type.IsUnknown(memberResult));

            return results.Count == 0
                ? ReportCannotUseToIndex(node, type, indexType)
                : BindType(node, TypeSimplifier.Simplify(new Types.UnionType(results)));
        }

        if (indexType is not Types.UnionType indexUnion || !indexUnion.Types.All(t => t is Types.LiteralType { Value: string }))
            return GetTypeAtIndexSingle(node, type, indexType);

        var stringLiteralResults = indexUnion.Types
            .Select(t => GetTypeAtIndexSingle(node, type, t))
            .Where(r => !Type.IsNever(r) && !Type.IsUnknown(r))
            .ToList();

        return stringLiteralResults.Count != 0
            ? BindType(node, TypeSimplifier.Simplify(new Types.UnionType(stringLiteralResults)))
            : ReportCannotUseToIndex(node, type, indexType);
    }

    private Type GetTypeAtIndexSingle(Node node, Type type, Type indexType) =>
        type switch
        {
            ObjectType objectType => GetTypeAtIndexInObject(node, objectType, indexType),
            InterfaceType interfaceType => GetTypeAtIndexInInterface(node, interfaceType, indexType),
            InstantiatedType instantiated => GetTypeAtIndex(node, instantiated.Expand(), indexType),
            _ => type
        };

    private Type GetTypeAtIndexInInterface(Node node, InterfaceType interfaceType, Type indexType)
    {
        var result = interfaceType.ObjectType.GetTypeAtIndex(indexType, interfaceType);
        var (bodyType, cannotFindReason) = result;
        if (bodyType != null)
            return BindType(node, bodyType.ValueType);

        var type = interfaceType.Constraints.Count > 0
            ? interfaceType.Constraints.ConvertAll(t => GetTypeAtIndexInInterface(node, t, indexType)).Find(Type.IsNotNever) ?? Types.PrimitiveType.Never
            : Types.PrimitiveType.Never;

        return Type.IsNever(type)
            ? ReportCannotUseToIndex(node, interfaceType, indexType, cannotFindReason)
            : BindType(node, type);
    }

    private Type GetTypeAtIndexInObject(Node node, ObjectType objectType, Type indexType)
    {
        var (bodyType, cannotFindReason) = objectType.GetTypeAtIndex(indexType);
        return bodyType != null
            ? BindType(node, bodyType.ValueType)
            : ReportCannotUseToIndex(node, objectType, indexType, cannotFindReason);
    }

    private static Type GetObjectValueType(Type type) =>
        type switch
        {
            Types.ArrayType array => array.ElementType,
            InterfaceType interfaceType => GetObjectValueType(interfaceType.ObjectType),
            ObjectType objectType => objectType.ValueUnion(),
            _ => Types.PrimitiveType.Never
        };

    private void CheckInvalidAccessAssignment(ElementAccess elementAccess, Type type, Type indexType)
    {
        if (elementAccess.Parent is not AssignmentOperator assignmentOperator) return;
        _diagnostics.Error(
            assignmentOperator,
            InternalCodes.InvalidAccess,
            $"Cannot assign to '{type.Widen()}[{indexType.Widen()}]' because the expression will be replaced by a macro."
        );
    }

    private void CheckArity(Arguments arguments, List<Type> argumentTypes, List<Type> parameterTypes, DeclareFunctionSignature? declaration)
    {
        var requiredParameterTypes = new List<Type>();
        if (declaration == null)
        {
            requiredParameterTypes = parameterTypes.FindAll(Type.IsNotOptional);
        }
        else
        {
            if (declaration.Parameters != null)
            {
                for (var i = 0; i < declaration.Parameters.ParameterList.Count; i++)
                {
                    var parameterType = parameterTypes[i];
                    var parameter = declaration.Parameters.ParameterList[i];
                    if (parameter.EqualsValueClause != null || !Type.IsNotOptional(parameterType)) continue;

                    requiredParameterTypes.Add(parameterType);
                }
            }
        }

        var minimum = requiredParameterTypes.Count;
        var maximum = parameterTypes.Count;

        var arityDisplay = minimum == maximum
            ? maximum.ToString()
            : $"{minimum}-{maximum}";

        if (argumentTypes.Count <= maximum && argumentTypes.Count >= minimum) return;

        var s = minimum != maximum || maximum != 1 ? "s" : "";
        _diagnostics.Error(arguments, InternalCodes.InvocationArity, $"Function expects {arityDisplay} argument{s}, but {argumentTypes.Count} were provided.");
    }

    private void AddArgumentConstraints(Arguments arguments, List<Type> argumentTypes, List<Type> parameterTypes)
    {
        for (var i = 0; i < Math.Min(argumentTypes.Count, parameterTypes.Count); i++)
        {
            _semanticModel.TypeSolver.AddConstraint(
                argumentTypes[i],
                parameterTypes[i],
                arguments.ArgumentList[i]
            );
        }
    }

    private Type BindNonGenericInvocation(Invocation invocation, List<Type> argumentTypes, Types.FunctionType functionType, DeclareFunctionSignature? declaration)
    {
        CheckArity(invocation.Arguments, argumentTypes, functionType.ParameterTypes, declaration);
        AddArgumentConstraints(invocation.Arguments, argumentTypes, functionType.ParameterTypes);
        return BindType(invocation, functionType.ReturnType);
    }

    private List<Type> CheckStatements(Node node, List<Statement> statements)
    {
        var current = _flowState;
        var types = new List<Type>(statements.Count);
        foreach (var statement in statements)
        {
            types.Add(Visit(statement, current));
            current = GetStatementExitState(statement, current);
        }

        _exitStates[node] = current;
        return types;
    }

    private Type CheckBody(Statement body, FlowState current)
    {
        var type = Visit(body, current);
        if (body is Block)
            return type;

        var exit = GetStatementExitState(body, current);
        _exitStates[body] = exit;

        return type;
    }

    private FlowState GetStatementExitState(Statement statement, FlowState entryState) =>
        statement switch
        {
            Block or If or While or For or After => _exitStates.GetValueOrDefault(statement, entryState),
            Return or Break or Continue => new FlowState(entryState) { IsUnreachable = true },
            _ => entryState
        };

    private void AssignLoopExitState(Node node)
    {
        var exits = _loopExitScopes.Pop();
        var bodyExit = exits.Aggregate(_flowState, MergeExitStates);
        _exitStates[node] = bodyExit;
    }

    private static FlowState MergeExitStates(FlowState left, FlowState right) =>
        left.IsUnreachable
            ? right
            : right.IsUnreachable
                ? left
                : left.Merge(right);

    private Type GetReturnType(FunctionDeclaration functionDeclaration)
    {
        if (functionDeclaration.ReturnType != null)
            return Visit(functionDeclaration.ReturnType);

        var possibleReturnTypes = functionDeclaration.Body is ExpressionBody body
            ? [Visit(body)]
            : functionDeclaration.Body
                .GetDescendants<Return>()
                .FindAll(returnStatement => returnStatement.FirstAncestorOfType<FunctionDeclaration>() == functionDeclaration)
                .ConvertAll(Visit);

        return possibleReturnTypes.Count == 0 ? Types.PrimitiveType.Void : TypeSimplifier.Simplify(new Types.UnionType(possibleReturnTypes));
    }

    private Type GetType(Symbol symbol) =>
        symbol is { IsGlobal: true, IsIntrinsic: false } && symbol.File.AbsolutePath != _semanticModel.Tree.File.AbsolutePath
            ? Visit(symbol.Declaration)
            : _semanticModel.GetType(symbol.Declaration);

    private T BindType<T>(Node node, T type)
        where T : Type
    {
        _semanticModel.TypeSolver.SetType(node, type);
        if (node is Tree or ExpressionStatement)
            return type;

        var simplified = TypeSimplifier.Simplify(type);
        _diagnostics.Debug(node, $"Solved type '{(simplified is InterfaceType i ? $"{i.ObjectType} ({i.Name})" : simplified)}' for {node.GetType().Name}");

        return type;
    }

    private Type ReportCannotUseToIndex(Node node, Type objectType, Type indexType, string? cannotFindReason = "")
    {
        _diagnostics.Error(node, InternalCodes.InvalidAccess, $"Expression of type '{indexType}' cannot be used to index type '{objectType}'.{cannotFindReason}");
        return BindType(node, Types.PrimitiveType.Never);
    }
}