using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class VariableDeclaration(Token keyword, Token name, ColonTypeClause? colonTypeClause, EqualsValueClause? equalsValueClause)
    : Statement([keyword, name, ..colonTypeClause?.Tokens ?? [], ..equalsValueClause?.Tokens ?? []], [colonTypeClause?.Type, equalsValueClause?.Value])
{
    public Token Keyword { get; } = keyword;
    public Token Name { get; } = name;
    public ColonTypeClause? ColonTypeClause { get; } = colonTypeClause;
    public EqualsValueClause? EqualsValueClause { get; } = equalsValueClause;

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitVariableDeclaration(this);
}