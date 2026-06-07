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

        var argumentTypes = invocation.Arguments.ArgumentList.ConvertAll(Visit);
        if (functionType.TypeParameters.Count == 0)
        {
            CheckArity(invocation.Arguments, argumentTypes, functionType.ParameterTypes);
            AddArgumentConstraints(invocation.Arguments, argumentTypes, functionType.ParameterTypes);
            return BindType(invocation, functionType.ReturnType);
        }

        var substitution = new Dictionary<Types.TypeParameter, Type>();
        var inferredTypes = new Dictionary<Types.TypeParameter, Type>();
        if (invocation.TypeArguments != null)
        {
            var explicitArguments = invocation.TypeArguments.ArgumentsList.ConvertAll(Visit);
            if (!CheckGenericArity(invocation.Arguments, functionType.TypeParameters, explicitArguments, "Function"))
                return BindType(invocation, Types.PrimitiveType.Never);

            for (var i = 0; i < explicitArguments.Count; i++)
                substitution[functionType.TypeParameters[i]] = explicitArguments[i];
        }
        else
        {
            for (var i = 0; i < Math.Min(functionType.ParameterTypes.Count, argumentTypes.Count); i++)
            {
                var paramType = functionType.ParameterTypes[i];
                var argType = argumentTypes[i];
                if (paramType is not Types.TypeParameter tp) continue;
                if (inferredTypes.TryGetValue(tp, out var existing))
                {
                    if (!existing.Equals(argType))
                    {
                        _diagnostics.Error(
                            invocation,
                            InternalCodes.InferredGenericConflict,
                            $"Inferred type '{argType}' for parameter '{tp.Name}' conflicts with previous '{existing}'."
                        );
                    }
                }
                else
                {
                    inferredTypes[tp] = argType;
                }
            }

            foreach (var tp in functionType.TypeParameters)
            {
                if (inferredTypes.TryGetValue(tp, out var inferred))
                {
                    substitution[tp] = inferred;
                }
                else if (tp.DefaultType != null)
                {
                    substitution[tp] = tp.DefaultType;
                }
                else
                {
                    _diagnostics.Error(
                        invocation,
                        InternalCodes.CannotInferType,
                        $"Cannot infer type parameter '{tp.Name}'. Provide explicit type arguments."
                    );

                    substitution[tp] = Types.PrimitiveType.Never;
                }
            }
        }

        foreach (var tp in functionType.TypeParameters)
        {
            if (!substitution.TryGetValue(tp, out var substitutedType) || tp.Constraint == null)
                continue;

            CheckTypeParameterConstraints(invocation, substitutedType, tp);
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

        CheckArity(invocation.Arguments, argumentTypes, substitutedParameterTypes);
        AddArgumentConstraints(invocation.Arguments, argumentTypes, substitutedParameterTypes);
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
        var constraint = MaybeVisit(typeParameter.ColonTypeClause);
        var parameter = new Types.TypeParameter(typeParameter.Name.Text, defaultType, constraint);
        return BindType(typeParameter, parameter);
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
        if (!CheckGenericArity(typeArguments, genericType.Parameters, arguments, $"Type '{genericType}'"))
            return BindType(node, Types.PrimitiveType.Never);
        
        var fullArguments = new List<Type>();
        for (var i = 0; i < genericType.Parameters.Count; i++)
        {
            var param = genericType.Parameters[i];
            if (i < arguments.Count)
            {
                fullArguments.Add(arguments[i]);
            }
            else if (param.DefaultType != null)
            {
                fullArguments.Add(param.DefaultType);
            }
            else
            {
                _diagnostics.Error(
                    node,
                    InternalCodes.MissingTypeArgument,
                    $"Missing type argument for type parameter '{param.Name}'."
                );
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