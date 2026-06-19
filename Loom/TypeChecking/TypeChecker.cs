using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using ArrayType = Loom.Parsing.AST.ArrayType;
using FunctionType = Loom.Parsing.AST.FunctionType;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using LiteralType = Loom.Parsing.AST.LiteralType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;
using TypeName = Loom.Parsing.AST.TypeName;
using TypeParameter = Loom.Parsing.AST.TypeParameter;
using UnionType = Loom.Parsing.AST.UnionType;

namespace Loom.TypeChecking;

public sealed class TypeChecker(SemanticModel semanticModel) : Visitor<Type>
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Stack<TypedFlowState> _flowStates = [];

    public TypeCheckerResult Check()
    {
        var tree = semanticModel.Tree;
        var type = BindType(tree, VisitTree(tree));
        semanticModel.TypeSolver.SolveConstraints();

        var diagnostics = DiagnosticBag.Concat([semanticModel.TypeSolver.Diagnostics, _diagnostics]);
        return new TypeCheckerResult(type, diagnostics);
    }

    public void ReportCannotInfer(Node node, Types.TypeParameter typeParameter) =>
        _diagnostics.Error(
            node,
            InternalCodes.CannotInferType,
            $"Cannot infer type parameter '{typeParameter.Name}'. Provide explicit type arguments."
        );

    protected override Type Visit(Node node) => node.Accept(this);

    public override Type VisitTree(Tree tree)
    {
        _flowStates.Push(new TypedFlowState());
        base.VisitTree(tree);
        _flowStates.Pop();

        return tree.Statements.Count > 0
            ? semanticModel.GetType(tree.Statements.Last())
            : Types.PrimitiveType.Never;
    }

    public override Type VisitBlock(Block block)
    {
        var types = block.Statements.ConvertAll(Visit);
        return types.LastOrDefault(Types.PrimitiveType.None);
    }

    public override Type VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        var type = TypeSimplifier.Simplify(base.VisitExpressionStatement(expressionStatement));
        _diagnostics.Info(expressionStatement, $"Solved type '{(type is InterfaceType i ? $"{i.ObjectType} ({i.Name})" : type)}' for expression");
        return BindType(expressionStatement, type);
    }

    public override Type VisitIf(If @if)
    {
        var conditionType = Visit(@if.Condition);
        semanticModel.TypeSolver.AddConstraint(conditionType, Types.PrimitiveType.Bool, @if.Condition);

        var (trueState, falseState) = ComputeBranchStates(@if.Condition);
        var thenBranchType = VisitWithFlowState(@if.ThenBranch, trueState);
        var elseBranchType = @if.ElseBranch != null ? VisitWithFlowState(@if.ElseBranch, falseState) : Types.PrimitiveType.None;
        return TypeSimplifier.Simplify(new Types.UnionType([thenBranchType, elseBranchType]));
    }

    public override Type VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var typeParameters = functionDeclaration.TypeParameters?.ParameterList.ConvertAll(VisitTypeParameter) ?? [];
        var parameterTypes = functionDeclaration.Parameters?.ParameterList.ConvertAll(Visit) ?? [];
        var returnType = GetReturnType(functionDeclaration);
        var functionType = BindType(functionDeclaration, new Types.FunctionType(typeParameters, parameterTypes, returnType));
        Visit(functionDeclaration.Body);

        _diagnostics.Info(functionDeclaration, $"Solved type '{TypeSimplifier.Simplify(functionType)}' for function");
        return functionType;
    }

    public override Type VisitTypeAlias(TypeAlias typeAlias)
    {
        if (typeAlias.TypeParameters == null)
        {
            var type = Visit(typeAlias.EqualsTypeClause);
            return BindType(typeAlias, TypeSimplifier.Simplify(type));
        }

        var parameters = typeAlias.TypeParameters.ParameterList.ConvertAll(VisitTypeParameter);
        var underlyingType = Visit(typeAlias.EqualsTypeClause);
        var genericType = new GenericType(typeAlias, parameters, underlyingType);
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
                semanticModel.TypeSolver.AddConstraint(initializerType, declaredType, variableDeclaration.EqualsValueClause.Value);

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
            DeclareVariableSignature variableSignature => Visit(variableSignature.ColonTypeClause),
            DeclareFunctionSignature functionSignature => new Types.FunctionType(
                functionSignature.TypeParameters?.ParameterList.ConvertAll(VisitTypeParameter) ?? [],
                functionSignature.Parameters?.ParameterList.ConvertAll(Visit) ?? [],
                Visit(functionSignature.ReturnType)
            ),
            _ => Types.PrimitiveType.Never
        };

        BindType(declare.Signature, type);
        return BindType(declare, type);
    }

    public override Type VisitParameter(Parameter parameter)
    {
        var declaredType = MaybeVisit(parameter.ColonTypeClause);
        var initializerType = MaybeVisit(parameter.EqualsValueClause);
        if (initializerType != null && parameter.EqualsValueClause != null)
            semanticModel.TypeSolver.AddConstraint(initializerType, declaredType!, parameter.EqualsValueClause.Value);

        return BindType(parameter, declaredType ?? initializerType!);
    }

    public override Type VisitEnumDeclaration(EnumDeclaration enumDeclaration)
    {
        var properties = new List<ObjectProperty>();
        var baseType = MaybeVisit(enumDeclaration.ColonTypeClause) ?? Types.PrimitiveType.Number;
        if (enumDeclaration.ColonTypeClause != null && !baseType.IsAssignableTo(Types.PrimitiveType.String) && !baseType.IsAssignableTo(Types.PrimitiveType.Number))
        {
            _diagnostics.Error(
                enumDeclaration.ColonTypeClause,
                InternalCodes.InvalidEnumBaseType,
                "Invalid enum base type.",
                "valid types are 'string' and 'number'"
            );

            return BindType(enumDeclaration, Types.PrimitiveType.Never);
        }

        if (!baseType.IsAssignableTo(Types.PrimitiveType.String))
        {
            var nextValue = 0d;
            foreach (var member in enumDeclaration.Members)
            {
                var memberValue = nextValue;
                if (member.EqualsValueClause != null)
                {
                    var explicitType = Visit(member.EqualsValueClause);
                    if (CheckEnumMemberIsConstant(member, explicitType))
                    {
                        memberValue = explicitType switch
                        {
                            Types.LiteralType { Value: long l } => l,
                            Types.LiteralType { Value: int i } => i,
                            Types.LiteralType { Value: double d } => d,
                            _ => nextValue
                        };

                        semanticModel.TypeSolver.AddConstraint(explicitType, baseType, member.EqualsValueClause.Value);
                    }
                }

                var memberType = new Types.LiteralType(memberValue);
                if (CheckEnumMemberIsConstant(member, memberType))
                    properties.Add(new ObjectProperty(false, member.Name.Text, memberType));

                nextValue = memberValue + 1;
            }

            return BindType(enumDeclaration, new ObjectType(null, properties));
        }

        foreach (var member in enumDeclaration.Members)
        {
            if (member.EqualsValueClause == null)
            {
                _diagnostics.Error(
                    member,
                    InternalCodes.StringEnumMemberMustHaveInitializer,
                    $"Member '{member.Name.Text}' of string enum '{enumDeclaration.Name.Text}' must have an initializer."
                );

                return BindType(enumDeclaration, Types.PrimitiveType.Never);
            }

            var type = MaybeVisit(member.EqualsValueClause) ?? baseType;
            if (!CheckEnumMemberIsConstant(member, type)) continue;

            semanticModel.TypeSolver.AddConstraint(type, baseType, member.EqualsValueClause.Value);
            properties.Add(new ObjectProperty(false, member.Name.Text, type));
        }

        return BindType(enumDeclaration, new ObjectType(null, properties));
    }

    public override Type VisitInterfaceDeclaration(InterfaceDeclaration interfaceDeclaration)
    {
        var name = interfaceDeclaration.Name.Text;
        var constraintTypes = interfaceDeclaration.ColonTypeListClause?.Types.ConvertAll(Visit) ?? [];
        if (constraintTypes.Any(constraintType => constraintType is not InterfaceType))
        {
            _diagnostics.Error(
                interfaceDeclaration.ColonTypeListClause!,
                InternalCodes.InvalidInterfaceConstraint,
                "Interfaces may only be constrained by other interfaces."
            );

            return Types.PrimitiveType.Never;
        }

        var parameters = interfaceDeclaration.TypeParameters?.ParameterList.ConvertAll(VisitTypeParameter);
        var constraints = constraintTypes.OfType<InterfaceType>().ToList();
        var indexerDeclaration = interfaceDeclaration.Body?.Members.OfType<IndexerDeclaration>().FirstOrDefault();
        var propertyDeclarations = interfaceDeclaration.Body?.Members.OfType<PropertyDeclaration>() ?? [];
        ObjectIndexer? indexer = null;

        if (indexerDeclaration != null)
        {
            var isMutable = indexerDeclaration.MutKeyword != null;
            var indexType = Visit(indexerDeclaration.IndexType);
            var valueType = Visit(indexerDeclaration.ColonTypeClause);
            indexer = new ObjectIndexer(isMutable, indexType, valueType);
        }

        var properties =
            from declaration in propertyDeclarations
            let isMutable = declaration.MutKeyword != null
            let valueType = Visit(declaration.ColonTypeClause)
            select new ObjectProperty(isMutable, declaration.Name.Text, valueType);

        var interfaceType = new InterfaceType(name, constraints, new ObjectType(indexer, properties.ToList()));
        if (parameters == null)
            return BindType(interfaceDeclaration, interfaceType);

        var genericType = new GenericType(interfaceDeclaration, parameters, interfaceType);
        return BindType(interfaceDeclaration, genericType);
    }

    public override Type VisitAsExpression(AsExpression asExpression)
    {
        var expressionType = Visit(asExpression.Expression);
        var castedType = Visit(asExpression.Type);
        if (Type.IsNotUnknown(expressionType) && Type.IsNotNever(castedType) && Type.IsNotUnknown(castedType))
            semanticModel.TypeSolver.AddConstraint(expressionType, castedType, asExpression);

        return BindType(asExpression, castedType);
    }

    public override Type VisitNameOf(NameOf nameOf) => new Types.LiteralType(nameOf.Name.ToString());

    public override Type VisitInvocation(Invocation invocation)
    {
        var type = Visit(invocation.Expression);
        if (type is not Types.FunctionType functionType)
        {
            _diagnostics.Error(invocation, InternalCodes.InvalidInvocation, $"Cannot call value of type '{type}'");
            return BindType(invocation, Types.PrimitiveType.Never);
        }

        var argumentTypes = invocation.Arguments.ArgumentList.ConvertAll(Visit);
        if (functionType.TypeParameters.Count == 0)
            return BindNonGenericInvocation(invocation, argumentTypes, functionType);

        var substitution = ResolveTypeArguments(invocation, functionType, argumentTypes);
        if (substitution == null)
            return BindType(invocation, Types.PrimitiveType.Never);

        var substitutedParameterTypes = SubstituteTypeParameters(functionType.ParameterTypes, substitution);
        var substitutedReturnType = SubstituteTypeParameters(functionType.ReturnType, substitution);
        var instantiated = new Types.FunctionType([], substitutedParameterTypes, substitutedReturnType);

        CheckArity(invocation.Arguments, argumentTypes, substitutedParameterTypes);
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
        if (type is Types.ArrayType && indexType.IsAssignableTo(IntrinsicTypes.Range))
        {
            CheckInvalidAccessAssignment(elementAccess, type, indexType);
            return BindType(elementAccess, type);
        }

        var indexIsRangeOrNumber = indexType.IsAssignableTo(IntrinsicTypes.Range) || indexType.IsAssignableTo(Types.PrimitiveType.Number);
        if (indexIsRangeOrNumber && type.IsAssignableTo(Types.PrimitiveType.String))
        {
            CheckInvalidAccessAssignment(elementAccess, type, indexType);
            return BindType(elementAccess, Types.PrimitiveType.String);
        }

        switch (type)
        {
            case ObjectType or InterfaceType:
                return GetTypeAtIndex(elementAccess, type, indexType);
            default:
                _diagnostics.Error(elementAccess, InternalCodes.InvalidAccess, $"Cannot index value of type '{type}'");
                return BindType(elementAccess, Types.PrimitiveType.Never);
        }
    }

    public override Type VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        var targetType = Visit(assignmentOperator.Left);
        var valueType = Visit(assignmentOperator.Right);
        if (assignmentOperator.Operator.Kind != SyntaxKind.Equals)
            return base.VisitBinaryOperator(assignmentOperator);

        if (assignmentOperator.Left is ElementAccess or PropertyAccess or QualifiedName)
        {
            var expression = assignmentOperator.Left switch
            {
                ElementAccess access => access.Expression,
                PropertyAccess propertyAccess => propertyAccess.Expression,
                QualifiedName name => name.Identifier,
                _ => null!
            };

            var expressionType = semanticModel.GetType(expression);
            var indexType = assignmentOperator.Left switch
            {
                ElementAccess access => semanticModel.GetType(access.IndexExpression),
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

        semanticModel.TypeSolver.AddConstraint(valueType, targetType, assignmentOperator.Right);
        return BindType(assignmentOperator, valueType);
    }

    public override Type VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        var leftType = Visit(binaryOperator.Left);
        var rightType = Visit(binaryOperator.Right);
        var rule = BinaryOperatorBinder.GetRule(binaryOperator, leftType, rightType);
        if (rule != null)
        {
            semanticModel.TypeSolver.AddConstraint(leftType, rule.LeftType, binaryOperator.Left);
            semanticModel.TypeSolver.AddConstraint(rightType, rule.RightType, binaryOperator.Right);
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
        var hint = FormatBinaryHint(binaryOperator, leftType, rightType, suggestion);
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
        var hint = FormatUnaryHint(unaryOperator, operandType, suggestion);
        _diagnostics.Error(unaryOperator, InternalCodes.InvalidUnaryOp, $"No unary operation for {unaryOperator.Operator.Text}{operandType.Widen()}.", hint);

        return BindType(unaryOperator, Types.PrimitiveType.Never);
    }

    public override Type VisitRangeLiteral(RangeLiteral rangeLiteral)
    {
        var minimumType = Visit(rangeLiteral.Minimum);
        var maximumType = Visit(rangeLiteral.Maximum);
        semanticModel.TypeSolver.AddConstraint(minimumType, Types.PrimitiveType.Number, rangeLiteral.Minimum);
        semanticModel.TypeSolver.AddConstraint(maximumType, Types.PrimitiveType.Number, rangeLiteral.Maximum);

        return BindType(rangeLiteral, IntrinsicTypes.Range);
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

    public override Type VisitIdentifier(Identifier identifier)
    {
        if (TryGetNarrowedType(identifier, out var narrowedType))
            return BindType(identifier, narrowedType);

        var symbol = semanticModel.GetSymbol(identifier);
        if (symbol != null)
            return BindType(identifier, GetDeclarationType(symbol));

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

    public override Type VisitArrayType(ArrayType arrayType) => BindType(arrayType, new Types.ArrayType(Visit(arrayType.ElementType), arrayType.MutKeyword != null));
    public override Type VisitOptionalType(OptionalType optionalType) => BindType(optionalType, new Types.OptionalType(Visit(optionalType.NonNullableType)));
    public override Type VisitPrimitiveType(PrimitiveType primitiveType) => BindType(primitiveType, new Types.PrimitiveType(primitiveType.Kind));
    public override Type VisitLiteralType(LiteralType literalType) => BindType(literalType, new Types.LiteralType(literalType.Value));

    public override Type VisitTypeName(TypeName typeName)
    {
        var symbol = semanticModel.GetSymbol(typeName);
        if (symbol != null)
        {
            var declaredType = GetDeclarationType(symbol);
            if (symbol is { Kind: SymbolKind.EnumType } && declaredType is ObjectType objectType)
                return BindType(typeName, objectType.PropertyUnion());

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
        if (defaultType != null && constraint != null)
            semanticModel.TypeSolver.AddConstraint(defaultType, constraint, typeParameter.EqualsTypeClause!);

        var parameter = new Types.TypeParameter(typeParameter.Name.Text, constraint, defaultType);
        return BindType(typeParameter, parameter);
    }

    private Type VisitWithFlowState(Node node, TypedFlowState state)
    {
        _flowStates.Push(state);
        var type = Visit(node);
        _flowStates.Pop();

        return type;
    }

    private bool TryGetNarrowedType(Expression expression, [MaybeNullWhen(false)] out Type narrowedType)
    {
        if (_flowStates.TryPeek(out var flow) && GetFlowAddress(expression) is { } address && flow.NarrowedTypes.TryGetValue(address, out var narrowed))
        {
            narrowedType = BindType(expression, narrowed);
            return true;
        }

        narrowedType = null;
        return false;
    }

    private (TypedFlowState trueState, TypedFlowState falseState) ComputeBranchStates(Expression condition)
    {
        var current = _flowStates.Peek();
        if (condition is BinaryOperator { Operator.Kind: SyntaxKind.EqualsEquals or SyntaxKind.BangEquals } binaryOperator)
            return NarrowByCondition(binaryOperator, current);

        return (new TypedFlowState(current), new TypedFlowState(current));
    }

    private (TypedFlowState trueState, TypedFlowState falseState) NarrowByCondition(BinaryOperator binaryOperator, TypedFlowState current)
    {
        var trueState = new TypedFlowState(current);
        var falseState = new TypedFlowState(current);
        if (TryGetExpressionAndLiteral(binaryOperator.Left, binaryOperator.Right, out var expr, out var literal)
            || TryGetExpressionAndLiteral(binaryOperator.Right, binaryOperator.Left, out expr, out literal))
        {
            ApplyBinaryNarrowing(expr, literal, binaryOperator.Operator.Kind, trueState, falseState);
        }

        return (trueState, falseState);
    }

    private bool TryGetExpressionAndLiteral(
        Expression expr1,
        Expression expr2,
        [MaybeNullWhen(false)] out Expression expr,
        [MaybeNullWhen(false)] out Expression literal)
    {
        if (!IsCompileTimeLiteral(expr2))
        {
            expr = null;
            literal = null;
            return false;
        }

        expr = expr1;
        literal = expr2;
        return true;
    }

    private bool IsCompileTimeLiteral(Expression expr) =>
        expr is Literal or NameOf || expr is QualifiedName && semanticModel.GetDeclaringSymbol(expr)?.Declaration is EnumDeclaration;

    private void ApplyBinaryNarrowing(
        Expression expression,
        Expression literal,
        SyntaxKind operatorKind,
        TypedFlowState trueState,
        TypedFlowState falseState)
    {
        var address = GetFlowAddress(expression);
        if (address == null) return;

        var baseType = semanticModel.GetType(expression);
        var literalType = semanticModel.GetType(literal);
        var isNone = literal is Literal { Value: null };
        var isEquals = operatorKind == SyntaxKind.EqualsEquals;

        if (isNone)
        {
            if (isEquals)
            {
                trueState.NarrowedTypes[address] = literalType;
                falseState.NarrowedTypes[address] = baseType.NonNullable();
            }
            else
            {
                trueState.NarrowedTypes[address] = baseType.NonNullable();
                falseState.NarrowedTypes[address] = literalType;
            }
        }
        else
        {
            if (isEquals)
                trueState.NarrowedTypes[address] = literalType;
            else
                falseState.NarrowedTypes[address] = literalType;
        }
    }

    private TypedFlowAddress? GetFlowAddress(Expression expr) =>
        expr switch
        {
            Identifier identifier => GetIdentifierFlowAddress(identifier),
            QualifiedName qualifiedName => BuildFieldChain(qualifiedName.Identifier, qualifiedName.Names),
            PropertyAccess propertyAccess => BuildFieldChain(propertyAccess.Expression, propertyAccess.Names),
            ElementAccess elementAccess => GetElementAddress(elementAccess),
            _ => null
        };

    private TypedFlowAddress? BuildFieldChain(Expression baseExpr, List<DotName> dotNames)
    {
        var address = GetFlowAddress(baseExpr);
        return address == null
            ? null
            : dotNames.Aggregate(address, (current, name) => TypedFlowAddress.Field(current, name.Name.Text));
    }

    private TypedFlowAddress? GetElementAddress(ElementAccess elementAccess)
    {
        if (GetFlowAddress(elementAccess.Expression) is not { } baseAddress)
            return null;

        if (elementAccess.IndexExpression is Literal { Value: not null and not bool } literal)
            return TypedFlowAddress.Element(baseAddress, literal.Value);

        return null;
    }

    private TypedFlowAddress? GetIdentifierFlowAddress(Identifier identifier)
    {
        var symbol = semanticModel.GetSymbol(identifier);
        return symbol != null ? TypedFlowAddress.Variable(symbol) : null;
    }

    private bool CheckEnumMemberIsConstant(EnumMember member, Type type)
    {
        if (type is Types.LiteralType { Value: string or long or int or double })
            return true;

        _diagnostics.Error(member.EqualsValueClause!.Value, InternalCodes.DynamicEnumMemberInitializer, "Enum member initializers must be constant values.");
        return false;
    }

    private Type GetTypeOfNamedAccess(Expression accessExpression, Expression targetExpression, List<DotName> names)
    {
        if (TryGetNarrowedType(accessExpression, out var narrowedType))
            return BindType(accessExpression, narrowedType);

        var type = Visit(targetExpression);
        foreach (var dotName in names)
        {
            if (type is not (ObjectType or InterfaceType))
            {
                _diagnostics.Error(accessExpression, InternalCodes.InvalidAccess, $"Cannot access property '{dotName.Name.Text}' on type '{type}'.");
                return BindType(accessExpression, Types.PrimitiveType.Never);
            }

            var indexType = new Types.LiteralType(dotName.Name.Text);
            type = GetTypeAtIndex(accessExpression, type, indexType);
        }

        return BindType(accessExpression, type);
    }

    private Type GetTypeAtIndex(Expression accessExpression, Type type, Type indexType) =>
        type switch
        {
            ObjectType objectType => GetTypeAtIndexInObject(accessExpression, objectType, indexType),
            InterfaceType interfaceType => GetTypeAtIndexInInterface(accessExpression, interfaceType, indexType),
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

        if (Type.IsNever(type))
            _diagnostics.Error(
                node,
                InternalCodes.InvalidAccess,
                $"Expression of type '{indexType}' cannot be used to index type '{interfaceType}'.{cannotFindReason}"
            );

        return BindType(node, type);
    }

    private Type GetTypeAtIndexInObject(Node node, ObjectType objectType, Type indexType)
    {
        var (bodyType, cannotFindReason) = objectType.GetTypeAtIndex(indexType);
        if (bodyType != null)
            return BindType(node, bodyType.ValueType);

        _diagnostics.Error(node, InternalCodes.InvalidAccess, $"Expression of type '{indexType}' cannot be used to index type '{objectType}'.{cannotFindReason}");
        return BindType(node, Types.PrimitiveType.Never);
    }

    private void CheckInvalidAccessAssignment(ElementAccess elementAccess, Type type, Type indexType)
    {
        if (elementAccess.Parent is not AssignmentOperator assignmentOperator) return;
        _diagnostics.Error(
            assignmentOperator,
            InternalCodes.InvalidAccess,
            $"Cannot assign to '{type.Widen()}[{indexType.Widen()}]' because the expression will be replaced by a macro."
        );
    }

    private void CheckArity(Arguments arguments, List<Type> argumentTypes, List<Type> parameterTypes)
    {
        var requiredParameterTypes = parameterTypes.FindAll(Type.IsNotOptional);
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
            semanticModel.TypeSolver.AddConstraint(
                argumentTypes[i],
                parameterTypes[i],
                arguments.ArgumentList[i]
            );
        }
    }

    private Type GetReturnType(FunctionDeclaration functionDeclaration)
    {
        if (functionDeclaration.ReturnType != null)
            return Visit(functionDeclaration.ReturnType);

        // TODO: flow analysis
        var possibleReturnTypes = functionDeclaration.Body is ExpressionBody body
            ? [Visit(body)]
            : functionDeclaration.Body
                .GetDescendants<Return>()
                .Where(returnStatement => returnStatement.FirstAncestorOfType<FunctionDeclaration>() == functionDeclaration)
                .Select(Visit)
                .ToList();

        return TypeSimplifier.Simplify(new Types.UnionType(possibleReturnTypes));
    }

    private Type InstantiateGenericType(Node node, TypeArguments? typeArguments, GenericType genericType)
    {
        var arguments = typeArguments?.ArgumentsList.ConvertAll(Visit) ?? [];
        if (!CheckGenericArity(typeArguments ?? node, genericType.Parameters, arguments, $"Type '{genericType}'"))
            return BindType(node, Types.PrimitiveType.Never);

        var fullArguments = new List<Type>();
        for (var i = 0; i < genericType.Parameters.Count; i++)
        {
            var typeParameter = genericType.Parameters[i];
            if (i < arguments.Count)
            {
                fullArguments.Add(arguments[i]);
            }
            else if (typeParameter.DefaultType != null)
            {
                fullArguments.Add(typeParameter.DefaultType);
            }
            else
            {
                ReportCannotInfer(typeArguments ?? node, typeParameter);
                return BindType(node, Types.PrimitiveType.Never);
            }
        }

        for (var i = 0; i < genericType.Parameters.Count; i++)
        {
            var parameter = genericType.Parameters[i];
            var argument = fullArguments[i];
            if (parameter.Constraint == null) continue;
            CheckTypeParameterConstraints(node, argument, parameter);
        }

        var instantiated = new InstantiatedType(genericType, arguments);
        return BindType(node, instantiated);
    }

    private Type BindNonGenericInvocation(Invocation invocation, List<Type> argumentTypes, Types.FunctionType functionType)
    {
        CheckArity(invocation.Arguments, argumentTypes, functionType.ParameterTypes);
        AddArgumentConstraints(invocation.Arguments, argumentTypes, functionType.ParameterTypes);
        return BindType(invocation, functionType.ReturnType);
    }

    private Dictionary<Types.TypeParameter, Type>? ResolveTypeArguments(
        Invocation invocation,
        Types.FunctionType functionType,
        List<Type> argumentTypes)
    {
        var substitution = new Dictionary<Types.TypeParameter, Type>();
        if (invocation.TypeArguments != null)
        {
            var explicitArguments = invocation.TypeArguments.ArgumentsList.ConvertAll(Visit);
            if (!CheckGenericArity(invocation, functionType.TypeParameters, explicitArguments, "Function"))
                return null;

            for (var i = 0; i < explicitArguments.Count; i++)
                substitution[functionType.TypeParameters[i]] = explicitArguments[i];
        }
        else
        {
            var inferred = InferTypeArguments(functionType, argumentTypes, invocation);
            if (inferred == null)
                return null;

            foreach (var (tp, type) in inferred)
                substitution[tp] = type;
        }

        foreach (var tp in functionType.TypeParameters)
            if (substitution.TryGetValue(tp, out var substitutedType) && tp.Constraint != null)
                CheckTypeParameterConstraints(invocation, substitutedType, tp);

        return substitution;
    }

    private Dictionary<Types.TypeParameter, Type>? InferTypeArguments(
        Types.FunctionType functionType,
        List<Type> argumentTypes,
        Node errorNode)
    {
        var inferred = new Dictionary<Types.TypeParameter, Type>();
        for (var i = 0; i < Math.Min(functionType.ParameterTypes.Count, argumentTypes.Count); i++)
        {
            var paramType = functionType.ParameterTypes[i];
            if (paramType is not Types.TypeParameter tp) continue;

            var argType = argumentTypes[i];
            if (inferred.TryGetValue(tp, out var existing))
            {
                if (existing.Equals(argType)) continue;

                _diagnostics.Error(
                    errorNode,
                    InternalCodes.InferredGenericConflict,
                    $"Inferred type '{argType}' for parameter '{tp.Name}' conflicts with previous '{existing}'."
                );
            }
            else
            {
                inferred[tp] = argType;
            }
        }

        var substitution = new Dictionary<Types.TypeParameter, Type>();
        foreach (var tp in functionType.TypeParameters)
        {
            if (inferred.TryGetValue(tp, out var inferredType))
            {
                substitution[tp] = inferredType;
            }
            else if (tp.DefaultType != null)
            {
                substitution[tp] = tp.DefaultType;
            }
            else
            {
                ReportCannotInfer(errorNode, tp);
                return null;
            }
        }

        return substitution;
    }

    private static List<Type> SubstituteTypeParameters(List<Type> types, Dictionary<Types.TypeParameter, Type> substitution) =>
        types.ConvertAll(t => SubstituteTypeParameters(t, substitution));

    private static Type SubstituteTypeParameters(Type type, Dictionary<Types.TypeParameter, Type> substitution)
    {
        if (type is Types.TypeParameter tp && substitution.TryGetValue(tp, out var substituted))
            return substituted;

        return TypeSolver.Transform(type, t => t is Types.TypeParameter tp2 && substitution.TryGetValue(tp2, out var s) ? s : t);
    }

    private void CheckTypeParameterConstraints(Node node, Type type, Types.TypeParameter parameter)
    {
        if (parameter.Constraint == null) return;
        if (type.IsAssignableTo(parameter.Constraint)) return;

        _diagnostics.Error(
            node,
            InternalCodes.ConstraintViolation,
            $"Type '{type}' does not satisfy constraint '{parameter.Constraint}' for type parameter '{parameter.Name}'."
        );
    }

    private bool CheckGenericArity(Node node, List<Types.TypeParameter> parameters, List<Type> arguments, string genericKind)
    {
        var minimum = parameters.Count(p => p.DefaultType == null);
        var maximum = parameters.Count;
        var arityDisplay = minimum == maximum ? minimum.ToString() : $"{minimum}-{maximum}";
        if (arguments.Count >= minimum && arguments.Count <= maximum)
            return true;

        _diagnostics.Error(
            node,
            InternalCodes.GenericArity,
            $"{genericKind} expects {arityDisplay} type argument{(minimum != maximum || maximum != 1 ? "s" : "")}, but {arguments.Count} were provided."
        );

        return false;
    }

    private Type GetDeclarationType(Symbol symbol) =>
        symbol is { IsGlobal: true, IsIntrinsic: false } && symbol.File.AbsolutePath != semanticModel.Tree.File.AbsolutePath
            ? Visit(symbol.Declaration)
            : semanticModel.GetType(symbol.Declaration);

    private T BindType<T>(Node node, T type)
        where T : Type
    {
        semanticModel.TypeSolver.SetType(node, type);
        return type;
    }

    private static string? FormatBinaryHint(BinaryOperator op, Type left, Type right, BinaryOperatorRule? suggestion)
    {
        if (suggestion == null)
            return null;

        var suggestedOp = SyntaxFacts.GetOperatorText(suggestion.OperatorKind);
        if (suggestion.OperatorKind != op.Operator.Kind)
            return $"did you mean '{op.Left} {suggestedOp} {op.Right}'?";

        if (!left.IsAssignableTo(suggestion.LeftType) && right.IsAssignableTo(suggestion.RightType))
            return $"left should be '{suggestion.LeftType}', not '{left}'";

        if (left.IsAssignableTo(suggestion.LeftType) && !right.IsAssignableTo(suggestion.RightType))
            return $"right should be '{suggestion.RightType}', not '{right}'";

        return $"left should be '{suggestion.LeftType}' and right should be '{suggestion.RightType}'";
    }

    private static string? FormatUnaryHint(UnaryOperator op, Type operand, UnaryOperatorRule? suggestion)
    {
        if (suggestion == null)
            return null;

        var suggestedOp = SyntaxFacts.GetOperatorText(suggestion.OperatorKind);
        return suggestion.OperatorKind != op.Operator.Kind
            ? $"did you mean '{suggestedOp}{op.Operand}'?"
            : $"operand should be '{suggestion.OperandType}', not '{operand}'";
    }
}