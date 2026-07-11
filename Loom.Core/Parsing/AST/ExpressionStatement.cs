namespace Loom.Core.Parsing.AST;

public class ExpressionStatement(Expression expression)
    : Statement(expression.Tokens, [expression])
{
    public Expression Expression { get; } = expression;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitExpressionStatement(this);
}