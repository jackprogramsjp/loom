namespace Loom.Luau;

public class NilLiteral : LuauExpression
{
    public override string Render(RenderState state) => "nil";
}