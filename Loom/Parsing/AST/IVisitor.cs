namespace Loom.Parsing.AST;

public interface IVisitor<out T>
{
    T VisitTree(Tree tree);
    T VisitLiteral(Literal literal);
    T VisitExpressionStatement(ExpressionStatement expressionStatement);
    T Visit(ASTNode node);
}