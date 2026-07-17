namespace Loom.Core.Text;

public readonly struct Location(SourceFile file, int character, int line, int position) : IEquatable<Location>
{
    public static Location Empty(SourceFile file) => new(file, 0, 1, 0);
    public static Location operator +(Location location, int n) => location with { Character = location.Character + n, Position = location.Position + n };

    public SourceFile File { get; } = file;
    public int Character { get; private init; } = character;
    public int Line { get; } = line;
    public int Position { get; private init; } = position;

    public static bool operator ==(Location left, Location right) => left.Equals(right);
    public static bool operator !=(Location left, Location right) => !(left == right);

    public bool Equals(Location other) =>
        File.Equals(other.File)
        && Character == other.Character
        && Line == other.Line
        && Position == other.Position;

    public override bool Equals(object? obj) => obj is Location other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(File, Character, Line, Position);
    public override string ToString() => $"{File.Name}:{Line}:{Character}";
}