namespace Loom.Luau;

public class BinaryOperator(LuauExpression left, string @operator, LuauExpression right) : LuauExpression
{
    public string Operator { get; } = @operator;
    public LuauExpression Left { get; } = left;
    public LuauExpression Right { get; } = right;

    public override string Render(RenderState state) => $"{Left.Render(state)} {Operator} {Right.Render(state)}";
}