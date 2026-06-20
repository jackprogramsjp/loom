namespace Loom.Luau.AST;

public class ElementAccess(LuauExpression target, LuauExpression index) : LuauExpression
{
    public LuauExpression Target { get; } = target;
    public LuauExpression Index { get; } = index;

    public override string Render(RenderState state) => $"{state.ParenthesizeIfNeeded(Target)}[{Index.Render(state)}]";
}