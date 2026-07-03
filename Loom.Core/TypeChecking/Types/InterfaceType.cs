namespace Loom.TypeChecking.Types;

public sealed class InterfaceType(string name, List<InterfaceType> constraints, ObjectType objectType)
    : Type
{
    public string Name { get; } = name;
    public List<InterfaceType> Constraints { get; } = constraints;
    public ObjectType ObjectType { get; } = objectType;
    public Type AssignabilityType =>
        Constraints.Count > 0
            ? new IntersectionType([ObjectType, ..Constraints.Select(c => c.AssignabilityType)])
            : ObjectType;

    public override bool Equals(Type? other) =>
        other is InterfaceType interfaceType
        && ListEquals(Constraints, interfaceType.Constraints)
        && ObjectType.Equals(interfaceType.ObjectType);

    public override bool IsAssignableTo(Type other) => AssignabilityType.IsAssignableTo(other);

    public override string ToString() => Name;
}