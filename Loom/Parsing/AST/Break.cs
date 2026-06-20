using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Break(Token keyword) : Statement([keyword], [])
{
    public Token Keyword { get; } = keyword;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitBreak(this);
}