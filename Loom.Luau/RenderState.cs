using Loom.Luau.AST;

namespace Loom.Luau;

public sealed class RenderState
{
    public static string Indent { get; set; } = "  ";
    public static char StringDelimiter { get; set; } = '"';

    private string _indent = "";

    public List<string> RenderList<T>(List<T> nodes)
        where T : LuauNode =>
        nodes.ConvertAll(a => a.Render(this));

    public string IndentedLine(string text) => Indented(text) + '\n';
    public string Indented(string text) => _indent + text;

    public string ParenthesizeIfNeeded(LuauNode node) => RequiresParentheses(node) ? $"({node.Render(this)})" : node.Render(this);

    private static bool RequiresParentheses(LuauNode node) =>
        node is (UnionType or IntersectionType or FunctionType or Table or BinaryOperator or UnaryOperator or IfExpression or TypeCast) and not OptionalType;

    public static string RenderVisibility(LuauVisibility? visibility) =>
        visibility == null
            ? ""
            : visibility.ToString() is { } s
                ? s.ToLower() + " "
                : "";

    public static string Escape(string input) =>
        input
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\0", "\\0")
            .Replace("\a", "\\a")
            .Replace("\b", "\\b")
            .Replace("\f", "\\f")
            .Replace("\v", "\\v");

    public string Block(Func<string> callback)
    {
        PushIndent();
        var result = callback();
        PopIndent();

        return result;
    }

    private void PushIndent() => _indent += Indent;
    private void PopIndent() => _indent = _indent[..^2];
}