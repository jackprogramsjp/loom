namespace Loom.TypeChecking.Types;

public sealed class LiteralType(object? value)
    : PrimitiveType(GetPrimitiveKind(value))
{
    private static PrimitiveTypeKind GetPrimitiveKind(object? value) =>
        value switch
        {
            long or int or double => PrimitiveTypeKind.Number,
            string => PrimitiveTypeKind.String,
            bool => PrimitiveTypeKind.Bool,
            _ => PrimitiveTypeKind.None
        };

    public object? Value { get; } = value switch
    {
        long or int or double => value,
        string => value,
        bool => value,
        null => value,
        _ => throw new ArgumentException($"Unsupported literal type: {value.GetType()}")
    };

    public override bool Equals(Type? other) => base.Equals(other) && other is LiteralType literal && (Value?.Equals(literal.Value) ?? literal.Value == null);

    public override bool IsAssignableTo(Type other) => base.IsAssignableTo(other) || other is LiteralType otherLiteral && otherLiteral.Value == Value;

    public override string ToString() =>
        Value switch
        {
            null => "none",
            string s => '"' + s + '"',
            bool b => b.ToString().ToLower(),
            _ => Value.ToString()!
        };
}