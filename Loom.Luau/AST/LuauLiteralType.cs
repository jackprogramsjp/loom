using System.Globalization;

namespace Loom.Luau.AST;

public abstract class LuauLiteralType<T>(T value)
    : PrimitiveType(
        value switch
        {
            long or int or double => PrimitiveTypeKind.Number,
            string => PrimitiveTypeKind.String,
            bool => PrimitiveTypeKind.Boolean,
            null => PrimitiveTypeKind.Nil,
            _ => PrimitiveTypeKind.Never
        }
    )
    where T : notnull
{
    public T Value { get; } = value;

    public override string Render(RenderState state) =>
        Value is double n
            ? n.ToString(CultureInfo.InvariantCulture).Replace("E+", "e")
            : Value.ToString() ?? "???";
}