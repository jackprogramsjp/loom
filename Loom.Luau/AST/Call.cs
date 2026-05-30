namespace Loom.Luau.AST;

public class Call(LuauExpression callee, List<LuauExpression> arguments, bool isMethod = false) : LuauExpression
{
    public LuauExpression Callee { get; } = callee;
    public List<LuauExpression> Arguments { get; } = arguments;
    public bool IsMethod { get; } = isMethod;

    public override string Render(RenderState state) => $"{RenderCallee(state)}({string.Join(", ", state.RenderList(Arguments))})";

    private string RenderCallee(RenderState state)
    {
        if (IsMethod && Callee is PropertyAccess access)
            access.Operator = ':';

        return Callee.Render(state);
    }
}