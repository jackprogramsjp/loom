namespace Loom.TypeChecking.Types;

public class TypeName(Type underlying) : Type
{
    public Type Underlying { get; } = underlying;

    public override bool Equals(Type? other) => other is TypeName typeName && Underlying.Equals(typeName.Underlying);

    public override bool IsAssignableTo(Type other) => Underlying.IsAssignableTo(other);

    public override string ToString() => Underlying.ToString();
}