using Loom.Config;

namespace Loom.Testing;

[Collection("Assembly")]
public class CompilerTest
{
    public static readonly IEnumerable<object[]> SnapshotFiles = Utility.GetSnapshotFiles("Luau", ".luau");

    [Theory]
    [MemberData(nameof(SnapshotFiles))]
    public void Compiles_Snapshots(string sourcePath, string snapshotPath)
    {
        var source = File.ReadAllText(sourcePath);
        var snapshot = File.ReadAllText(snapshotPath);
        AssertCompiled(source, snapshot);
    }

    private static void AssertCompiled(string source, string expected) =>
        Assert.Equal(expected.Replace(Environment.NewLine, "\n") + '\n', Compile(source).RenderedLuau);

    private static CompiledFile Compile(string source)
    {
        var compilationUnit = new CompilationUnit(new LoomConfig());
        var compiler = new Compiler(compilationUnit, Utility.TestFile(source));
        var file = compiler.Compile();
        Utility.AssertNoErrors(file.Diagnostics);

        return file;
    }
}