using Loom.Syntax;

namespace Loom.Parsing.AST;

public class OptionalType(Token question, TypeExpression requiredType)
    : TypeExpression([..requiredType.Tokens, question], [requiredType])
{
    public Token Question { get; } = question;
    public TypeExpression RequiredType { get; } = requiredType;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitOptionalType(this);
}