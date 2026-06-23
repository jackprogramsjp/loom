using Loom.Text;

namespace Loom.Parsing.AST;

public class ExpressionBody(Token arrow, Expression expression)
    : Statement([arrow, ..expression.Tokens], [expression])
{
    public Token Arrow { get; } = arrow;
    public Expression Expression { get; } = expression;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitExpressionBody(this);
}