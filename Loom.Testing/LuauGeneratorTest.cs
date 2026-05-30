using Loom.Luau;
using ExpressionStatement = Loom.Luau.ExpressionStatement;
using UnaryOperator = Loom.Luau.UnaryOperator;

namespace Loom.Testing;

public class LuauGeneratorTest
{
    [Fact]
    public void Generates_Identifiers()
    {
        var luauTree = Utility.GetLuauAST("abc");
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var identifier = Assert.IsType<Identifier>(expressionStatement.Expression);
        Assert.Equal("abc", identifier.Name);
    }
    
    [Fact]
    public void Generates_BinaryOperators()
    {
        var luauTree = Utility.GetLuauAST("1 + 2");
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var binary = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
        var left = Assert.IsType<NumberLiteral>(binary.Left);
        var right = Assert.IsType<NumberLiteral>(binary.Right);
        Assert.Equal(1, left.Value);
        Assert.Equal(2, right.Value);
        Assert.Equal("+", binary.Operator);
    }
    
    [Fact]
    public void Generates_UnaryOperators()
    {
        var luauTree = Utility.GetLuauAST("!false");
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var unary = Assert.IsType<UnaryOperator>(expressionStatement.Expression);
        Assert.IsType<BooleanLiteral>(unary.Operand);
        Assert.Equal("not ", unary.Operator);
    }
    
    [Theory]
    [InlineData("420", 420)]
    [InlineData("69.420", 69.42)]
    [InlineData(".5", 0.5)]
    public void Generates_NumberLiterals(string source, double expected)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var literal = Assert.IsType<NumberLiteral>(expressionStatement.Expression);
        Assert.Equal(expected, literal.Value);
    }
    
    [Theory]
    [InlineData("'abc'", "abc")]
    [InlineData("\"def\"", "def")]
    public void Generates_StringLiterals(string source, string expected)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var literal = Assert.IsType<StringLiteral>(expressionStatement.Expression);
        Assert.Equal(expected, literal.Value);
    }
    
    [Fact]
    public void Generates_BoolLiterals()
    {
        var luauTree = Utility.GetLuauAST("true");
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var literal = Assert.IsType<BooleanLiteral>(expressionStatement.Expression);
        Assert.True(literal.Value);
    }
    
    [Fact]
    public void Generates_NilLiterals()
    {
        var luauTree = Utility.GetLuauAST("none");
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        Assert.IsType<NilLiteral>(expressionStatement.Expression);
    }
}