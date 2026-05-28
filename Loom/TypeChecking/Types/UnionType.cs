namespace Loom.TypeChecking.Types;

public class UnionType(List<Type> types) : Type
{
    public List<Type> Types { get; } = types;

    public override bool IsAssignableTo(Type other) => Types.Any(other.IsAssignableTo);
    public override string ToString() => string.Join(" | ", Types.ConvertAll(type => type.ToString()));
}