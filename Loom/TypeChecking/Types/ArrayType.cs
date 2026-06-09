namespace Loom.TypeChecking.Types;

public sealed class ArrayType(Type elementType, bool isMutable)
    : ObjectType(new ObjectIndexer(isMutable, new UnionType([PrimitiveType.Number]), elementType), [])
{
    public Type ElementType { get; } = elementType;
    public bool IsMutable { get; } = isMutable;

    public override bool Equals(Type? other) => other is ArrayType array && ElementType.Equals(array.ElementType) && IsMutable == array.IsMutable;

    public override bool IsAssignableTo(Type other)
    {
        if (base.IsAssignableTo(other))
            return true;

        if (other is not ArrayType targetArray)
            return false;

        if (!IsMutable && !targetArray.IsMutable)
            return ElementType.IsAssignableTo(targetArray.ElementType);

        var validMutability = IsMutable || !targetArray.IsMutable;
        return validMutability && (IsNever(ElementType) || ElementType.Equals(targetArray.ElementType));
    }

    public override Type Widen() => IsNever(ElementType) ? new ArrayType(PrimitiveType.Unknown, IsMutable) : this;

    public override string ToString() => $"{ParenthesizeIfNeeded(ElementType)}[{(IsMutable ? "mut" : "")}]";
}