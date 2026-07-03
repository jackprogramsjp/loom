namespace Loom.TypeChecking.Types;

public abstract class Type : IEquatable<Type>
{
    public abstract bool Equals(Type? other);
    public abstract override string ToString();

#pragma warning disable CS0659
    public override bool Equals(object? obj) => obj is Type type && Equals(type);
#pragma warning restore CS0659

    public static bool IsNotNever(Type type) => !IsNever(type);
    public static bool IsNever(Type type) => type.Equals(PrimitiveType.Never);
    public static bool IsNotUnknown(Type type) => !IsUnknown(type);
    public static bool IsUnknown(Type type) => type.Equals(PrimitiveType.Unknown);
    public static bool IsDefined(Type type) => !IsNone(type);
    public static bool IsNone(Type type) => type is PrimitiveType { Kind: PrimitiveTypeKind.Void or PrimitiveTypeKind.None };
    public static bool IsNotOptional(Type type) => !IsOptional(type);
    public static bool IsOptional(Type type) => IsNone(type) || type is OptionalType || type is UnionType union && union.Types.Any(t => IsNone(t) || IsOptional(t));
    protected static string ParenthesizeIfNeeded(Type type) => RequiresParentheses(type) ? $"({type})" : type.ToString();
    private static bool RequiresParentheses(Type type) => type is (UnionType or IntersectionType or FunctionType) and not OptionalType;
    
    public Type NonNullable() =>
        IsNone(this)
            ? PrimitiveType.Never
            : this is OptionalType optional
                ? optional.NonNullableType.NonNullable()
                : IsOptional(this)
                    ? TypeSimplifier.Simplify(this).NonNullable()
                    : this;

    public virtual Type Widen() => this;

    public virtual bool IsAssignableTo(Type other) =>
        other switch
        {
            InterfaceType interfaceType => IsAssignableTo(interfaceType.AssignabilityType),
            UnionType union => union.Types.Exists(IsAssignableTo),
            IntersectionType intersection => intersection.Types.TrueForAll(IsAssignableTo),
            PrimitiveType primitive => primitive.Kind == PrimitiveTypeKind.Unknown || this is PrimitiveType thisPrimitive && thisPrimitive.IsAssignableTo(primitive),
            InstantiatedType instantiated => IsAssignableTo(instantiated.Expand()),
            _ => Equals(other)
        };
    
    protected static bool ListEquals<T>(List<T> list, List<T> otherList) where T : Type =>
        list.Count == otherList.Count
        && list.All(t => otherList.Any(u => u.Equals(t)));
}