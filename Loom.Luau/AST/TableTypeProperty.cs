namespace Loom.Luau.AST;

public class TableTypeProperty(LuauVisibility? visibility, string name, LuauType type) : LuauType
{
    public LuauVisibility? Visibility { get; } = visibility;
    public string Name { get; } = name;
    public LuauType Type { get; } = type;
    
    public override string Render(RenderState state) => $"{RenderVisibility()}{Name}: {Type.Render(state)}";
    private string RenderVisibility() => Visibility == null ? "" : Visibility.ToString() is {} s ? s.ToLower() + " " : "";
}