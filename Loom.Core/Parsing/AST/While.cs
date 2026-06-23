using Loom.Text;

namespace Loom.Parsing.AST;

public class While(Token keyword, Expression condition, Statement body)
    : Statement([keyword, ..condition.Tokens, ..body.Tokens], [condition, body])
{
    public Token Keyword { get; } = keyword;
    public Expression Condition { get; } = condition;
    public Statement Body { get; } = body;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitWhile(this);
}