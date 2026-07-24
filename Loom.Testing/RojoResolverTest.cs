using Loom.Config;

namespace Loom.Testing;

public sealed class RojoResolverTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "loom_rojo_test_" + Guid.NewGuid().ToString("N"));

    public RojoResolverTest() => Directory.CreateDirectory(_root);

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void Resolves_Runtime_Through_Directory_Mapping()
    {
        var dir = ProjectDir("dir_mapping");
        Write(dir, Path.Combine("include", RojoResolver.RuntimeFileName), "return {}");
        Write(dir, RojoResolver.ProjectFileName, """
            {
              "tree": {
                "$className": "DataModel",
                "ReplicatedStorage": {
                  "include": { "$path": "include" }
                }
              }
            }
            """);

        var resolver = RojoResolver.FromProjectDirectory(dir);
        Assert.NotNull(resolver);
        Assert.Equal(["ReplicatedStorage", "include", "loom_runtime"], resolver.ResolveRuntimePath());
    }

    [Fact]
    public void Resolves_Runtime_Through_Nested_Directory_Mapping()
    {
        var dir = ProjectDir("nested_mapping");
        Write(dir, Path.Combine("out", "vendor", RojoResolver.RuntimeFileName), "return {}");
        Write(dir, RojoResolver.ProjectFileName, """
            {
              "tree": {
                "$className": "DataModel",
                "ReplicatedStorage": {
                  "packages": { "$path": "out" }
                }
              }
            }
            """);

        var resolver = RojoResolver.FromProjectDirectory(dir);
        Assert.NotNull(resolver);
        Assert.Equal(["ReplicatedStorage", "packages", "vendor", "loom_runtime"], resolver.ResolveRuntimePath());
    }

    [Fact]
    public void Resolves_Runtime_When_Path_Points_At_File()
    {
        var dir = ProjectDir("file_mapping");
        Write(dir, Path.Combine("runtime", RojoResolver.RuntimeFileName), "return {}");
        Write(dir, RojoResolver.ProjectFileName, """
            {
              "tree": {
                "$className": "DataModel",
                "ReplicatedStorage": {
                  "LoomRuntime": { "$path": "runtime/loom_runtime.luau" }
                }
              }
            }
            """);

        var resolver = RojoResolver.FromProjectDirectory(dir);
        Assert.NotNull(resolver);
        Assert.Equal(["ReplicatedStorage", "LoomRuntime"], resolver.ResolveRuntimePath());
    }

    [Fact]
    public void Returns_Null_When_Runtime_Not_Mapped()
    {
        var dir = ProjectDir("no_runtime");
        Write(dir, Path.Combine("src", "main.luau"), "return {}");
        Write(dir, RojoResolver.ProjectFileName, """
            {
              "tree": {
                "$className": "DataModel",
                "ServerScriptService": {
                  "App": { "$path": "src" }
                }
              }
            }
            """);

        var resolver = RojoResolver.FromProjectDirectory(dir);
        Assert.NotNull(resolver);
        Assert.Null(resolver.ResolveRuntimePath());
    }

    [Fact]
    public void FromProjectDirectory_Returns_Null_Without_Project_File()
    {
        var dir = ProjectDir("no_project");
        Assert.Null(RojoResolver.FromProjectDirectory(dir));
    }

    [Fact]
    public void FromProjectDirectory_Returns_Null_For_Missing_Directory() =>
        Assert.Null(RojoResolver.FromProjectDirectory(Path.Combine(_root, "does_not_exist")));

    private string ProjectDir(string name)
    {
        var dir = Path.Combine(_root, name);
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void Write(string projectDir, string relativePath, string content)
    {
        var path = Path.Combine(projectDir, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }
}
