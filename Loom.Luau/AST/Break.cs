namespace Loom.Luau.AST;

public class Break : LuauStatement
{
    public override string Render(RenderState state) => "break";
}