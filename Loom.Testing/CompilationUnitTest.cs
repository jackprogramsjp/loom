using Loom.Luau.AST;
using Loom.Projects;
using BinaryOperator = Loom.Parsing.AST.BinaryOperator;
using ExpressionStatement = Loom.Parsing.AST.ExpressionStatement;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;

namespace Loom.Testing;

public class CompilationUnitTest
{
    [Fact]
    public void Compiles_Project_NoEmit()
    {
        var config = GetConfig();
        config.NoEmit = true;
        
        var compilationUnit = new CompilationUnit(config);
        var result = compilationUnit.Compile();
        Utility.AssertNoErrors(result);
        Assert.Single(result.Files);

        var path = config.Files.OutputDirectory;
        Directory.Delete(path, true);
        Directory.CreateDirectory(path);
        
        var luauFiles = Directory.EnumerateFiles(path);
        Assert.Empty(luauFiles);
    }
    
    [Fact]
    public void Compiles_Project()
    {
        var config = GetConfig();
        var compilationUnit = new CompilationUnit(config);
        var result = compilationUnit.Compile();
        Utility.AssertNoErrors(result);
        Assert.Single(result.Files);

        var file = result.Files.First();
        Assert.EndsWith("my-file.luau", file.Path);
        Assert.Equal(4, file.Tokens.Count);
        Assert.Single(file.Tree.Statements);
        Assert.IsType<BinaryOperator>(Assert.IsType<ExpressionStatement>(file.Tree.Statements.First()).Expression);
        Assert.Null(file.SemanticModel.GetSymbol(file.Tree));
        Assert.Equal(PrimitiveType.Number, file.ReturnType);
        Assert.Single(file.LuauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(file.LuauTree.Statements.First());
        var binary = Assert.IsType<Luau.AST.BinaryOperator>(variable.Initializer);
        Assert.Equal("_", variable.Name);
        Assert.IsType<NumberLiteral>(binary.Left);
        Assert.IsType<NumberLiteral>(binary.Right);
    }

    private static LoomConfig GetConfig()
    {
        var config = ConfigReader.LocateFromDirectory(AssemblyFixture.TestFiles);
        Assert.NotNull(config);
        Assert.Equal(AssemblyFixture.TestFiles, config.ProjectDirectory);

        return config;
    }
}