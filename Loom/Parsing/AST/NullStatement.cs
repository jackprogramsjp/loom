using Loom.Syntax;

namespace Loom.Parsing.AST;

public class NullStatement(Token token)
    : Statement([token], [])
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNullStatement(this);
}