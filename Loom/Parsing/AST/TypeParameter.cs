using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeParameter(Token name, ColonTypeClause? colonTypeClause, EqualsTypeClause? equalsTypeClause)
    : Node([name, ..colonTypeClause?.Tokens ?? [], ..equalsTypeClause?.Tokens ?? []], [colonTypeClause, equalsTypeClause])
{
    public Token Name { get; } = name;
    public ColonTypeClause? ColonTypeClause { get; } = colonTypeClause;
    public EqualsTypeClause? EqualsTypeClause { get; } = equalsTypeClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeParameter(this);
}