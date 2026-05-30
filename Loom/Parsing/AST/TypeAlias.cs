using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeAlias(Token keyword, Token name, Token equals, TypeExpression type)
    : Statement([keyword, name, equals, ..type.Tokens], [type])
{
    public Token Keyword { get; } = keyword;
    public Token Name { get; } = name;
    public TypeExpression Type { get; } = type;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeAlias(this);
}