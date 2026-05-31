namespace Loom.Luau.AST;

public class TypeName(string name, List<LuauType>? typeArguments = null) : LuauType
{
    public string Name { get; } = name;
    public List<LuauType> TypeArguments { get; } = typeArguments ?? [];

    public override string Render(RenderState state) =>
        Name + (TypeArguments.Count > 0 ? $"<{string.Join(", ", TypeArguments.ConvertAll(t => t.Render(state)))}>" : "");
}