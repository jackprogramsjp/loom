namespace Loom.TypeChecking.Types;

public sealed class IntersectionType(List<Type> types) : Type
{
    public List<Type> Types { get; } = types;

    public override bool Equals(Type? other) =>
        other is IntersectionType intersection
        && Types.Count == intersection.Types.Count
        && Types.All(t => intersection.Types.Any(u => u.Equals(t)));

    public override bool IsAssignableTo(Type other) =>
        other is LiteralType
            ? Types.Any(t => t.IsAssignableTo(other))
            : Types.All(other.IsAssignableTo);
    
    public override string ToString() => string.Join(" & ", Types.ConvertAll(type => type.ToString()));
}