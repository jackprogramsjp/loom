namespace Loom.Luau;

public class UnaryOperator(string @operator, LuauExpression operand) : LuauExpression
{
    public string Operator { get; } = @operator;
    public LuauExpression Operand { get; } = operand;

    public override string Render(RenderState state) => Operator + Operand.Render(state);
}