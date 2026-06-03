using Loom.Luau.AST;

namespace Loom.Luau;

public class RenderState
{
    public static char Indent { get; set; } = '\t';
    public static char StringDelimiter { get; set; } = '"';

    private string _indent = "";

    public List<string> RenderList<T>(List<T> nodes)
        where T : LuauNode =>
        nodes.ConvertAll(a => a.Render(this));

    public string Line(LuauNode node) => Line(node.Render(this));
    public string Line(string text) => NewLine(Indented(text));

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

    public T Block<T>(Func<T> callback)
    {
        PushIndent();
        var result = callback();
        PopIndent();
        return result;
    }

    private static string NewLine(string text) => text + '\n';
    private string Indented(string text) => _indent + text;
    private void PushIndent() => _indent += Indent;
    private void PopIndent() => _indent = _indent[..^1];
}