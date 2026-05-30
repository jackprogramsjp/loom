namespace Loom.Luau.AST;

public class OptionalType(LuauType inner) : LuauType
{
    public LuauType Inner { get; } = inner;

    public override string Render(RenderState state) => WrapParens(Inner.Render(state)) + "?";

    private string WrapParens(string content) => RequiresParens() ? $"({content})" : content;
    private bool RequiresParens() => Inner is UnionType or IntersectionType;
}