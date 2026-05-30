namespace Loom.Luau.AST;

public abstract class LuauNode
{
    public string Render() => Render(new RenderState());
    public abstract string Render(RenderState state);
}