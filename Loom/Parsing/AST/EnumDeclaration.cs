using Loom.Syntax;

namespace Loom.Parsing.AST;

public class EnumDeclaration(Token keyword, Token name, Token leftBrace, Token rightBrace, ColonTypeClause? colonTypeClause, List<EnumMember> members)
    : Statement([keyword, name, leftBrace, ..members.SelectMany(m => m.Tokens), rightBrace], [colonTypeClause, ..members])
{
    public Token Keyword { get; } = keyword;
    public Token Name { get; } = name;
    public Token LeftBrace { get; } = leftBrace;
    public Token RightBrace { get; } = rightBrace;
    public ColonTypeClause? ColonTypeClause { get; } = colonTypeClause;
    public List<EnumMember> Members { get; } = members;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitEnumDeclaration(this);
}