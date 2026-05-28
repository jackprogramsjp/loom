namespace Loom.Parsing.AST;

public class NullTypeExpression() : TypeExpression([], [])
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitNullTypeExpression(this);
}