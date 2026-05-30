namespace Loom.Luau;

public class RenderState
{
    public static char Indent { get; set; } = '\t';
    public static char StringDelimiter { get; set; } = '"';
    
    private string _indent = "";

    public string Line(LuauNode node) => Line(node.Render(this));
    public string Line(string text) => NewLine(Indented(text));

    public T Block<T>(Func<T> callback)
    {
        PushIndent();
        var result = callback();
        PopIndent();
        return result;
    }

    private string NewLine(string text) => text + '\n';
    private string Indented(string text) => _indent + text;
    private void PushIndent() => _indent += Indent;
    private void PopIndent() => _indent = _indent[..^1];
}