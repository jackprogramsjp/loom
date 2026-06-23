using Loom.Text;

namespace Loom;

public static class FileManager
{
    public const string LoomExtension = ".loom";
    
    public static void WriteCompiledFile(CompiledFile file)
    {
        var directory = Path.GetDirectoryName(file.Path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllText(file.Path, file.RenderedLuau);
    }

    public static SourceFile LoadSingle(string path) => new(Path.GetFullPath(path));

    public static List<SourceFile> LoadDirectory(string directoryPath) =>
        LoadDirectory(directoryPath, SearchOption.AllDirectories);

    private static List<SourceFile> LoadDirectory(string directoryPath, SearchOption searchOption) =>
        !string.IsNullOrWhiteSpace(directoryPath) && Directory.Exists(directoryPath)
            ? Directory.GetFiles(directoryPath, $"*{LoomExtension}", searchOption).Select(LoadSingle).ToList()
            : [];
}