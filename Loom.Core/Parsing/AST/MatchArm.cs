using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class MatchArm(Pattern pattern, Token arrow, Expression body)
    : Node([arrow, ..pattern.Tokens, ..body.Tokens], [pattern, body])
{
    public Pattern Pattern { get; } = pattern;
    public Token Arrow { get; } = arrow;
    public Expression Body { get; } = body;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitMatchArm(this);
}
