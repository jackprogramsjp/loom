namespace Loom.TypeChecking.Types;

public sealed class IntersectionType(List<Type> types) : Type
{
    public List<Type> Types { get; } = types;

    public override bool IsAssignableTo(Type other) => Types.All(other.IsAssignableTo);
    public override string ToString() => string.Join(" & ", Types.ConvertAll(type => type.ToString()));
}