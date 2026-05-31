namespace Loom.Luau.AST;

public class TypeParameters(List<TypeParameter>? parameters = null) : LuauNode
{
    public List<TypeParameter> Parameters { get; } = parameters ?? [];

    public override string Render(RenderState state) =>
        Parameters.Count == 0
            ? ""
            : $"<{string.Join(", ", Parameters.ConvertAll(t => t.Render(state)))}>";
}