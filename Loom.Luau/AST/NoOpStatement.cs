namespace Loom.Luau.AST;

public class NoOpStatement : LuauStatement
{
    public override string Render(RenderState state) => "";
}