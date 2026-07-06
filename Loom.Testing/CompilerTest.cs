using Loom.Projects;

namespace Loom.Testing;

[Collection("Assembly")]
public class CompilerTest
{
    [Fact]
    public void BasicConstantVariable() => AssertCompiled("let x: bool = true;", "const x: boolean = true");
    public void WrapsOrphanedExpression() => AssertCompiled("69", "const _ = 69");

    private static void AssertCompiled(string source, string expected) => Assert.Equal(expected + '\n', Compile(source).RenderedLuau);
    
    private static CompiledFile Compile(string source)
    {
        var compilationUnit = new CompilationUnit(new LoomConfig());
        var compiler = new Compiler(compilationUnit, Utility.TestFile(source));
        return compiler.Compile();
    }
}