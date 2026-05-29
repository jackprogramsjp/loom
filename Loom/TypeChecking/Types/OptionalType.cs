namespace Loom.TypeChecking.Types;

public sealed class OptionalType(Type requiredType)
    : UnionType([requiredType, PrimitiveType.None])
{
    public Type RequiredType { get; } = requiredType;

    public override bool Equals(Type? other) => other is OptionalType optional && RequiredType.Equals(optional.RequiredType);

    public override string ToString() => RequiredType + "?";
}