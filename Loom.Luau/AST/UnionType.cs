namespace Loom.Luau.AST;

public class UnionType(List<LuauType> types) : LuauType
{
    public List<LuauType> Types { get; } = types.Distinct().ToList();

    public override string Render(RenderState state) => string.Join(" | ", Types.ConvertAll(state.ParenthesizeIfNeeded));
}