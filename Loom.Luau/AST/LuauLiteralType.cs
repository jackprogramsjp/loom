namespace Loom.Luau.AST;

public abstract class LuauLiteralType<T>(T value)
    : PrimitiveType(
        value switch
        {
            string => PrimitiveTypeKind.String,
            bool => PrimitiveTypeKind.Boolean,
            _ => PrimitiveTypeKind.Never
        }
    )
    where T : notnull
{
    public T Value { get; } = value;

    public override string Render(RenderState state) => Value.ToString() ?? "???";
}