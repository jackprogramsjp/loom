using Loom.Text;

namespace Loom.Parsing.AST;

public class After(Token keyword, Expression duration, Statement body)
    : Statement([keyword, ..duration.Tokens, ..body.Tokens], [duration, body])
{
    public Token Keyword { get; } = keyword;
    public Expression Duration { get; } = duration;
    public Statement Body { get; } = body;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitAfter(this);
}