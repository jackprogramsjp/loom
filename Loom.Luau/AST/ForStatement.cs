namespace Loom.Luau.AST;

public class ForStatement(List<string> names, LuauExpression expression, Chunk body) : LuauStatement
{
    public List<string> Names { get; } = names;
    public LuauExpression Expression { get; } = expression;
    public Chunk Body { get; } = body;

    public override string Render(RenderState state) =>
        $"for {string.Join(", ", Names)} in {Expression.Render(state)} do\n"
        + state.Indented(Body.Render(state))
        + "end";
}