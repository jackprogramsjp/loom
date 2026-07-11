namespace Loom.Core.Parsing.AST;

public class PropertyAccess(Expression expression, List<DotName> names) : AssignmentTarget([..expression.Tokens, ..names.SelectMany(n => n.Tokens)], [expression])
{
    public Expression Expression { get; } = expression;
    public List<DotName> Names { get; } = names;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitPropertyAccess(this);
}