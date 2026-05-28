using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeName(Token name)
    : TypeExpression([name], [])
{
    public Token Name { get; } = name;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeName(this);
}