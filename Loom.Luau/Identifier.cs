namespace Loom.Luau;

public class Identifier(string name) : LuauExpression
{
    public string Name { get; } = name;

    public override string Render(RenderState state) => Name;
}