namespace Loom.Parsing.AST;

public class NullStatement()
    : Statement([], [])
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNullStatement(this);
}