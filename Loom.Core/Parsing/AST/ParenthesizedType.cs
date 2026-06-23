using Loom.Text;

namespace Loom.Parsing.AST;

public class ParenthesizedType(Token leftParen, Token rightParen, TypeExpression type)
    : TypeExpression([leftParen, ..type.Tokens, rightParen], [type])
{
    public Token LeftParen { get; } = leftParen;
    public Token RightParen { get; } = rightParen;
    public TypeExpression Type { get; } = type;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitParenthesizedType(this);
}