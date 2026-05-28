namespace Loom.Parsing.AST;

public class NullTypeExpression()
    : TypeExpression([], [])
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNullTypeExpression(this);
}