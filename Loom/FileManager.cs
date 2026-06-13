using Loom.Syntax;

namespace Loom;

public static class FileManager
{
    public const string LoomExtension = ".loom";
    
    public static void WriteCompiledFile(CompiledFile file) => File.WriteAllText(file.Path, file.RenderedLuau);
    public static SourceFile LoadSingle(string path) => new(Path.GetFullPath(path));
    public static List<SourceFile> LoadDirectory(string directoryPath) => LoadDirectory(directoryPath, SearchOption.AllDirectories);

    public static List<SourceFile> LoadDirectory(string directoryPath, SearchOption searchOption) =>
        !string.IsNullOrWhiteSpace(directoryPath) && Directory.Exists(directoryPath)
            ? Directory.GetFiles(directoryPath, $"*{LoomExtension}", searchOption).Select(LoadSingle).ToList()
            : [];
}