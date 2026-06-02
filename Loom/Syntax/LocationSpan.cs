namespace Loom.Syntax;

public class LocationSpan(Location start, Location end)
{
    public SourceFile File { get; } = start.File;
    public Location Start { get; } = start;
    public Location End { get; } = end;
    public int Length { get; } = end.Position - start.Position;

    public static LocationSpan Empty(SourceFile file) => new(Location.Empty(file), Location.Empty(file));

    public string GetText() => File.SourceText[Start.Position..End.Position];
    public override string ToString() => $"{File.Name} @ {Start.Line}:{Start.Character} - {End.Line}:{End.Character}";
}