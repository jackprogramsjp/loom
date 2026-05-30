namespace Loom.Luau.AST;

public class NilLiteral : LuauExpression
{
    public override string Render(RenderState state) => "nil";
}