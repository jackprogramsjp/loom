namespace Loom.Luau.AST;

public class NumericForStatement(string name, LuauExpression start, LuauExpression end, LuauExpression? incrementBy, Chunk body) : LuauStatement
{
    public string Name { get; } = name;
    public LuauExpression Start { get; } = start;
    public LuauExpression End { get; } = end;
    public LuauExpression? IncrementBy { get; } = incrementBy;
    public Chunk Body { get; } = body;

    public override string Render(RenderState state) =>
        $"for {Name} = {Start.Render(state)}, {End.Render(state)}{(IncrementBy != null ? ", " + IncrementBy.Render(state) : "")} do\n"
        + state.Indented(Body.Render(state))
        + "end";
}