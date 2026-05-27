using Loom.Diagnostics.Debug;

namespace Loom.Syntax;

public class LocationSpan(Location start, Location end) : IExaminable
{
    public SourceFile File { get; } = start.File;
    public Location Start { get; } = start;
    public Location End { get; } = end;

    public string GetText() => File.SourceText[Start.Position..End.Position];
    public string Examine() => $"{File.RelativePath()} @ {Start.Line}:{Start.Character} - {End.Line}:{End.Character}";
}