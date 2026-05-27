namespace Loom.Parsing.AST.Traversal;

public interface IVisitor<out T>
{
    T VisitTree(Tree tree);
    T VisitVariableDeclaration(VariableDeclaration variableDeclaration);
    T VisitExpressionStatement(ExpressionStatement expressionStatement);
    
    T VisitLiteral(Literal literal);
    T VisitIdentifier(Identifier identifier);
    T VisitParenthesized(Parenthesized parenthesized);
    T VisitBinaryOperator(BinaryOperator binaryOperator);
    T VisitUnaryOperator(UnaryOperator unaryOperator);
    
    T VisitColonTypeClause(ColonTypeClause colonTypeClause);
    T VisitEqualsValueClause(EqualsValueClause equalsValueClause);
    
    T VisitNullExpression(NullExpression nullExpression) => default!;
    T VisitNullStatement(NullStatement nullStatement) => default!;
    T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;
    T Visit(ASTNode node);
}