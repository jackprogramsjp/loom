namespace Loom.Luau.AST;

public class BooleanLiteralType(bool value)
    : LuauLiteralType<bool>(value)
{
    public override string Render(RenderState state) => Value.ToString().ToLower();
}