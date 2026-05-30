namespace Loom.Luau.AST;

public class LuauTree(List<LuauStatement> statements) : LuauNode
{
    public List<LuauStatement> Statements { get; } = statements;

    public override string Render(RenderState state) => string.Join('\n', Statements.ConvertAll(statement => statement.Render(state)));
}