namespace Loom.Core.Parsing.AST;

public class Attribute(Expression expression, TypeArguments? typeArguments, Arguments? arguments)
    : Invocation(expression, typeArguments, arguments ?? new Arguments(expression.Tokens[^1], expression.Tokens[^1], []))
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitAttribute(this);
}