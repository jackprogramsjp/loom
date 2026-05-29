using Loom.Syntax;

namespace Loom;

public static class FileLoader
{
    public static SourceFile LoadSingle(string path) => new(Path.GetFullPath(path));
}