using Loom.Parsing.AST.Traversal;

namespace Loom.Parsing.AST;

public class NullExpression() : Expression([])
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitNullExpression(this);
}