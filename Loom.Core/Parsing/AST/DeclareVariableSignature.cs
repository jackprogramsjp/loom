using Loom.Text;

namespace Loom.Parsing.AST;

public class DeclareVariableSignature(Token keyword, Token name, ColonTypeClause colonTypeClause, params Node?[] extraChildren)
    : DeclareSignature([keyword], name, [colonTypeClause, ..extraChildren])
{
    public Token Keyword { get; } = keyword;
    public ColonTypeClause ColonTypeClause { get; } = colonTypeClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitDeclareVariableSignature(this);
}