namespace Loom.Text;

public sealed class Location(SourceFile file, int character, int line, int position)
{
    public SourceFile File { get; } = file;
    public int Character { get; } = character;
    public int Line { get; } = line;
    public int Position { get; } = position;

    public static Location Empty(SourceFile file) => new(file, 0, 1, 0);
    public static Location operator+(Location location, int n) => new(location.File, location.Character + n, location.Line, location.Position + n);

    public override string ToString() => $"{File.Name}:{Line}:{Character}";
}