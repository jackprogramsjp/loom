using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Parameter(Token name, ColonTypeClause? colonTypeClause, EqualsValueClause? equalsValueClause)
    : Node([name, ..colonTypeClause?.Tokens ?? [], ..equalsValueClause?.Tokens ?? []], [colonTypeClause, equalsValueClause])
{
    public Token Name { get; } = name;
    public ColonTypeClause? ColonTypeClause { get; } = colonTypeClause;
    public EqualsValueClause? EqualsValueClause { get; } = equalsValueClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitParameter(this);
}