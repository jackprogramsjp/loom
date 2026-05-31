namespace Loom.TypeChecking.Types;

public abstract class Type
{
    public abstract bool Equals(Type? other);
    public abstract override string ToString();

    public static bool IsNotNever(Type type) => !IsNever(type);
    public static bool IsNever(Type type) => type is PrimitiveType { Kind: PrimitiveTypeKind.Never };
    public static bool IsDefined(Type type) => !IsNone(type);
    public static bool IsNone(Type type) => type is PrimitiveType { Kind: PrimitiveTypeKind.Void or PrimitiveTypeKind.None };
    
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
            IntersectionType intersection => intersection.Types.All(IsAssignableTo),
            _ => false
        };
    }
}