using Loom.Syntax;

namespace Loom.Parsing.AST;

public class LiteralType(Token token, object? value)
    : TypeExpression([token], [])
{
    public Token Token { get; } = token;
    public object? Value { get; } = value;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitLiteralType(this);
}