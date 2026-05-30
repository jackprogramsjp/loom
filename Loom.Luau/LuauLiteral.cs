namespace Loom.Luau;

public abstract class LuauLiteral<T>(T value) : LuauExpression where T : notnull
{
    public T Value { get; } = value;
    
    public override string Render(RenderState state) => Value.ToString() ?? "???";
}