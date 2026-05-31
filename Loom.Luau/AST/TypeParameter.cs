namespace Loom.Luau.AST;

public class TypeParameter(string name, LuauType? defaultType) : LuauNode
{
    public bool OfFunction { get; set; } = false;
    public string Name { get; } = name;
    public LuauType? DefaultType { get; } = defaultType;
    
    public override string Render(RenderState state) => Name + (OfFunction ? "" : DefaultType != null ? " = " + DefaultType.Render(state) : "");
}