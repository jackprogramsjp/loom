namespace Loom.Parsing.AST;

public class QualifiedName(Identifier identifier, List<DotName> names)
    : Name(identifier.Name, names.SelectMany(n => n.Tokens).ToList()!, [identifier])
{
    public Identifier Identifier { get; } = identifier;
    public List<DotName> Names { get; } = names;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitQualifiedName(this);
}