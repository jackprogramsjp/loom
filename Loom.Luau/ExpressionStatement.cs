namespace Loom.Luau;

public class ExpressionStatement(LuauExpression expression) : LuauStatement
{
    public LuauExpression Expression { get; } = expression;
    
    public override string Render(RenderState state) => state.Line(Expression);
}