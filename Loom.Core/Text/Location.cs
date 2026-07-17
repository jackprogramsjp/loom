namespace Loom.Core.Text;

public readonly struct Location(SourceFile file, int position) : IEquatable<Location>
{
    public static Location Empty(SourceFile file) => new(file, 0);

    public static Location operator +(Location location, int n) =>
        new(location.File, location.Position + n);

    public SourceFile File { get; } = file;
    public int Position { get; } = position;
    public int Character => File.GetCharacterFromPosition(Position);
    public int Line => File.GetLineFromPosition(Position);

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