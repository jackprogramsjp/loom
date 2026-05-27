using Loom.Diagnostics.Debug;

namespace Loom.Syntax;

public class Location(SourceFile file, int character, int line, int position) : IExaminable
{
    public SourceFile File { get; } = file;
    public int Character { get; } = character;
    public int Line { get; } = line;
    public int Position { get; } = position;
    
    public string Examine() => $"{Line}:{Character}:{File.RelativePath()}";
}