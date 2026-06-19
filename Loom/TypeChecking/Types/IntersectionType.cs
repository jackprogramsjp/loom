namespace Loom.TypeChecking.Types;

public class IntersectionType : Type
{
    public List<Type> Types { get; }

    public IntersectionType(List<Type> types)
    {
        Types = types;
    }

    public override bool Equals(Type? other) => other is IntersectionType intersection && ListEquals(Types, intersection.Types);

    public override bool IsAssignableTo(Type other) =>
        base.IsAssignableTo(other) || Types.Exists(t => t.IsAssignableTo(other));

    public override string ToString() => string.Join(" & ", Types.ConvertAll(ParenthesizeIfNeeded));
}