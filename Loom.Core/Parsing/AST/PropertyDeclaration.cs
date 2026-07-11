using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class PropertyDeclaration(Token? mutKeyword, Token name, ColonTypeClause colonTypeClause)
    : InterfaceMember([mutKeyword, name, ..colonTypeClause.Tokens], [colonTypeClause])
{
    public Token? MutKeyword { get; } = mutKeyword;
    public Token Name { get; } = name;
    public ColonTypeClause ColonTypeClause { get; } = colonTypeClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitPropertyDeclaration(this);
}