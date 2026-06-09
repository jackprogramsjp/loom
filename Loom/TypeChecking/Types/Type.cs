namespace Loom.TypeChecking.Types;

public abstract class Type
{
    public abstract bool Equals(Type? other);
    public abstract override string ToString();

    public static bool IsNotNever(Type type) => !IsNever(type);
    public static bool IsNever(Type type) => type is PrimitiveType { Kind: PrimitiveTypeKind.Never };
    public static bool IsDefined(Type type) => !IsNone(type);
    public static bool IsNone(Type type) => type is PrimitiveType { Kind: PrimitiveTypeKind.Void or PrimitiveTypeKind.None };
    public static bool IsNotOptional(Type type) => !IsOptional(type);
    public static bool IsOptional(Type type) => IsNone(type) || type is OptionalType || type is UnionType union && union.Types.Any(t => IsNone(t) || IsOptional(t));
    protected static bool RequiresParentheses(Type type) => type is UnionType or IntersectionType or FunctionType;
    
    public Type NonNullable() =>
        IsNone(this)
            ? PrimitiveType.Never
            : this is OptionalType optional
                ? optional.NonNullableType.NonNullable()
                : IsOptional(this)
                    ? TypeSimplifier.Simplify(this).NonNullable()
                    : this;

    public virtual Type Widen() => this;

    public virtual bool IsAssignableTo(Type other)
    {
        return other switch
        {
            UnionType union => union.Types.Any(IsAssignableTo),
            IntersectionType intersection => intersection.Types.All(IsAssignableTo),
            _ => false
        };
    }
    
    protected static bool ListEquals<T>(List<T> list, List<T> otherList) where T : Type =>
        list.Count == otherList.Count
        && list.All(t => otherList.Any(u => u.Equals(t)));
}