using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Literal(Token token)
    : Expression([token], [])
{
    public Token Token { get; } = token;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitLiteral(this);
}