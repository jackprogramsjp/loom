namespace Loom.Luau.AST;

public class Do(Chunk body) : LuauStatement
{
    public Chunk Body { get; } = body;
    
    public override string Render(RenderState state) => "do\n" + state.Block(() => Body.Render(state)) + "end";
}