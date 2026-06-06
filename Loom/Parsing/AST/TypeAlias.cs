using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeAlias(Token keyword, Token name, TypeParameters? typeParameters, EqualsTypeClause equalsTypeClause)
    : GenericNamedDeclaration(keyword, name, typeParameters, equalsTypeClause)
{
    public EqualsTypeClause EqualsTypeClause { get; } = equalsTypeClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeAlias(this);
}