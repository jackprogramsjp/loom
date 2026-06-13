using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Parameter(Token name, ColonTypeClause? colonTypeClause, EqualsValueClause? equalsValueClause)
    : NamedDeclaration([..colonTypeClause?.Tokens ?? [], ..equalsValueClause?.Tokens ?? []], name, colonTypeClause, equalsValueClause)
{
    public ColonTypeClause? ColonTypeClause { get; } = colonTypeClause;
    public EqualsValueClause? EqualsValueClause { get; } = equalsValueClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitParameter(this);
}