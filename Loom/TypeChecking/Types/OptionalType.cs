namespace Loom.TypeChecking.Types;

public sealed class OptionalType(Type nonNullableType)
    : UnionType([nonNullableType, PrimitiveType.None])
{
    public Type NonNullableType { get; } = nonNullableType;

    public override bool Equals(Type? other) => other is OptionalType optional && NonNullableType.Equals(optional.NonNullableType);

    public override string ToString() => NonNullableType + "?";
}