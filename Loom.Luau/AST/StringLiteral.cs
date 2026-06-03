namespace Loom.Luau.AST;

public class StringLiteral(string value) : LuauLiteral<string>(value)
{
    public override string Render(RenderState state)
    {
        var isMultiline = Value.Contains('\n');
        if (!isMultiline)
            return RenderState.StringDelimiter + RenderState.Escape(Value) + RenderState.StringDelimiter;

        var equals = GetSafeBracketEquals();
        return $"[{equals}[{Value}]{equals}]";
    }

    private string GetSafeBracketEquals()
    {
        var equalsCount = 0;
        while (Value.Contains($"]{equals()}]") || Value.EndsWith($"]{equals()}"))
            equalsCount++;

        return equals();

        string equals() => new('=', equalsCount);
    }
}