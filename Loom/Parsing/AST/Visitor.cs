namespace Loom.Parsing.AST;

public abstract class Visitor<T>
{
    public abstract T Visit(Node node);
    public virtual T VisitTree(Tree tree) => VisitList(tree.Statements);

    public virtual T VisitTypeAlias(TypeAlias typeAlias)
    {
        var results = new List<T>();
        if (typeAlias.TypeParameters != null)
            results.Add(Visit(typeAlias.TypeParameters));

        results.Add(Visit(typeAlias.EqualsTypeClause.Type));
        return CombineResults(results);
    }

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
    public virtual T VisitAssignmentOperator(AssignmentOperator assignmentOperator) => CombineResults([Visit(assignmentOperator.Left), Visit(assignmentOperator.Right)]);
    public virtual T VisitBinaryOperator(BinaryOperator binaryOperator) => CombineResults([Visit(binaryOperator.Left), Visit(binaryOperator.Right)]);
    public virtual T VisitUnaryOperator(UnaryOperator unaryOperator) => Visit(unaryOperator.Operand);

    public abstract T VisitLiteralType(LiteralType literalType);
    public abstract T VisitPrimitiveType(PrimitiveType primitiveType);
    public abstract T VisitTypeName(TypeName typeName);
    public virtual T VisitParenthesizedType(ParenthesizedType parenthesized) => Visit(parenthesized.Type);
    public virtual T VisitOptionalType(OptionalType optionalType) => Visit(optionalType.NonNullableType);
    public virtual T VisitUnionType(UnionType unionType) => VisitList(unionType.Types);
    public virtual T VisitIntersectionType(IntersectionType intersectionType) => VisitList(intersectionType.Types);

    public virtual T VisitTypeParameter(TypeParameter typeParameter) => typeParameter.EqualsTypeClause != null ? Visit(typeParameter.EqualsTypeClause.Type) : default!;
    public virtual T VisitTypeParameters(TypeParameters typeParameters) => VisitList(typeParameters.Parameters);
    public virtual T VisitTypeArguments(TypeArguments typeArguments) => VisitList(typeArguments.Arguments);
    public virtual T VisitColonTypeClause(ColonTypeClause colonTypeClause) => Visit(colonTypeClause.Type);
    public virtual T VisitEqualsTypeClause(EqualsTypeClause equalsTypeClause) => Visit(equalsTypeClause.Type);
    public virtual T VisitEqualsValueClause(EqualsValueClause equalsValueClause) => Visit(equalsValueClause.Value);

    public virtual T VisitNullExpression(NullExpression nullExpression) => default!;
    public virtual T VisitNullStatement(NullStatement nullStatement) => default!;
    public virtual T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;
    
    protected virtual T CombineResults(IEnumerable<T> results) => results.LastOrDefault()!;

    protected T? MaybeVisit<TNode>(TNode? node) where TNode : Node => node is null ? default : Visit(node);
    private T VisitList<TNode>(List<TNode> nodes) where TNode : Node => CombineResults(nodes.ConvertAll(Visit));
}