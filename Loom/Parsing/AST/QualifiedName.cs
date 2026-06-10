namespace Loom.Parsing.AST;

public class QualifiedName(Identifier identifier, List<DotName> names)
    : AssignmentTarget([..identifier.Tokens, ..names.SelectMany(n => n.Tokens)], [identifier])
{
    public Identifier Identifier { get; } = identifier;
    public List<DotName> Names { get; } = names;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitQualifiedName(this);
}