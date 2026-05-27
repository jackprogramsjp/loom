namespace Loom.Syntax;

public class SourceFile(string absolutePath)
{
    public string AbsolutePath { get; } = absolutePath;
    public string SourceText { get; } = File.ReadAllText(absolutePath);

    public string RelativePath(string to = ".")
    {
        return Path.GetRelativePath(to, AbsolutePath);
    }
}