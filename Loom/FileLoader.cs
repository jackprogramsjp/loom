using Loom.Syntax;

namespace Loom;

internal static class FileLoader
{
    public static SourceFile LoadSingle(string path) => new(Path.GetFullPath(path));
}