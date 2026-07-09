using System.Globalization;

namespace Loom.Luau.AST;

public abstract class LuauLiteral<T>(T value) : LuauExpression
    where T : notnull
{
    public T Value { get; } = value;

    public override string Render(RenderState state) =>
        Value is double n
            ? n.ToString("R", CultureInfo.InvariantCulture).Replace('E', 'e').Replace("+", "")
            : Value.ToString() ?? "???";
}