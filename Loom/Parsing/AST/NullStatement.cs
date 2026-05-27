namespace Loom.Parsing.AST;

public class NullStatement() : Statement([])
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitNullStatement(this);
}