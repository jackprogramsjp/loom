namespace Loom.TypeChecking.Types;

public class InterfaceType(string name, List<InterfaceType> constraints, ObjectType objectType)
    : Type
{
    private readonly IntersectionType _assignabilityType = new([objectType, ..constraints]);

    public string Name { get; } = name;
    public List<InterfaceType> Constraints { get; } = constraints;
    public ObjectType ObjectType { get; } = objectType;

    public override bool Equals(Type? other) =>
        other is InterfaceType interfaceType
        && Name == interfaceType.Name
        && ListEquals(Constraints, interfaceType.Constraints)
        && ObjectType.Equals(interfaceType.ObjectType);

    public override bool IsAssignableTo(Type other) => base.IsAssignableTo(other) || _assignabilityType.IsAssignableTo(other);

    public override string ToString() => Name;
}