namespace Loom.Core.TypeChecking.Types;

public sealed class TypeVariable(int id) : Type
{
    public int Id { get; } = id;

    public override bool IsAssignableTo(Type other) => false;
    public override bool Equals(Type? other) => other is TypeVariable v && Id == v.Id;
    public override string ToString() => $"T{Id}";
}