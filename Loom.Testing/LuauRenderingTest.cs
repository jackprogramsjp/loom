using Loom.Luau;
using Loom.Luau.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using Identifier = Loom.Luau.AST.Identifier;
using PrimitiveType = Loom.Luau.AST.PrimitiveType;
using UnaryOperator = Loom.Luau.AST.UnaryOperator;

namespace Loom.Testing;

[Collection("Assembly")]
public class LuauRenderingTest
{
    [Fact]
    public void Renders_TypeAlias_GenericWithDefault()
    {
        var typeParameters = new TypeParameters([new TypeParameter("T", PrimitiveType.Number)]);
        Assert.Equal("type Id<T = number> = T", new TypeAlias("Id", typeParameters, new TypeName("T")).Render());
    }

    [Fact]
    public void Renders_TypeAlias_Generic()
    {
        var typeParameters = new TypeParameters([new TypeParameter("T", null)]);
        Assert.Equal("type Id<T> = T", new TypeAlias("Id", typeParameters, new TypeName("T")).Render());
    }

    [Fact]
    public void Renders_TypeAlias()
    {
        Assert.Equal("type A = boolean", new TypeAlias("A", new TypeParameters(), PrimitiveType.Boolean).Render());
    }

    [Fact]
    public void Renders_Call()
    {
        var emptyCall = new Call(new Identifier("abc"), []);
        var call = new Call(new Identifier("abc"), [new StringLiteral("foo")]);
        Assert.Equal("abc()", emptyCall.Render());
        Assert.Equal("abc(\"foo\")", call.Render());
    }

    [Fact]
    public void Renders_PropertyAccess_MethodCall()
    {
        var target = new Identifier("abc");
        var access = new PropertyAccess(target, ["foo", "bar"]);
        var call = new Call(access, [], true);
        Assert.Equal("abc.foo:bar()", call.Render());
    }

    [Fact]
    public void Renders_PropertyAccess()
    {
        var target = new Identifier("abc");
        var access = new PropertyAccess(target, ["foo"]);
        var bigAccess = new PropertyAccess(target, ["foo", "bar"]);
        Assert.Equal("abc.foo", access.Render());
        Assert.Equal("abc.foo.bar", bigAccess.Render());
    }

    [Fact]
    public void Renders_ElementAccess()
    {
        var target = new Identifier("abc");
        var access = new ElementAccess(target, new StringLiteral("foo"));
        Assert.Equal("abc[\"foo\"]", access.Render());
    }

    [Fact]
    public void Renders_ConstVariable_Annotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new ConstVariable("abc", PrimitiveType.Number, initializer);
        Assert.Equal("const abc: number = 1", variable.Render());
    }

    [Fact]
    public void Renders_ConstVariable_Unannotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new ConstVariable("abc", null, initializer);
        Assert.Equal("const abc = 1", variable.Render());
    }

    [Fact]
    public void Renders_LocalVariable_Annotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new LocalVariable("abc", PrimitiveType.Number, initializer);
        Assert.Equal("local abc: number = 1", variable.Render());
    }

    [Fact]
    public void Renders_LocalVariable_Annotated_NoInitializer()
    {
        var type = new OptionalType(PrimitiveType.Number);
        var variable = new LocalVariable("abc", type, null);
        Assert.Equal("local abc: number?", variable.Render());
    }

    [Fact]
    public void Renders_LocalVariable_Unannotated()
    {
        var initializer = new NumberLiteral(1);
        var variable = new LocalVariable("abc", null, initializer);
        Assert.Equal("local abc = 1", variable.Render());
    }

    [Fact]
    public void Renders_LocalVariable_Unannotated_NoInitializer()
    {
        var variable = new LocalVariable("abc", null, null);
        Assert.Equal("local abc", variable.Render());
    }

    [Fact]
    public void Renders_OptionalType_RequiresParens()
    {
        Assert.Equal("(string | boolean)?", new OptionalType(new UnionType([PrimitiveType.String, PrimitiveType.Boolean])).Render());
    }

    [Fact]
    public void Renders_OptionalType()
    {
        Assert.Equal("number?", new OptionalType(PrimitiveType.Number).Render());
    }

    [Fact]
    public void Renders_UnionType()
    {
        Assert.Equal("number | string | boolean", new UnionType([PrimitiveType.Number, PrimitiveType.String, PrimitiveType.Boolean]).Render());
    }

    [Fact]
    public void Renders_IntersectionType()
    {
        Assert.Equal("number & string & boolean", new IntersectionType([PrimitiveType.Number, PrimitiveType.String, PrimitiveType.Boolean,]).Render());
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
        Assert.Equal(kind.ToString().ToLower(), new PrimitiveType(kind).Render());
    }

    [Fact]
    public void Renders_TypeName_Generic()
    {
        Assert.Equal("Id<number, boolean>", new TypeName("Id", [PrimitiveType.Number, PrimitiveType.Boolean,]).Render());
    }

    [Fact]
    public void Renders_TypeName()
    {
        Assert.Equal("Hello", new TypeName("Hello").Render());
    }
    
    [Fact]
    public void Renders_StringLiteralType()
    {
        Assert.Equal($"{RenderState.StringDelimiter}abc{RenderState.StringDelimiter}", new StringLiteralType("abc").Render());
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Renders_BooleanLiteralType(bool value, string expected)
    {
        Assert.Equal(expected, new BooleanLiteralType(value).Render());
    }

    [Fact]
    public void Renders_ParenthesizedType()
    {
        Assert.Equal("(number)", new ParenthesizedType(new PrimitiveType(PrimitiveTypeKind.Number)).Render());
    }

    [Fact]
    public void Renders_UnitType()
    {
        Assert.Equal("()", new UnitType().Render());
    }

    [Fact]
    public void Renders_Parenthesized()
    {
        Assert.Equal("(69)", new Parenthesized(new NumberLiteral(69)).Render());
    }

    [Fact]
    public void Renders_Identifier()
    {
        Assert.Equal("abc", new Identifier("abc").Render());
    }

    [Theory]
    [InlineData("+")]
    [InlineData("+=")]
    [InlineData("-")]
    [InlineData("-=")]
    [InlineData("=")]
    [InlineData("==")]
    [InlineData("~=")]
    [InlineData("and")]
    [InlineData("or")]
    public void Renders_BinaryOperator(string op)
    {
        var expression = new BinaryOperator(
            new NumberLiteral(1),
            op,
            new NumberLiteral(2)
        );

        Assert.Equal($"1 {op} 2", expression.Render());
    }

    [Fact]
    public void Renders_UnaryOperator()
    {
        var expression = new UnaryOperator("-", new NumberLiteral(1));
        Assert.Equal("-1", expression.Render());
    }

    [Fact]
    public void Renders_MultilineStringLiteral()
    {
        Assert.Equal("[[abc\ndef]]", new StringLiteral("abc\ndef").Render());
    }

    [Fact]
    public void Renders_StringLiteral()
    {
        Assert.Equal($"{RenderState.StringDelimiter}abc{RenderState.StringDelimiter}", new StringLiteral("abc").Render());
    }

    [Theory]
    [InlineData(12, "12")]
    [InlineData(69.420, "69.42")]
    [InlineData(1e3, "1000")]
    [InlineData(0xFF, "255")]
    public void Renders_NumberLiteral(double value, string expected)
    {
        Assert.Equal(expected, new NumberLiteral(value).Render());
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Renders_BooleanLiteral(bool value, string expected)
    {
        Assert.Equal(expected, new BooleanLiteral(value).Render());
    }

    [Fact]
    public void Renders_NilLiteral()
    {
        Assert.Equal("nil", new NilLiteral().Render());
    }
}