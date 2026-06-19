using Loom.Syntax;

namespace Loom.Parsing.AST;

public class InterfaceBody(Token leftBrace, Token rightBrace, List<InterfaceMember> members)
    : Statement([leftBrace, rightBrace, ..members.SelectMany(m => m.Tokens)], members)
{
    public Token LeftBrace { get; } = leftBrace;
    public Token RightBrace { get; } = rightBrace;
    public List<InterfaceMember> Members { get; } = members;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitInterfaceBody(this);
}