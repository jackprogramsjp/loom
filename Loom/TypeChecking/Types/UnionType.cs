namespace Loom.TypeChecking.Types;

public class UnionType(List<Type> types) : Type
{
    public List<Type> Types { get; } = types;

    public override bool Equals(Type? other) => 
        other is UnionType union
        && union.Types.TrueForAll(t => t.Equals(Types.ElementAtOrDefault(union.Types.IndexOf(t))));

    public override bool IsAssignableTo(Type other) => base.IsAssignableTo(other) || other is UnionType && Types.Any(t => t.IsAssignableTo(other));
    
    public override string ToString() => string.Join(" | ", Types.ConvertAll(type => type.ToString()));
}