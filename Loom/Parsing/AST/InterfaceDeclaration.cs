using Loom.Syntax;

namespace Loom.Parsing.AST;

public class InterfaceDeclaration(
    Token keyword,
    Token name,
    TypeParameters? typeParameters,
    ColonTypeListClause? colonTypeListClause,
    Token leftBrace,
    Token rightBrace,
    List<InterfaceMember> members
)
    : GenericNamedDeclaration([leftBrace, rightBrace], keyword, name, typeParameters, [colonTypeListClause, ..members])
{
    public ColonTypeListClause? ColonTypeListClause { get; } = colonTypeListClause;
    public Token LeftBrace { get; } = leftBrace;
    public Token RightBrace { get; } = rightBrace;
    public List<InterfaceMember> Members { get; } = members;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitInterfaceDeclaration(this);
}