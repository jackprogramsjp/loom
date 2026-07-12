namespace Loom.Luau.AST;

public class Comment(string content) : LuauStatement
{
    public string Content { get; } = content;

    public override string Render(RenderState state) => Content.Contains('\n') ? $"[[{Content}]]" : $"--{Content}";
}