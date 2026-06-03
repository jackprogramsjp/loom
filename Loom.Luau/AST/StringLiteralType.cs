namespace Loom.Luau.AST;

public class StringLiteralType(string value)
    : LuauLiteral<string>(value)
{
    public override string Render(RenderState state) => RenderState.StringDelimiter + RenderState.Escape(Value) + RenderState.StringDelimiter;
}