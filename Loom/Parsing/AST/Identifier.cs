using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Identifier(Token name)
    : Expression([name], [])
{
    public Token Name { get; } = name;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitIdentifier(this);
}