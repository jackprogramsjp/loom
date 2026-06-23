namespace Loom.Text;

public sealed class SourceFile
{
    public static readonly SourceFile Empty = new("<anonymous>", string.Empty);
    
    public string AbsolutePath { get; }
    public string Name { get; }
    public string SourceText { get; }
    public bool IsDeclaration { get; }
    
    public SourceFile(string absolutePath, string? sourceText = null)
    {
        AbsolutePath = absolutePath;
        Name = Path.GetFileName(absolutePath);
        SourceText = sourceText ?? File.ReadAllText(absolutePath);
        IsDeclaration = Name.EndsWith(".d.loom");
    }

    public override string ToString() => Name;

    public string RelativePath(string to = ".") => Path.GetRelativePath(to, AbsolutePath);
}