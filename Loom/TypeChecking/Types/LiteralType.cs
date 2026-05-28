namespace Loom.TypeChecking.Types;

public sealed class LiteralType(object? value)
    : PrimitiveType(value switch
    {
        int or double => PrimitiveTypeKind.Number,
        string => PrimitiveTypeKind.String,
        bool => PrimitiveTypeKind.Bool,
        _ => PrimitiveTypeKind.None
    })
{
    public object? Value { get; } = value switch
    {
        int or double => value,
        string => value,
        bool => value,
        null => value,
        _ => throw new ArgumentException($"Unsupported literal type: {value.GetType()}")
    };

    public override bool IsAssignableTo(Type other) => base.IsAssignableTo(other) || other is LiteralType otherLiteral && otherLiteral.Value == Value;

    public override string ToString() => Kind.ToString().ToLower();
}