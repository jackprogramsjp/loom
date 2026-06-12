namespace Loom.Luau.AST;

public class Chunk(List<LuauStatement> statements) : LuauStatement
{
    public List<LuauStatement> Statements { get; } = statements;

    public override string Render(RenderState state)
    {
        var renders = Statements.ConvertAll(statement => statement.Render(state));
        return this is LuauTree
            ? string.Join('\n', renders.ConvertAll(state.IndentedLine))
            : state.Block(() => string.Join('\n', renders.ConvertAll(state.IndentedLine)));
    }
}