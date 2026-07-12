namespace Loom.Core.TypeChecking.Types;

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

    public HashSet<string> TraitMethodNames { get; set; } = [];
    public ObjectIndexer? Indexer { get; } = objectType.Indexer ?? constraints.Select(c => c.Indexer).FirstOrDefault(i => i != null);
    public ObjectProperty? GetProperty(string name) => ObjectType.GetProperty(name) ?? Constraints.Select(c => c.GetProperty(name)).FirstOrDefault(p => p != null);

    public override bool Equals(Type? other) =>
        other is InterfaceType interfaceType
        && ListEquals(Constraints, interfaceType.Constraints)
        && ObjectType.Equals(interfaceType.ObjectType);

    public override bool IsAssignableTo(Type other) => AssignabilityType.IsAssignableTo(other);

    public override string ToString() => Name;
}