using Loom.Syntax;

namespace Loom.Parsing.AST;

public class VariableDeclaration(Token keyword, Token name, ColonTypeClause? colonTypeClause, EqualsValueClause? equalsValueClause)
    : NamedDeclaration([keyword], name, colonTypeClause, equalsValueClause)
{
    public Token Keyword { get; } = keyword;
    public ColonTypeClause? ColonTypeClause { get; } = colonTypeClause;
    public EqualsValueClause? EqualsValueClause { get; } = equalsValueClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitVariableDeclaration(this);
}