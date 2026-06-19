namespace Loom.TypeChecking.Types;

public class InterfaceType(string name, List<InterfaceType> constraints, ObjectType objectType)
    : Type
{
    public string Name { get; } = name;
    public List<InterfaceType> Constraints { get; } = constraints;
    public ObjectType ObjectType { get; } = objectType;
    public Type AssignabilityType { get; } = constraints.Count > 0 ? new IntersectionType([objectType, ..constraints]) : objectType;

    public override bool Equals(Type? other) =>
        other is InterfaceType interfaceType
        && ListEquals(Constraints, interfaceType.Constraints)
        && ObjectType.Equals(interfaceType.ObjectType);

    public override bool IsAssignableTo(Type other) => AssignabilityType.IsAssignableTo(other);

    public override string ToString() => Name;
}