namespace Loom.Luau.AST;

public class OptionalType(LuauType inner) : LuauType
{
    public LuauType Inner { get; } = inner;

    public override string Render(RenderState state) => state.ParenthesizeIfNeeded(Inner) + "?";
}