namespace Loom.Luau.AST;

public class FunctionType(TypeParameters? typeParameters, List<LuauType> parameterTypes, LuauType returnType) : LuauType
{
    public TypeParameters? TypeParameters { get; } = typeParameters;
    public List<LuauType> ParameterTypes { get; } = parameterTypes;
    public LuauType ReturnType { get; } = returnType;

    public override string Render(RenderState state) =>
        (TypeParameters?.Render(state) ?? "") + '(' + string.Join(", ", state.RenderList(ParameterTypes)) + ") -> " + state.ParenthesizeIfNeeded(ReturnType);
}