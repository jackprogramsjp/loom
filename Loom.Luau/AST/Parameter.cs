namespace Loom.Luau.AST;

public class Parameter(string name, LuauType? declaredType)
    : Variable(name, declaredType)
{
    public override string Render(RenderState state) => Name + RenderType(state);
}