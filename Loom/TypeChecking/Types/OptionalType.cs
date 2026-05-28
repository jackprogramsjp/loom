namespace Loom.TypeChecking.Types;

public sealed class OptionalType(Type requiredType)
    : UnionType([requiredType, PrimitiveType.None])
{
    public Type RequiredType { get; } = requiredType;

    public override string ToString() => RequiredType + "?";
}