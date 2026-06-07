using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using LiteralType = Loom.Parsing.AST.LiteralType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;
using TypeName = Loom.Parsing.AST.TypeName;
using TypeParameter = Loom.Parsing.AST.TypeParameter;
using UnionType = Loom.Parsing.AST.UnionType;

namespace Loom.TypeChecking;

public class TypeChecker(SemanticModel semanticModel) : Visitor<Type>
{
    private readonly DiagnosticBag _diagnostics = new();
    private Type? _expectedType;

    public TypeCheckerResult Check()
    {
        var tree = semanticModel.Tree;
        var type = BindType(tree, VisitTree(tree));
        semanticModel.TypeSolver.SolveConstraints();

        var diagnostics = DiagnosticBag.Concat([semanticModel.TypeSolver.Diagnostics, _diagnostics]);
        return new TypeCheckerResult(type, diagnostics);
    }

    public override Type Visit(Node node) => node.Accept(this);

    public override Type VisitTree(Tree tree)
    {
        base.VisitTree(tree);
        return tree.Statements.Count > 0
            ? semanticModel.GetType(tree.Statements.Last())
            : Types.PrimitiveType.Never;
    }

    public override Type VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        var type = base.VisitExpressionStatement(expressionStatement);
        _diagnostics.Info(expressionStatement, $"Solved type '{TypeSimplifier.Simplify(type)}' for expression");
        return BindType(expressionStatement, type);
    }

    public override Type VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var typeParameters = functionDeclaration.TypeParameters?.ParameterList.ConvertAll(Visit<Types.TypeParameter>) ?? [];
        var parameterTypes = functionDeclaration.Parameters?.ParameterList.ConvertAll(Visit) ?? [];
        var returnType = GetReturnType(functionDeclaration);
        var functionType = new FunctionType(typeParameters, parameterTypes, returnType);

        _diagnostics.Info(functionDeclaration, $"Solved type '{TypeSimplifier.Simplify(functionType)}' for function");
        return BindType(functionDeclaration, functionType);
    }

    public override Type VisitTypeAlias(TypeAlias typeAlias)
    {
        if (typeAlias.TypeParameters == null)
        {
            var type = Visit(typeAlias.EqualsTypeClause);
            return BindType(typeAlias, TypeSimplifier.Simplify(type));
        }

        var parameters = typeAlias.TypeParameters.ParameterList.ConvertAll(Visit<Types.TypeParameter>);
        var underlyingType = Visit(typeAlias.EqualsTypeClause);
        var genericType = new GenericType(typeAlias, parameters, underlyingType);
        return BindType(typeAlias, genericType);
    }

    public override Type VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        Type? declaredType = null;
        if (variableDeclaration.ColonTypeClause != null)
            declaredType = Visit(variableDeclaration.ColonTypeClause);

        var initializerType = declaredType ?? Types.PrimitiveType.Unknown;
        if (variableDeclaration.EqualsValueClause != null)
        {
            var previousExpected = _expectedType;
            _expectedType = declaredType;
            initializerType = Visit(variableDeclaration.EqualsValueClause);
            _expectedType = previousExpected;
        }

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

    public override Type VisitParameter(Parameter parameter)
    {
        var declaredType = MaybeVisit(parameter.ColonTypeClause);
        var initializerType = MaybeVisit(parameter.EqualsValueClause);
        if (initializerType != null && parameter.EqualsValueClause != null)
            semanticModel.TypeSolver.AddConstraint(initializerType, declaredType!, parameter.EqualsValueClause.Value);

        return BindType(parameter, declaredType ?? initializerType!);
    }

    public override Type VisitInvocation(Invocation invocation)
    {
        var type = Visit(invocation.Expression);
        if (type is not FunctionType functionType)
        {
            _diagnostics.Error(invocation, InternalCodes.InvalidInvocation, $"Cannot call value of type '{type}'");
            return BindType(invocation, Types.PrimitiveType.Never);
        }

        if (functionType.TypeParameters.Count == 0)
            return functionType.ReturnType;

        var argumentTypes = invocation.Arguments.ArgumentList.ConvertAll(Visit);
        var substitution = new Dictionary<Types.TypeParameter, Type>();
        if (invocation.TypeArguments != null)
        {
            var explicitArgs = invocation.TypeArguments.ArgumentsList.ConvertAll(Visit);
            if (explicitArgs.Count != functionType.TypeParameters.Count)
            {
                _diagnostics.Error(
                    invocation,
                    InternalCodes.GenericArity,
                    $"Function expects {functionType.TypeParameters.Count} type argument(s), but {explicitArgs.Count} were provided."
                );

                return BindType(invocation, Types.PrimitiveType.Never);
            }

            for (var i = 0; i < explicitArgs.Count; i++)
                substitution[functionType.TypeParameters[i]] = explicitArgs[i];
        }
        else
        {
            for (var i = 0; i < Math.Min(functionType.ParameterTypes.Count, argumentTypes.Count); i++)
            {
                var paramType = functionType.ParameterTypes[i];
                var argType = argumentTypes[i];
                if (paramType is not Types.TypeParameter tp) continue;

                if (substitution.TryGetValue(tp, out var existing))
                {
                    if (!existing.Equals(argType))
                    {
                        _diagnostics.Warn(
                            invocation,
                            InternalCodes.TypeMismatch,
                            $"Inferred type '{argType}' for parameter '{tp.Name}' conflicts with previous '{existing}'."
                        );
                    }
                }
                else
                {
                    substitution[tp] = argType;
                }

                // TODO: nested type parameters, recursive inference pass
            }

            foreach (var tp in functionType.TypeParameters.Where(tp => !substitution.ContainsKey(tp)))
            {
                substitution[tp] = Types.PrimitiveType.Never;
            }
        }

        var substitutedParameterTypes = functionType.ParameterTypes
            .Select(solveTypeParameters)
            .ToList();

        var substitutedReturnType = solveTypeParameters(functionType.ReturnType);
        var instantiated = new FunctionType(
            typeParameters: [],
            parameterTypes: substitutedParameterTypes,
            returnType: substitutedReturnType
        );

        for (var i = 0; i < Math.Min(argumentTypes.Count, substitutedParameterTypes.Count); i++)
        {
            semanticModel.TypeSolver.AddConstraint(
                argumentTypes[i],
                substitutedParameterTypes[i],
                invocation.Arguments.ArgumentList[i]
            );
        }

        return BindType(invocation, instantiated.ReturnType);

        Type solveTypeParameters(Type forType) =>
            canSubstitute(forType, out var subst)
                ? subst
                : TypeSolver.Transform(forType, substituteTypeParameter);

        Type substituteTypeParameter(Type t) => canSubstitute(t, out var subst) ? subst : t;

        bool canSubstitute(Type t, [MaybeNullWhen(false)] out Type subst)
        {
            subst = null;
            return t is Types.TypeParameter tp && substitution.TryGetValue(tp, out subst);
        }
    }

    public override Type VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        var targetType = Visit(assignmentOperator.Left);
        var valueType = Visit(assignmentOperator.Right);
        if (assignmentOperator.Operator.Kind != SyntaxKind.Equals)
            return base.VisitBinaryOperator(assignmentOperator);

        semanticModel.TypeSolver.AddConstraint(valueType, targetType, assignmentOperator.Right);
        return BindType(assignmentOperator, targetType);
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
            return rule.ReturnType;
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

            return TypeSimplifier.Simplify(new Types.UnionType([leftType, rightType]).NonNullable());
        }

        var suggestion = BinaryOperatorBinder.GetSuggestion(binaryOperator, leftType, rightType);
        var hint = FormatBinaryHint(binaryOperator, leftType, rightType, suggestion);
        _diagnostics.Error(
            binaryOperator,
            InternalCodes.InvalidBinaryOp,
            $"No binary operation for {leftType.Widen()} {binaryOperator.Operator.Text} {rightType.Widen()}",
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
        _diagnostics.Error(unaryOperator, InternalCodes.InvalidUnaryOp, $"No unary operation for '{unaryOperator.Operator.Text}{operandType.Widen()}'", hint);

        return BindType(unaryOperator, Types.PrimitiveType.Never);
    }

    public override Type VisitLiteral(Literal literal) => BindType(literal, new Types.LiteralType(literal.Value));

    public override Type VisitIdentifier(Identifier identifier)
    {
        var symbol = semanticModel.GetSymbol(identifier);
        if (symbol != null)
        {
            var type = semanticModel.GetType(symbol.Declaration);
            return BindType(identifier, type);
        }

        _diagnostics.Error(identifier, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of variable '{identifier.Name.Text}'.");
        return BindType(identifier, Types.PrimitiveType.Never);
    }

    public override Type VisitIntersectionType(IntersectionType intersectionType) =>
        BindType(intersectionType, new Types.IntersectionType(intersectionType.Types.ConvertAll(Visit)));


    public override Type VisitUnionType(UnionType unionType) => BindType(unionType, new Types.UnionType(unionType.Types.ConvertAll(Visit)));
    public override Type VisitOptionalType(OptionalType optionalType) => new Types.OptionalType(Visit(optionalType.NonNullableType));
    public override Type VisitPrimitiveType(PrimitiveType primitiveType) => BindType(primitiveType, new Types.PrimitiveType(primitiveType.Kind));
    public override Type VisitLiteralType(LiteralType literalType) => BindType(literalType, new Types.LiteralType(literalType.Value));

    public override Type VisitTypeName(TypeName typeName)
    {
        var symbol = semanticModel.GetSymbol(typeName);
        if (symbol != null)
        {
            var declaredType = semanticModel.GetType(symbol.Declaration);
            if (typeName.TypeArguments == null)
                return BindType(typeName, declaredType);

            if (declaredType is GenericType genericType)
                return InstantiateGenericType(typeName, typeName.TypeArguments, genericType);

            _diagnostics.Error(typeName, InternalCodes.NotGeneric, $"Type '{typeName.Name.Text}' is not generic and cannot receive type arguments.");
            return BindType(typeName, Types.PrimitiveType.Never);
        }

        _diagnostics.Error(typeName, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of type '{typeName.Name.Text}'.");
        return BindType(typeName, Types.PrimitiveType.Never);
    }

    public override Type VisitTypeParameter(TypeParameter typeParameter)
    {
        var defaultType = MaybeVisit(typeParameter.EqualsTypeClause);

        // var constraint = MaybeVisit(typeParameter.TypeConstraintClause);
        var parameter = new Types.TypeParameter(typeParameter.Name.Text, defaultType, null);
        return BindType(typeParameter, parameter);
    }

    private Type GetReturnType(FunctionDeclaration functionDeclaration)
    {
        if (functionDeclaration.ReturnType != null)
            return Visit(functionDeclaration.ReturnType);

        // TODO: flow analysis
        var possibleReturnTypes = functionDeclaration.Body is ExpressionBody body
            ? [Visit(body)]
            : functionDeclaration.Body.Children.FindAll(n => n is not FunctionDeclaration)
                .SelectMany(n => n.Children)
                .Where(n => n is Return)
                .Cast<Return>()
                .Select(Visit)
                .ToList();

        return TypeSimplifier.Simplify(new Types.UnionType(possibleReturnTypes));
    }

    private Type InstantiateGenericType(Node node, TypeArguments typeArguments, GenericType genericType)
    {
        var arguments = typeArguments.ArgumentsList.ConvertAll(Visit);
        var minimumParameterCount = genericType.Parameters.Count(p => p.DefaultType == null);
        var maximumParameterCount = genericType.Parameters.Count;
        var arityDisplay = minimumParameterCount == maximumParameterCount ? minimumParameterCount.ToString() : $"{minimumParameterCount}-{maximumParameterCount}";
        if (arguments.Count != minimumParameterCount)
        {
            _diagnostics.Error(
                node,
                InternalCodes.GenericArity,
                $"Type '{(node is FunctionDeclaration ? genericType.Underlying : genericType)}' expects {arityDisplay} type argument{(maximumParameterCount > 1 ? "s" : "")}, but {arguments.Count} were provided."
            );

            return BindType(node, Types.PrimitiveType.Never);
        }

        var instantiated = new InstantiatedType(genericType, arguments);
        return BindType(node, instantiated);
    }

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