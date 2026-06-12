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
    public void Renders_IfStatement()
    {
        Assert.Equal(
            "if a then\n  x = 1\nelseif b then\n  x = 2\nelseif c then\n  x = 3\nelse\n  x = 69\nend",
            new IfStatement(
                new Identifier("a"),
                new Chunk([setX(1)]),
                [new ElseIfBranch(new Identifier("b"), new Chunk([setX(2)])), new ElseIfBranch(new Identifier("c"), new Chunk([setX(3)]))],
                new Chunk([setX(69)])
            ).Render()
        );

        return;
        ExpressionStatement setX(int x) => new(new BinaryOperator(new Identifier("x"), "=", new NumberLiteral(x)));
    }

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
    public void Renders_Function()
    {
        var parameters = new List<Parameter> { new("a", PrimitiveType.Number), new("b", new TypeName("T")) };
        var typeParameter = new TypeParameter("T", PrimitiveType.Number) { OfFunction = true };
        var fn = new Function(
            "add",
            new TypeParameters([typeParameter]),
            parameters,
            PrimitiveType.Number,
            new Chunk([new Return(new BinaryOperator(new Identifier("a"), "+", new Identifier("b")))])
        );

        Assert.Equal("const function add<T>(a: number, b: T): number\n  return a + b\nend", fn.Render());
    }

    [Fact]
    public void Renders_Long_ComputedProperty_Table()
    {
        var table = new Table(
            [
                new ComputedPropertyTableInitializer(new StringLiteral("a"), new NumberLiteral(1)),
                new ComputedPropertyTableInitializer(new StringLiteral("b"), new NumberLiteral(2)),
                new ComputedPropertyTableInitializer(new StringLiteral("c"), new NumberLiteral(3)),
                new ComputedPropertyTableInitializer(new StringLiteral("d"), new NumberLiteral(4)),
                new ComputedPropertyTableInitializer(new StringLiteral("e"), new NumberLiteral(5)),
                new ComputedPropertyTableInitializer(new StringLiteral("f"), new NumberLiteral(6))
            ]
        );

        Assert.Equal("{\n  [\"a\"] = 1,\n  [\"b\"] = 2,\n  [\"c\"] = 3,\n  [\"d\"] = 4,\n  [\"e\"] = 5,\n  [\"f\"] = 6,\n}", table.Render());
    }

    [Fact]
    public void Renders_Short_ComputedProperty_Table()
    {
        var table = new Table(
            [
                new ComputedPropertyTableInitializer(new StringLiteral("foo"), new NumberLiteral(69)),
                new ComputedPropertyTableInitializer(new StringLiteral("bar"), new NumberLiteral(420))
            ]
        );

        Assert.Equal("{ [\"foo\"] = 69, [\"bar\"] = 420 }", table.Render());
    }

    [Fact]
    public void Renders_Long_Property_Table()
    {
        var table = new Table(
            [
                new PropertyTableInitializer("a", new NumberLiteral(1)),
                new PropertyTableInitializer("b", new NumberLiteral(2)),
                new PropertyTableInitializer("c", new NumberLiteral(3)),
                new PropertyTableInitializer("d", new NumberLiteral(4)),
                new PropertyTableInitializer("e", new NumberLiteral(5)),
                new PropertyTableInitializer("f", new NumberLiteral(6))
            ]
        );

        Assert.Equal("{\n  a = 1,\n  b = 2,\n  c = 3,\n  d = 4,\n  e = 5,\n  f = 6,\n}", table.Render());
    }

    [Fact]
    public void Renders_Short_Property_Table()
    {
        var table = new Table([new PropertyTableInitializer("foo", new NumberLiteral(69)), new PropertyTableInitializer("bar", new NumberLiteral(420))]);
        Assert.Equal("{ foo = 69, bar = 420 }", table.Render());
    }

    [Fact]
    public void Renders_Nested_Long_Array_Table()
    {
        var innerTable = new Table(
            new List<int>
            {
                1,
                2,
                3,
                4,
                5,
                6
            }.ConvertAll(i => new TableInitializer(new NumberLiteral(i)))
        );

        var table = new Table(
            [..innerTable.Initializers, new TableInitializer(new Table(innerTable.Initializers.Take(3).ToList())), new TableInitializer(innerTable)]
        );

        Assert.Equal("{\n  1,\n  2,\n  3,\n  4,\n  5,\n  6,\n  {1, 2, 3},\n  {\n    1,\n    2,\n    3,\n    4,\n    5,\n    6,\n  },\n}", table.Render());
    }

    [Fact]
    public void Renders_Long_Array_Table()
    {
        var table = new Table(
            new List<int>
            {
                1,
                2,
                3,
                4,
                5,
                6
            }.ConvertAll(i => new TableInitializer(new NumberLiteral(i)))
        );

        Assert.Equal("{\n  1,\n  2,\n  3,\n  4,\n  5,\n  6,\n}", table.Render());
    }

    [Fact]
    public void Renders_Short_Array_Table()
    {
        var table = new Table(new List<int> { 1, 2, 3 }.ConvertAll(i => new TableInitializer(new NumberLiteral(i))));
        Assert.Equal("{1, 2, 3}", table.Render());
    }

    [Fact]
    public void Renders_Empty_Table()
    {
        var table = new Table([]);
        Assert.Equal("{}", table.Render());
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
    public void Renders_ExpressionStatement()
    {
        Assert.Equal("1", new ExpressionStatement(new NumberLiteral(1)).Render());
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
        Assert.Equal("Id<number, boolean>", new TypeName("Id", [PrimitiveType.Number, PrimitiveType.Boolean]).Render());
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

    [Fact]
    public void Renders_Dictionary_TableType()
    {
        Assert.Equal("{ [string]: number }", new TableType(PrimitiveType.String, PrimitiveType.Number).Render());
    }

    [Fact]
    public void Renders_Array_TableType()
    {
        Assert.Equal("{ number }", new TableType(null, PrimitiveType.Number).Render());
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