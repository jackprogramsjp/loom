using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class NullTypeExpression(Token token)
    : TypeExpression([token], [])
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNullTypeExpression(this);
}