using Loom.Syntax;

namespace Loom.Parsing.AST;

public class IntersectionType(List<Token> ampersands, List<TypeExpression> types)
    : TypeExpression([..ampersands, ..types.SelectMany(t => t.Tokens)], types)
{
    public List<Token> Ampersands { get; } = ampersands;
    public List<TypeExpression> Types { get; } = types;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitIntersectionType(this);
}