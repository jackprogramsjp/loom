using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;
using TypeName = Loom.Parsing.AST.TypeName;
using TypeParameter = Loom.Parsing.AST.TypeParameter;
using UnionType = Loom.Parsing.AST.UnionType;

namespace Loom.TypeChecking;

public class TypeChecker : Visitor<Type>
{
    public TypeSolver TypeSolver { get; }

    private readonly DiagnosticBag _diagnostics = new();
    private readonly SemanticModel _semanticModel;
    private Type? _expectedType;

    public TypeChecker(SemanticModel semanticModel)
    {
        TypeSolver = new TypeSolver(_diagnostics);
        _semanticModel = semanticModel;
    }

    public TypeCheckerResult Check()
    {
        var tree = _semanticModel.Tree;
        var type = BindType(tree, VisitTree(tree));
        TypeSolver.SolveConstraints();
        return new TypeCheckerResult(type, _diagnostics);
    }

    public override Type Visit(Node node) => node.Accept(this);

    public override Type VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        var type = base.VisitExpressionStatement(expressionStatement);
        _diagnostics.Info(expressionStatement.Span, $"Solved type '{TypeSimplifier.Simplify(type)}' for expression: {expressionStatement.Expression}");
        return BindType(expressionStatement, type);
    }

    public override Type VisitTypeAlias(TypeAlias typeAlias)
    {
        if (typeAlias.TypeParameters == null)
        {
            var type = Visit(typeAlias.EqualsTypeClause);
            return BindType(typeAlias, TypeSimplifier.Simplify(type));
        }

        var parameters = typeAlias.TypeParameters.Parameters.ConvertAll(Visit<Types.TypeParameter>);
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
                TypeSolver.AddConstraint(initializerType, declaredType, variableDeclaration.EqualsValueClause.Value.Span);

            finalType = declaredType;
        }
        else if (variableDeclaration.EqualsValueClause != null)
        {
            finalType = initializerType;
        }

        if (variableDeclaration.Keyword.Kind == SyntaxKind.MutKeyword)
            finalType = finalType.Widen();

        return BindType(variableDeclaration, finalType);
    }

    public override Type VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        var leftType = Visit(binaryOperator.Left);
        var rightType = Visit(binaryOperator.Right);
        var rule = BinaryOperatorBinder.GetRule(binaryOperator, leftType, rightType);
        if (rule != null)
            return rule.ReturnType;

        var suggestion = BinaryOperatorBinder.GetSuggestion(binaryOperator, leftType, rightType);
        var hint = FormatBinaryHint(binaryOperator, leftType, rightType, suggestion);
        _diagnostics.Error(binaryOperator, InternalCodes.InvalidUnaryOp, $"No binary operation for '{leftType} {binaryOperator.Operator.Text} {rightType}'", hint);
        return Types.PrimitiveType.Never;
    }

    public override Type VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var operandType = Visit(unaryOperator.Operand);
        var rule = UnaryOperatorBinder.GetRule(unaryOperator, operandType);
        if (rule != null)
            return rule.ReturnType;

        var suggestion = UnaryOperatorBinder.GetSuggestion(unaryOperator, operandType);
        var hint = FormatUnaryHint(unaryOperator, operandType, suggestion);
        _diagnostics.Error(unaryOperator, InternalCodes.InvalidUnaryOp, $"No unary operation for '{unaryOperator.Operator.Text}{operandType}'", hint);
        return Types.PrimitiveType.Never;
    }

    public override Type VisitLiteral(Literal literal) => BindType(literal, new LiteralType(literal.Value));

    public override Type VisitIdentifier(Identifier identifier)
    {
        var symbol = _semanticModel.GetSymbol(identifier);
        if (symbol != null)
        {
            var type = TypeSolver.GetType(symbol.DeclaringNode);
            return BindType(identifier, type);
        }

        _diagnostics.Error(identifier.Span, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of variable '{identifier.Name.Text}'.");
        return BindType(identifier, Types.PrimitiveType.Never);
    }

    public override Type VisitIntersectionType(IntersectionType intersectionType) =>
        BindType(intersectionType, new Types.IntersectionType(intersectionType.Types.ConvertAll(Visit)));

    public override Type VisitUnionType(UnionType unionType) => BindType(unionType, new Types.UnionType(unionType.Types.ConvertAll(Visit)));
    public override Type VisitPrimitiveType(PrimitiveType primitiveType) => BindType(primitiveType, new Types.PrimitiveType(primitiveType.Kind));

    public override Type VisitTypeName(TypeName typeName)
    {
        var symbol = _semanticModel.GetSymbol(typeName);
        if (symbol != null)
        {
            var declaredType = TypeSolver.GetType(symbol.DeclaringNode);
            if (typeName.TypeArguments == null)
                return BindType(typeName, declaredType);

            if (declaredType is GenericType genericType)
            {
                var arguments = typeName.TypeArguments.Arguments.ConvertAll(Visit);
                if (arguments.Count != genericType.Parameters.Count)
                {
                    _diagnostics.Error(
                        typeName.Span,
                        InternalCodes.GenericArity,
                        $"Type '{typeName.Name.Text}' expects {genericType.Parameters.Count} type argument(s), but {arguments.Count} were provided."
                    );

                    return BindType(typeName, Types.PrimitiveType.Never);
                }

                var instantiated = new InstantiatedType(genericType, arguments);
                return BindType(typeName, instantiated);
            }

            _diagnostics.Error(typeName.Span, InternalCodes.NotGeneric, $"Type '{typeName.Name.Text}' is not generic and cannot receive type arguments.");
            return BindType(typeName, Types.PrimitiveType.Never);
        }

        _diagnostics.Error(typeName.Span, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of type '{typeName.Name.Text}'.");
        return BindType(typeName, Types.PrimitiveType.Never);
    }

    public override Type VisitTypeParameter(TypeParameter typeParameter)
    {
        var defaultType = MaybeVisit(typeParameter.EqualsTypeClause);
        // var constraint = MaybeVisit(typeParameter.TypeConstraintClause);
        var parameter = new Types.TypeParameter(typeParameter.Name.Text, defaultType, null);
        return BindType(typeParameter, parameter);
    }

    private Type BindType(Node node, Type type)
    {
        TypeSolver.SetType(node, type);
        return type;
    }
    
    private static string? FormatBinaryHint(
        BinaryOperator op, Type left, Type right, BinaryOperatorRule? suggestion)
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

    private static string? FormatUnaryHint(
        UnaryOperator op, Type operand, UnaryOperatorRule? suggestion)
    {
        if (suggestion == null)
            return null;

        var suggestedOp = SyntaxFacts.GetOperatorText(suggestion.OperatorKind);
        return suggestion.OperatorKind != op.Operator.Kind 
            ? $"did you mean '{suggestedOp}{op.Operand}'?" 
            : $"operand should be '{suggestion.OperandType}', not '{operand}'";
    }

    private T Visit<T>(Node node)
        where T : Type =>
        (T)Visit(node);
}