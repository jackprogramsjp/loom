namespace Loom.Luau.AST;

public class BooleanLiteral(bool value)
    : LuauLiteral<bool>(value)
{
    public override string Render(RenderState state) => Value.ToString().ToLower();
}