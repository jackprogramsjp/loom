using Loom.Luau;

namespace Loom.Testing;

public class LuauRenderingTest
{
    [Fact]
    public void Renders_BinaryExpression()
    {
        var expression = new BinaryOperator(
            new NumberLiteral(1),
            "+",
            new NumberLiteral(2)
        );
        Assert.Equal("1 + 2\n", Utility.RenderExpression(expression));
    }
    
    [Fact]
    public void Renders_UnaryExpression()
    {
        var expression = new UnaryOperator("-", new NumberLiteral(1));
        Assert.Equal("-1\n", Utility.RenderExpression(expression));
    }
    
    [Fact]
    public void Renders_MultilineStringLiteral()
    {
        Assert.Equal("[[abc\ndef]]\n", Utility.RenderExpression(new StringLiteral("abc\ndef")));
    }
    
    [Fact]
    public void Renders_StringLiteral()
    {
        Assert.Equal($"{RenderState.StringDelimiter}abc{RenderState.StringDelimiter}\n", Utility.RenderExpression(new StringLiteral("abc")));
    }
    
    [Theory]
    [InlineData(12, "12\n")]
    [InlineData(69.420, "69.42\n")]
    [InlineData(1e3, "1000\n")]
    [InlineData(0xFF, "255\n")]
    public void Renders_NumberLiteral(double value, string expected)
    {
        Assert.Equal(expected, Utility.RenderExpression(new NumberLiteral(value)));
    }
    
    [Theory]
    [InlineData(true, "true\n")]
    [InlineData(false, "false\n")]
    public void Renders_BooleanLiteral(bool value, string expected)
    {
        Assert.Equal(expected, Utility.RenderExpression(new BooleanLiteral(value)));
    }
    
    [Fact]
    public void Renders_NilLiteral()
    {
        var output = Utility.RenderExpression(new NilLiteral());
        Assert.Equal("nil\n", output);
    }
}