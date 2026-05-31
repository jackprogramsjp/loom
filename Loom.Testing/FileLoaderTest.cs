namespace Loom.Testing;

[Collection("Assembly")]
public class FileLoaderTest
{
    [Fact]
    public void Loads_Single()
    {
        var file = FileLoader.LoadSingle("../../../TestFiles/my-file.loom");
        Assert.Equal("my-file.loom", file.Name);
        Assert.Equal($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}TestFiles{Path.DirectorySeparatorChar}my-file.loom", file.RelativePath());
        Assert.Equal("my-file.loom", file.RelativePath("../../../TestFiles"));
        Assert.Equal("1 + 2", file.SourceText);
    }
}