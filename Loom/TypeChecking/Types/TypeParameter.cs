namespace Loom.TypeChecking.Types;

public sealed class TypeParameter(string name, Type? constraint = null, Type? defaultType = null) : Type
{
    public string Name { get; } = name;
    public Type? Constraint { get; } = constraint;
    public Type? DefaultType { get; } = defaultType;

    public override bool Equals(Type? other) =>
        other is TypeParameter parameter
        && (DefaultType?.Equals(parameter.DefaultType) ?? parameter.DefaultType == null)
        && (Constraint?.Equals(parameter.Constraint) ?? parameter.Constraint == null);

    public override string ToString() => Name + (Constraint != null ? ": " + Constraint : "") + (DefaultType != null ? " = " + DefaultType : "");
}