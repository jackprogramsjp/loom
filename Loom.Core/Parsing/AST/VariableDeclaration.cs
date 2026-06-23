using Loom.Text;

namespace Loom.Parsing.AST;

public class VariableDeclaration(Token keyword, Token name, ColonTypeClause? colonTypeClause, EqualsValueClause? equalsValueClause)
    : DeclareVariableSignature(keyword, name, colonTypeClause!, equalsValueClause)
{
    public EqualsValueClause? EqualsValueClause { get; } = equalsValueClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitVariableDeclaration(this);
}