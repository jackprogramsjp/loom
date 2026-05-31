using Loom.Luau.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using ExpressionStatement = Loom.Luau.AST.ExpressionStatement;
using Identifier = Loom.Luau.AST.Identifier;
using IntersectionType = Loom.Luau.AST.IntersectionType;
using OptionalType = Loom.Luau.AST.OptionalType;
using PrimitiveType = Loom.Luau.AST.PrimitiveType;
using UnaryOperator = Loom.Luau.AST.UnaryOperator;
using UnionType = Loom.Luau.AST.UnionType;

namespace Loom.Testing;

[Collection("Assembly")]
public class LuauGeneratorTest
{
    [Fact]
    public void Generates_IntersectionTypes()
    {
        var luauTree = Utility.GetLuauAST("mut x: number & bool");
        Assert.Single(luauTree.Statements);
        
        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);
        
        var intersection = Assert.IsType<IntersectionType>(variable.DeclaredType);
        Assert.Equal(2, intersection.Types.Count);
        
        var left = Assert.IsType<PrimitiveType>(intersection.Types.First());
        var right = Assert.IsType<PrimitiveType>(intersection.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, left.Kind);
        Assert.Equal(PrimitiveTypeKind.Boolean, right.Kind);
    }
    
    [Fact]
    public void Generates_UnionTypes()
    {
        var luauTree = Utility.GetLuauAST("mut x: number | bool");
        Assert.Single(luauTree.Statements);
        
        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);
        
        var union = Assert.IsType<UnionType>(variable.DeclaredType);
        Assert.Equal(2, union.Types.Count);
        
        var left = Assert.IsType<PrimitiveType>(union.Types.First());
        var right = Assert.IsType<PrimitiveType>(union.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, left.Kind);
        Assert.Equal(PrimitiveTypeKind.Boolean, right.Kind);
    }
    
    [Fact]
    public void Generates_OptionalTypes()
    {
        var luauTree = Utility.GetLuauAST("mut x: number?;");
        Assert.Single(luauTree.Statements);
        
        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);
        
        var optional = Assert.IsType<OptionalType>(variable.DeclaredType);
        var inner = Assert.IsType<PrimitiveType>(optional.Inner);
        Assert.Equal(PrimitiveTypeKind.Number, inner.Kind);
    }
    
    [Fact]
    public void Generates_ParenthesizedType()
    {
        var luauTree = Utility.GetLuauAST("mut x: (number);");
        Assert.Single(luauTree.Statements);
        
        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);
        
        var parenthesized = Assert.IsType<ParenthesizedType>(variable.DeclaredType);
        var primitive = Assert.IsType<PrimitiveType>(parenthesized.Type);
        Assert.Equal("number", primitive.Render());
    }
    
    [Theory]
    [InlineData("number")]
    [InlineData("string")]
    [InlineData("bool", "boolean")]
    [InlineData("never")]
    [InlineData("unknown")]
    [InlineData("none", "nil")]
    [InlineData("void", "nil")]
    public void Generates_PrimitiveTypes(string name, string? expected = null)
    {
        var luauTree = Utility.GetLuauAST($"mut x: {name};");
        Assert.Single(luauTree.Statements);
        
        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);
        
        var primitive = Assert.IsType<PrimitiveType>(variable.DeclaredType);
        Assert.Equal(expected ?? name, primitive.Render());
    }
    
    [Fact]
    public void Generates_TypeAliases_GenericWithDefault()
    {
        var luauTree = Utility.GetLuauAST("type Id<T = number> = T");
        Assert.Single(luauTree.Statements);
        
        var alias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Id", alias.Name);
        Assert.Single(alias.TypeParameters.Parameters);
        
        var parameter = alias.TypeParameters.Parameters.First();
        Assert.Equal("T", parameter.Name);
        
        var primitive = Assert.IsType<PrimitiveType>(parameter.DefaultType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
        
        var typeName = Assert.IsType<TypeName>(alias.Type);
        Assert.Equal("T", typeName.Name);
    }
    
    [Fact]
    public void Generates_TypeAliases_Generic()
    {
        var luauTree = Utility.GetLuauAST("type Id<T> = T");
        Assert.Single(luauTree.Statements);
        
        var alias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Id", alias.Name);
        Assert.Single(alias.TypeParameters.Parameters);
        
        var parameter = alias.TypeParameters.Parameters.First();
        Assert.Equal("T", parameter.Name);
        Assert.Null(parameter.DefaultType);
        
        var typeName = Assert.IsType<TypeName>(alias.Type);
        Assert.Equal("T", typeName.Name);
    }
    
    [Fact]
    public void Generates_TypeAliases()
    {
        var luauTree = Utility.GetLuauAST("type A = bool");
        Assert.Single(luauTree.Statements);
        
        var alias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("A", alias.Name);
        
        var primitive = Assert.IsType<PrimitiveType>(alias.Type);
        Assert.Equal(PrimitiveTypeKind.Boolean, primitive.Kind);
    }
    
    [Fact]
    public void Generates_ConstVariables()
    {
        var luauTree = Utility.GetLuauAST("let x = 1;");
        Assert.Single(luauTree.Statements);
        
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        Assert.Null(variable.DeclaredType);
        Assert.Equal("x", variable.Name);
        
        var literal = Assert.IsType<NumberLiteral>(variable.Initializer);
        Assert.Equal(1, literal.Value);
    }
    
    [Fact]
    public void Generates_LocalVariables()
    {
        var luauTree = Utility.GetLuauAST("mut x = 1;");
        Assert.Single(luauTree.Statements);
        
        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.Null(variable.DeclaredType);
        Assert.NotNull(variable.Initializer);
        Assert.Equal("x", variable.Name);
        
        var literal = Assert.IsType<NumberLiteral>(variable.Initializer);
        Assert.Equal(1, literal.Value);
    }
    
    [Fact]
    public void Generates_Parenthesized()
    {
        var luauTree = Utility.GetLuauAST("(abc)");
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var parenthesized = Assert.IsType<Parenthesized>(expressionStatement.Expression);
        var identifier = Assert.IsType<Identifier>(parenthesized.Expression);
        Assert.Equal("abc", identifier.Name);
    }
    
    [Fact]
    public void Generates_Identifiers()
    {
        var luauTree = Utility.GetLuauAST("abc");
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var identifier = Assert.IsType<Identifier>(expressionStatement.Expression);
        Assert.Equal("abc", identifier.Name);
    }
    
    [Theory]
    [InlineData("a & b & c & d", true)]
    [InlineData("a << b << c << d", false)]
    public void Generates_ConcatenatedBitwiseArguments(string source, bool isConcatenated)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var call = Assert.IsType<Call>(expressionStatement.Expression);
        Assert.IsType<PropertyAccess>(call.Callee);

        Assert.Equal(isConcatenated ? 4 : 2, call.Arguments.Count);
    }
    
    [Theory]
    [InlineData("a & b", "band")]
    [InlineData("a | b", "bor")]
    [InlineData("a ~ b", "bxor")]
    [InlineData("a << b", "lshift")]
    [InlineData("a >> b", "arshift")]
    [InlineData("a >>> b", "rshift")]
    public void Generates_MappedBitwiseOperators(string source, string expectedMethod)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);
        
        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var call = Assert.IsType<Call>(expressionStatement.Expression);
        var access = Assert.IsType<PropertyAccess>(call.Callee);
        var bit32Identifier = Assert.IsType<Identifier>(access.Target);
        Assert.Equal("bit32", bit32Identifier.Name);
        Assert.Single(access.Names);

        var name = access.Names.First();
        Assert.Equal(expectedMethod, name);
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