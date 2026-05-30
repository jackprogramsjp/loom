namespace Loom.TypeChecking.Types;

public class TypeEqualityComparer : IEqualityComparer<Type>
{
    public static readonly TypeEqualityComparer Default = new();
    
    public bool Equals(Type? x, Type? y) => x?.Equals(y) ?? y == null;

    public int GetHashCode(Type obj) => 0;
}

public abstract class Type
{
    public abstract bool Equals(Type? other);
    
    public Type Widen() =>
        this switch
        {
            LiteralType literal => new PrimitiveType(literal.Kind),
            _ => this
        };

    public virtual bool IsAssignableTo(Type other)
    {
        return other switch
        {
            UnionType union => union.Types.Any(IsAssignableTo),
            IntersectionType intersection => intersection.IsAssignableTo(this),
            _ => false
        };
    }
}