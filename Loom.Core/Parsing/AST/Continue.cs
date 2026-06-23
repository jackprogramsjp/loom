using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Continue(Token keyword) : Statement([keyword], [])
{
    public Token Keyword { get; } = keyword;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitContinue(this);
}