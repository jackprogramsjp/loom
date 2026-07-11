using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class InterfaceDeclaration(
    Token? sealedKeyword,
    Token keyword,
    Token name,
    TypeParameters? typeParameters,
    ColonTypeListClause? colonTypeListClause,
    InterfaceBody? body
)
    : GenericNamedDeclaration(
        sealedKeyword != null ? [sealedKeyword] : [],
        keyword,
        name,
        typeParameters,
        colonTypeListClause,
        body
    )
{
    public Token? SealedKeyword { get; } = sealedKeyword;
    public ColonTypeListClause? ColonTypeListClause { get; } = colonTypeListClause;
    public InterfaceBody? Body { get; } = body;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitInterfaceDeclaration(this);
}