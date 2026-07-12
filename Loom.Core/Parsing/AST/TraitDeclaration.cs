using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class TraitDeclaration(Token keyword, Token name, TypeParameters? typeParameters, TraitBody body)
    : GenericNamedDeclaration([], keyword, name, typeParameters, body)
{
    public TraitBody Body { get; } = body;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTraitDeclaration(this);
}