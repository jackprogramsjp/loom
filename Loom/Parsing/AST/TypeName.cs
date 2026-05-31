using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeName(Token name, TypeArguments? typeArguments = null)
    : TypeExpression([name], [])
{
    public Token Name { get; } = name;
    public TypeArguments? TypeArguments { get; } = typeArguments;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeName(this);
}