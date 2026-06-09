using Loom.Luau.AST;
using BinaryOperator = Loom.Parsing.AST.BinaryOperator;
using ExpressionStatement = Loom.Parsing.AST.ExpressionStatement;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;

namespace Loom.Testing;

public class CompilationUnitTest
{
    [Fact]
    public void Compiles_SingleFile()
    {
        var file = FileLoader.LoadSingle($"{AssemblyFixture.TestFiles}/my-file.loom");
        var compiledFile = CompilationUnit.Compile(file);
        Assert.Empty(compiledFile.Diagnostics.WithoutInfo().Set);
        Assert.Equal(3, compiledFile.Tokens.Count);
        Assert.Single(compiledFile.Tree.Statements);
        Assert.IsType<BinaryOperator>(Assert.IsType<ExpressionStatement>(compiledFile.Tree.Statements.First()).Expression);
        Assert.Null(compiledFile.SemanticModel.GetSymbol(compiledFile.Tree));
        Assert.Equal(PrimitiveType.Number, compiledFile.ReturnType);
        Assert.Single(compiledFile.LuauTree.Statements);
        
        var variable = Assert.IsType<ConstVariable>(compiledFile.LuauTree.Statements.First());
        var binary = Assert.IsType<Luau.AST.BinaryOperator>(variable.Initializer);
        Assert.Equal("_", variable.Name);
        Assert.IsType<NumberLiteral>(binary.Left);
        Assert.IsType<NumberLiteral>(binary.Right);
    }
}