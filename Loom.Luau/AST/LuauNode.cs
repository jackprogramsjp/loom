namespace Loom.Luau.AST;

public abstract class LuauNode
{
    public abstract string Render(RenderState state);
}