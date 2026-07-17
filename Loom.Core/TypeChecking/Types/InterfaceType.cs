namespace Loom.Core.TypeChecking.Types;

public sealed class InterfaceType(string name, List<InterfaceType> constraints, ObjectType objectType, HashSet<string>? traitMethodNames = null)
    : Type
{
    public string Name { get; } = name;
    public List<InterfaceType> Constraints { get; } = constraints;
    public ObjectType ObjectType { get; } = objectType;
    public Type AssignabilityType =>
        Constraints.Count > 0
            ? new IntersectionType([ObjectType, ..Constraints.Select(c => c.AssignabilityType)])
            : ObjectType;

    public HashSet<string> TraitMethodNames { get; set; } = traitMethodNames ?? [];
    public ObjectIndexer? Indexer => ObjectType.Indexer ?? Constraints.Select(c => c.Indexer).FirstOrDefault(i => i != null);
    public ObjectProperty? GetProperty(string name) => ObjectType.GetProperty(name) ?? Constraints.Select(c => c.GetProperty(name)).FirstOrDefault(p => p != null);

    public override int GetHashCode() => HashCode.Combine(Name, Constraints.Count, ObjectType.GetHashCode());
    
    public override bool Equals(Type? other)
    {
        if (ReferenceEquals(this, other)) return true;
        return other is InterfaceType interfaceType
            && ListEquals(Constraints, interfaceType.Constraints)
            && ObjectType.Equals(interfaceType.ObjectType);
    }

    public override bool IsAssignableTo(Type other) => AssignabilityType.IsAssignableTo(other);

    public override string ToString() => Name;
}