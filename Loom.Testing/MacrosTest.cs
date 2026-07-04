using Loom.Luau.AST;

namespace Loom.Testing;

[Collection("Assembly")]
public class MacrosTest
{
    [Fact]
    public void Generates_Result_Ok()
    {
        const string source = "Result.ok(69)";
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Equal(2, table.Initializers.Count);

        var kindInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        var valueInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[1]);
        Assert.Equal("kind", kindInit.PropertyName);
        Assert.Equal("value", valueInit.PropertyName);

        var kindValue = Assert.IsType<NumberLiteral>(kindInit.Value);
        var value = Assert.IsType<NumberLiteral>(valueInit.Value);
        Assert.Equal(0, kindValue.Value);
        Assert.Equal(69, value.Value);
    }
    
    [Fact]
    public void Generates_Result_Err()
    {
        const string source = "Result.err('stupid program')";
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Equal(2, table.Initializers.Count);

        var kindInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        var errorInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[1]);
        Assert.Equal("kind", kindInit.PropertyName);
        Assert.Equal("error", errorInit.PropertyName);

        var kindValue = Assert.IsType<NumberLiteral>(kindInit.Value);
        var errorValue = Assert.IsType<StringLiteral>(errorInit.Value);
        Assert.Equal(1, kindValue.Value);
        Assert.Equal("stupid program", errorValue.Value);
    }
}