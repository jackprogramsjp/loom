namespace Loom.Parsing.AST;

public abstract class Visitor<T>
{
    public abstract T Visit(Node node);
    public T VisitTree(Tree tree) => VisitList(tree.Statements);

    public virtual T VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var results = new List<T>();
        if (variableDeclaration.ColonTypeClause != null)
            results.Add(Visit(variableDeclaration.ColonTypeClause));

        if (variableDeclaration.EqualsValueClause != null)
            results.Add(Visit(variableDeclaration.EqualsValueClause));

        return CombineResults(results);
    }

    public virtual T VisitExpressionStatement(ExpressionStatement expressionStatement) => Visit(expressionStatement.Expression);

    public abstract T VisitLiteral(Literal literal);
    public abstract T VisitIdentifier(Identifier identifier);
    public virtual T VisitParenthesized(Parenthesized parenthesized) => Visit(parenthesized.Expression);
    public virtual T VisitBinaryOperator(BinaryOperator binaryOperator) => CombineResults([Visit(binaryOperator.Left), Visit(binaryOperator.Right)]);
    public virtual T VisitUnaryOperator(UnaryOperator unaryOperator) => Visit(unaryOperator.Operand);

    public abstract T VisitTypeName(TypeName typeName);
    public abstract T VisitPrimitiveType(PrimitiveType primitiveType);
    public virtual T VisitOptionalType(OptionalType optionalType) => Visit(optionalType.NonNullableType);

    public virtual T VisitColonTypeClause(ColonTypeClause colonTypeClause) => Visit(colonTypeClause.Type);
    public virtual T VisitEqualsValueClause(EqualsValueClause equalsValueClause) => Visit(equalsValueClause.Value);

    public virtual T VisitNullExpression(NullExpression nullExpression) => default!;
    public virtual T VisitNullStatement(NullStatement nullStatement) => default!;
    public virtual T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;
    
    protected virtual T CombineResults(IEnumerable<T> results) => results.LastOrDefault()!;
    private T VisitList(List<Node> nodes) => CombineResults(nodes.ConvertAll(Visit));
}