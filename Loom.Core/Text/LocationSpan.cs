namespace Loom.Text;

public sealed class LocationSpan
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

    public string GetText() => File.SourceText[Start.Position..End.Position];
    public override string ToString() => $"{File.Name} @ {Start.Line}:{Start.Character} - {End.Line}:{End.Character}";
}