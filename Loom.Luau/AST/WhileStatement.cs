namespace Loom.Luau.AST;

public class WhileStatement(LuauExpression condition, Chunk body) : LuauStatement
{
    public LuauExpression Condition { get; } = condition;
    public Chunk Body { get; } = body;

    public override string Render(RenderState state) =>
        $"while {Condition.Render(state)} do\n"
        + state.Indented(Body.Render(state))
        + "end";
}