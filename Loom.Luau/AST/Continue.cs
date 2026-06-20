namespace Loom.Luau.AST;

public class Continue : LuauStatement
{
    public override string Render(RenderState state) => "continue";
}