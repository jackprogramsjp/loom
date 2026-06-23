using Loom.Text;

namespace Loom.Parsing.AST;

public class NullTypeExpression(Token token)
    : TypeExpression([token], [])
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNullTypeExpression(this);
}