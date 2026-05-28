namespace Loom.Parsing.AST;

public class NullExpression()
    : Expression([], [])
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNullExpression(this);
}