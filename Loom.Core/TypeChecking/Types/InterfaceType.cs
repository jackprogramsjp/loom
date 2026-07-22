namespace Loom.Core.TypeChecking.Types;

public sealed class InterfaceType(
    string name,
    List<InterfaceType> constraints,
    ObjectType objectType,
    HashSet<string>? traitMethodNames = null
) : NativelyIndexableType
{
    public string Name { get; } = name;
    public List<InterfaceType> Constraints { get; } = constraints;
    public ObjectType ObjectType { get; } = objectType;
    public Type AssignabilityType =>
        Constraints.Count > 0
            ? new IntersectionType([ObjectType, ..Constraints.Select(c => c.AssignabilityType)])
            : ObjectType;

    public HashSet<string> TraitMethodNames { get; set; } = traitMethodNames ?? [];
    public override ObjectIndexer? Indexer
    {
        get => ObjectType.Indexer ?? Constraints.Select(c => c.Indexer).FirstOrDefault(i => i != null);
        internal set => throw new NotImplementedException();
    }
    public override List<ObjectProperty> Properties => [..ObjectType.Properties, ..Constraints.SelectMany(c => c.Properties)];

    public override Type PropertyKeyUnion()
    {
        var baseType = ObjectType.PropertyKeyUnion();
        var constraintTypes = Constraints.Select(constraint => constraint.ObjectType.PropertyKeyUnion());
        var unionTypes = new List<Type>([baseType, ..constraintTypes]);
        return TypeSimplifier.Simplify(new UnionType(unionTypes));
    }

    public override bool Equals(Type? other) =>
        GuardedEquals(
            this,
            other,
            () => other is InterfaceType interfaceType
                && ListEquals(Constraints, interfaceType.Constraints)
                && ObjectType.Equals(interfaceType.ObjectType)
        );

    public override int GetHashCode() => HashCode.Combine(Name, Constraints.Count, ObjectType.GetHashCode());
    public override bool IsAssignableTo(Type other) => AssignabilityType.IsAssignableTo(other);
    public override string ToString() => Name;

    internal bool MatchOrMatchConstraint(Predicate<InterfaceType> predicate) => predicate(this) || Constraints.Any(c => c.MatchOrMatchConstraint(predicate));
}