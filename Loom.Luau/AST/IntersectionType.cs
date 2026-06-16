namespace Loom.Luau.AST;

public class IntersectionType(List<LuauType> types) : LuauType
{
    public List<LuauType> Types { get; } = types;

    public override string Render(RenderState state) => string.Join(" & ", Types.ConvertAll(state.ParenthesizeIfNeeded));
}