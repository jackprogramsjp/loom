namespace Loom.TypeChecking.Types;

public class TypeParameter(string name, Type? defaultType, Type? constraint) : Type
{
    public string Name { get; } = name;
    public Type? DefaultType { get; } = defaultType;
    public Type? Constraint { get; } = constraint;

    public override bool Equals(Type? other) =>
        other is TypeParameter parameter
        && Name == parameter.Name
        && (DefaultType?.Equals(parameter.DefaultType) ?? parameter.DefaultType == null)
        && (Constraint?.Equals(parameter.Constraint) ?? parameter.Constraint == null);

    public override string ToString() => Name + (Constraint != null ? " : " + Constraint : "") + (DefaultType != null ? " = " + DefaultType : "");
}