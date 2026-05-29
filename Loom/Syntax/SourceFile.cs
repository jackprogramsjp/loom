namespace Loom.Syntax;

public class SourceFile(string absolutePath, string? sourceText = null)
{
    public string AbsolutePath { get; } = absolutePath;
    public string SourceText { get; } = sourceText ?? File.ReadAllText(absolutePath);

    public string RelativePath(string to = ".") => Path.GetRelativePath(to, AbsolutePath);
}