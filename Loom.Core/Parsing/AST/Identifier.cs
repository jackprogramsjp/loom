using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class Identifier(Token name)
    : Name(name)
{
    public Token Name { get; } = name;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitIdentifier(this);
}