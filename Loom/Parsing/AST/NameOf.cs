using Loom.Syntax;

namespace Loom.Parsing.AST;

public class NameOf(Token keyword, Token leftParen, Token rightParen, Name name)
    : Expression([keyword, leftParen, ..name.Tokens, rightParen], [name])
{
    public Token Keyword { get; } = keyword;
    public Token LeftParen { get; } = leftParen;
    public Token RightParen { get; } = rightParen;
    public Name Name { get; } = name;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNameOf(this);
}