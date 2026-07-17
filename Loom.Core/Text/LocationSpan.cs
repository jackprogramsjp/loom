namespace Loom.Core.Text;

public readonly struct LocationSpan
    : IEquatable<LocationSpan>
{
    public LocationSpan(Location start, Location end)
    {
        File = start.File;
        Start = start;
        End = end;
        Length = end.Position - start.Position;
    }
    
    public LocationSpan(Location start, int length)
    {
        File = start.File;
        Start = start;
        End = start + length;
        Length = length;
    }

    public SourceFile File { get; }
    public Location Start { get; }
    public Location End { get; }
    public int Length { get; }

    public static LocationSpan Empty(SourceFile? file = null) => new(Location.Empty(file ?? SourceFile.Empty), Location.Empty(file ?? SourceFile.Empty));
    public static LocationSpan operator+(LocationSpan span, int n) => new(span.Start + n, span.End + n);
    public static bool operator ==(LocationSpan left, LocationSpan right) => left.Equals(right);
    public static bool operator !=(LocationSpan left, LocationSpan right) => !(left == right);

    public string GetText() => File.SourceText.Substring(Start.Position, Length);
    
    public bool Equals(LocationSpan other) =>
        File.Equals(other.File)
        && Start.Equals(other.Start)
        && End.Equals(other.End)
        && Length == other.Length;

    public override bool Equals(object? obj) => obj is LocationSpan other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(File, Start, End, Length);
    public override string ToString() => $"{File.Name} @ {Start.Line}:{Start.Character} - {End.Line}:{End.Character}";
}