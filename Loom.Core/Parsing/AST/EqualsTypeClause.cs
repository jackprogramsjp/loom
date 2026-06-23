using Loom.Text;

namespace Loom.Parsing.AST;

public class EqualsTypeClause(Token equalsToken, TypeExpression type)
    : Node([equalsToken, ..type.Tokens], [type])
{
    public Token EqualsToken { get; } = equalsToken;
    public TypeExpression Type { get; } = type;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitEqualsTypeClause(this);
}