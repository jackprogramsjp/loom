using Loom.Text;

namespace Loom.Parsing.AST;

public class OptionalType(Token question, TypeExpression nonNullableType)
    : TypeExpression([..nonNullableType.Tokens, question], [nonNullableType])
{
    public Token Question { get; } = question;
    public TypeExpression NonNullableType { get; } = nonNullableType;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitOptionalType(this);
}