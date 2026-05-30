namespace Loom.Luau.AST;

public class TypeName(string name) : LuauType
{
    public string Name { get; } = name;

    public override string Render(RenderState state) => Name;
}