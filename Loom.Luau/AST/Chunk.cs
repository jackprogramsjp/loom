namespace Loom.Luau.AST;

public class Chunk(List<LuauStatement> statements) : LuauStatement
{
    public List<LuauStatement> Statements { get; } = statements;

    public override string Render(RenderState state)
    {
        var renders = Statements.ConvertAll(statement => statement.Render(state)).SelectMany(render => render.Split('\n')).Select(state.IndentedLine);
        return this is LuauTree
            ? string.Join("", renders)
            : state.Block(() => string.Join("", renders));
    }
}