namespace Loom.Core.TypeChecking.Types;

public sealed class IndexedType(Type target, Type index) : Type
{
    public Type Target { get; } = target;
    public Type Index { get; } = index;

    public override int GetHashCode() => HashCode.Combine(Target.GetHashCode(), Index.GetHashCode());
    public override bool Equals(Type? other) => other is IndexedType indexedType && Target.Equals(indexedType.Target) && Index.Equals(indexedType.Index);
    public override string ToString() => $"{Target}[{Index}]";
}