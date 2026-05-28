namespace Loom.Parsing.AST;

public class ExpressionStatement(Expression expression) : Statement(expression.Tokens, [expression])
{
    public Expression Expression { get; } = expression;
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitExpressionStatement(this);
}