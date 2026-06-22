namespace Loom.Luau.AST;

public class AnonymousFunction(TypeParameters? typeParameters, List<Parameter> parameters, LuauType? returnType, Chunk body) : LuauExpression
{
    public TypeParameters? TypeParameters { get; } = typeParameters;
    public List<Parameter> Parameters { get; } = parameters;
    public LuauType? ReturnType { get; } = returnType;
    public Chunk Body { get; } = body;

    public override string Render(RenderState state) =>
        "function"
        + (TypeParameters?.Render(state) ?? "")
        + '('
        + string.Join(", ", Parameters.ConvertAll(p => p.Render(state)))
        + ')'
        + (ReturnType != null ? ": " + ReturnType.Render(state) : "")
        + '\n'
        + Body.Render(state)
        + "end";
}