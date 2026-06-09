namespace Loom.TypeChecking.Types;

public sealed class IntersectionType(List<Type> types) : Type
{
    public List<Type> Types { get; } = types;

    public override bool Equals(Type? other) => other is IntersectionType intersection && ListEquals(Types, intersection.Types);

    public override bool IsAssignableTo(Type other) =>
        other is LiteralType
            ? Types.Exists(t => t.IsAssignableTo(other))
            : Types.TrueForAll(other.IsAssignableTo);

    public override string ToString() => string.Join(" & ", Types.ConvertAll(ParenthesizeIfNeeded));
}