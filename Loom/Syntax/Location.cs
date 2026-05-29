namespace Loom.Syntax;

public class Location(SourceFile file, int character, int line, int position)
{
    public SourceFile File { get; } = file;
    public int Character { get; } = character;
    public int Line { get; } = line;
    public int Position { get; } = position;

    public static Location Empty(SourceFile file) => new(file, 0, 1, 0);

    public override string ToString() => $"{Line}:{Character}:{File.RelativePath()}";
}