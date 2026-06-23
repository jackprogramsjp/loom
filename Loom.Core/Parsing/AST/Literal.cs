using Loom.Text;

namespace Loom.Parsing.AST;

public class Literal(Token token, object? value)
    : Expression([token], [])
{
    public Token Token { get; } = token;
    public object? Value { get; } = value;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitLiteral(this);
}