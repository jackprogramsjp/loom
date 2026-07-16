namespace Loom.Core.Text;

public readonly struct Location(SourceFile file, int character, int line, int position)
{
    public static Location Empty(SourceFile file) => new(file, 0, 1, 0);
    public static Location operator+(Location location, int n) => location with { Character = location.Character + n, Position = location.Position + n };

    public SourceFile File { get; } = file;
    public int Character { get; private init; } = character;
    public int Line { get; } = line;
    public int Position { get; private init; } = position;

    public override string ToString() => $"{File.Name}:{Line}:{Character}";
}