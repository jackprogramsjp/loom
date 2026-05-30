namespace Loom.Luau.AST;

public class PropertyAccess(LuauExpression target, List<string> names) : LuauExpression
{
    public char Operator { get; set; } = '.';
    public LuauExpression Target { get; } = target;
    public List<string> Names { get; } = names;

    public override string Render(RenderState state) => Target.Render(state) + (Names.Count > 1 ? '.' : "") + string.Join('.', Names[..^1]) + Operator + Names.Last();
}