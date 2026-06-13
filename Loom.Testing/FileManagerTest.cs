namespace Loom.Testing;

[Collection("Assembly")]
public class FileManagerTest
{
    [Fact]
    public void Loads_Single()
    {
        var file = FileManager.LoadSingle($"{AssemblyFixture.TestFiles}/my-file.loom");
        Assert.Equal("my-file.loom", file.Name);
        Assert.Equal($"{AssemblyFixture.TestFiles}{Path.DirectorySeparatorChar}my-file.loom", file.RelativePath());
        Assert.Equal("my-file.loom", file.RelativePath(AssemblyFixture.TestFiles));
        Assert.Equal("1 + 2", file.SourceText);
    }
}