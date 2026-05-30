namespace Loom.Luau.AST;

public class TypeAlias(string name, LuauType type) : LuauStatement
{
    public string Name { get; } = name;
    public LuauType Type { get; } = type;
    
    public override string Render(RenderState state) => $"type {Name} = {Type.Render(state)}";
}