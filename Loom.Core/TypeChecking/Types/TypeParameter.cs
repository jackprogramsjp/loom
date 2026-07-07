namespace Loom.TypeChecking.Types;

public sealed class TypeParameter(string name, Type? constraint = null, Type? defaultType = null) : Type
{
    public string Name { get; } = name;
    public Type? Constraint { get; } = constraint;
    public Type? DefaultType { get; } = defaultType;
    
    public override bool Equals(Type? other) =>
        other is TypeParameter parameter
        && Equals(Constraint, parameter.Constraint)
        && Equals(DefaultType, parameter.DefaultType);

    public override string ToString() => Name + (Constraint != null ? ": " + Constraint : "") + (DefaultType != null ? " = " + DefaultType : "");
}