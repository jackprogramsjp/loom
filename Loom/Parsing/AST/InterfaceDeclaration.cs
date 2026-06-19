using Loom.Syntax;

namespace Loom.Parsing.AST;

public class InterfaceDeclaration(
    Token keyword,
    Token name,
    TypeParameters? typeParameters,
    ColonTypeListClause? colonTypeListClause,
    InterfaceBody? body
)
    : GenericNamedDeclaration(
        [],
        keyword,
        name,
        typeParameters,
        colonTypeListClause,
        body
    )
{
    public ColonTypeListClause? ColonTypeListClause { get; } = colonTypeListClause;
    public InterfaceBody? Body { get; } = body;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitInterfaceDeclaration(this);
}