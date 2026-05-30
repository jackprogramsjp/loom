using Loom.Luau;
using Loom.Luau.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using Identifier = Loom.Luau.AST.Identifier;
using PrimitiveType = Loom.Luau.AST.PrimitiveType;
using UnaryOperator = Loom.Luau.AST.UnaryOperator;

namespace Loom.Testing;

public class LuauRenderingTest
{
    [Fact]
    public void Renders_ConstVariable_Annotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new ConstVariable("abc", PrimitiveType.Number, initializer);
        Assert.Equal("const abc: number = 1", Utility.Render(variable));
    }
    
    [Fact]
    public void Renders_ConstVariable_Unannotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new ConstVariable("abc", null, initializer);
        Assert.Equal("const abc = 1", Utility.Render(variable));
    }
    
    [Fact]
    public void Renders_LocalVariable_Annotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new LocalVariable("abc", PrimitiveType.Number, initializer);
        Assert.Equal("local abc: number = 1", Utility.Render(variable));
    }
    
    [Fact]
    public void Renders_LocalVariable_Annotated_NoInitializer()
    {
        var type = new OptionalType(PrimitiveType.Number);
        var variable = new LocalVariable("abc", type, null);
        Assert.Equal("local abc: number?", Utility.Render(variable));
    }
    
    [Fact]
    public void Renders_LocalVariable_Unannotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new LocalVariable("abc", null, initializer);
        Assert.Equal("local abc = 1", Utility.Render(variable));
    }
    
    [Fact]
    public void Renders_LocalVariable_Unannotated_NoInitializer()
    {
        var variable = new LocalVariable("abc", null, null);
        Assert.Equal("local abc", Utility.Render(variable));
    }
    
    [Fact]
    public void Renders_OptionalType_RequiresParens()
    {
        Assert.Equal("(string | boolean)?", Utility.Render(new OptionalType(new UnionType([PrimitiveType.String, PrimitiveType.Boolean]))));
    }
    
    [Fact]
    public void Renders_OptionalType()
    {
        Assert.Equal("number?", Utility.Render(new OptionalType(PrimitiveType.Number)));
    }
    
    [Fact]
    public void Renders_UnionType()
    {
        Assert.Equal("number | string | boolean", Utility.Render(new UnionType([PrimitiveType.Number, PrimitiveType.String, PrimitiveType.Boolean])));
    }
    
    [Fact]
    public void Renders_IntersectionType()
    {
        Assert.Equal("number & string & boolean", Utility.Render(new IntersectionType([PrimitiveType.Number, PrimitiveType.String, PrimitiveType.Boolean, ])));
    }
    
    [Theory]
    [InlineData(PrimitiveTypeKind.Number)]
    [InlineData(PrimitiveTypeKind.String)]
    [InlineData(PrimitiveTypeKind.Boolean)]
    [InlineData(PrimitiveTypeKind.Never)]
    [InlineData(PrimitiveTypeKind.Unknown)]
    [InlineData(PrimitiveTypeKind.Any)]
    [InlineData(PrimitiveTypeKind.Nil)]
    public void Renders_PrimitiveType(PrimitiveTypeKind kind)
    {
        Assert.Equal(kind.ToString().ToLower(), Utility.Render(new PrimitiveType(kind)));
    }
    
    [Fact]
    public void Renders_TypeName()
    {
        Assert.Equal("Hello", Utility.Render(new TypeName("Hello")));
    }
    
    [Fact]
    public void Renders_UnitType()
    {
        Assert.Equal("()", Utility.Render(new UnitType()));
    }
    
    [Fact]
    public void Renders_Identifier()
    {
        Assert.Equal("abc", Utility.Render(new Identifier("abc")));
    }
    
    [Fact]
    public void Renders_BinaryOperator()
    {
        var expression = new BinaryOperator(
            new NumberLiteral(1),
            "+",
            new NumberLiteral(2)
        );
        Assert.Equal("1 + 2", Utility.Render(expression));
    }
    
    [Fact]
    public void Renders_UnaryOperator()
    {
        var expression = new UnaryOperator("-", new NumberLiteral(1));
        Assert.Equal("-1", Utility.Render(expression));
    }
    
    [Fact]
    public void Renders_MultilineStringLiteral()
    {
        Assert.Equal("[[abc\ndef]]", Utility.Render(new StringLiteral("abc\ndef")));
    }
    
    [Fact]
    public void Renders_StringLiteral()
    {
        Assert.Equal($"{RenderState.StringDelimiter}abc{RenderState.StringDelimiter}", Utility.Render(new StringLiteral("abc")));
    }
    
    [Theory]
    [InlineData(12, "12")]
    [InlineData(69.420, "69.42")]
    [InlineData(1e3, "1000")]
    [InlineData(0xFF, "255")]
    public void Renders_NumberLiteral(double value, string expected)
    {
        Assert.Equal(expected, Utility.Render(new NumberLiteral(value)));
    }
    
    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Renders_BooleanLiteral(bool value, string expected)
    {
        Assert.Equal(expected, Utility.Render(new BooleanLiteral(value)));
    }
    
    [Fact]
    public void Renders_NilLiteral()
    {
        var output = Utility.Render(new NilLiteral());
        Assert.Equal("nil", output);
    }
}