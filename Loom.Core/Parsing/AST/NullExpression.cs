using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class NullExpression(Token token)
    : Expression([token], [])
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitNullExpression(this);
}