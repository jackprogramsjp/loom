namespace Loom.Luau.AST;

public class UnitType : LuauType
{
    public override string Render(RenderState state) => "()";
}