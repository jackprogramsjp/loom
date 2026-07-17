using Loom.Core.Debug;
using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;

namespace Loom.Testing;

[Collection("Assembly")]
public class ParserTest
{
    public static readonly IEnumerable<TheoryDataRow<string, string, string, string?>> ErrorTestCases =
    [
        new("let", InternalCodes.UnexpectedEof, "Expected identifier, got EOF.", null),
        new("let x:", InternalCodes.UnexpectedEof, "Expected type, got EOF.", null),
        new("!", InternalCodes.UnexpectedEof, "Unexpected end of file.", null),
        new("if )", InternalCodes.UnexpectedToken, "Expected expression, got ')'.", null),
        new("nameof(123)", InternalCodes.InvalidNameOf, "'123' is not a valid name.", null),
        new("(1 + 2", InternalCodes.UnexpectedEof, "Expected ')' here to close '(' at character 0, got EOF.", null),
        new("(1 + 2]", InternalCodes.UnexpectedToken, "Expected ')' here to close '(' at character 0, got ']'.", null),
        new("arr[0", InternalCodes.UnexpectedEof, "Expected ']', got EOF.", null),
        new("1 = 1", InternalCodes.InvalidAssignmentTarget, "Invalid assignment target.", null),
        new("a ? b : c = d", InternalCodes.InvalidAssignmentTarget, "Invalid assignment target.", null),
        new("fn foo", InternalCodes.MissingFunctionBody, "Expected function body, got EOF.", null),
        new("if true let x = 42", InternalCodes.DeclarationOutsideOfBlock, "Declarations can only be declared inside of a block.", "surround with '{' and '}'"),
        new(
            "if true { return 1 } else let x = 42",
            InternalCodes.DeclarationOutsideOfBlock,
            "Declarations can only be declared inside of a block.",
            "surround with '{' and '}'"
        ),
        new("while true let x = 1", InternalCodes.DeclarationOutsideOfBlock, "Declarations can only be declared inside of a block.", "surround with '{' and '}'"),
        new("after 10ms let x = 1", InternalCodes.DeclarationOutsideOfBlock, "Declarations can only be declared inside of a block.", null),
        new("declare fn foo(a: number)", InternalCodes.MissingDeclareFnReturnType, "Declared function signatures must have a return type.", null),
        new(
            "declare fn foo(a: number = 5): void",
            InternalCodes.UseOfDeclareFnParameterDefaults,
            "Parameters may not have default values in declared function signatures.",
            null
        ),
        new("declare fn foo(a): void", InternalCodes.MissingDeclareFnParameterType, "Parameters must have types in declared function signatures.", null),
        new("declare 123", InternalCodes.ExpectedDeclarationSignature, "Expected declaration signature, got '123'.", null),
        new("type Fn = fn(number)", InternalCodes.MissingDeclareFnReturnType, "Function types must have a return type.", null),
        new("type Fn = fn(x: number = 5): number", InternalCodes.UseOfDeclareFnParameterDefaults, "Parameters may not have default values in function types.", null),
        new("type Fn = fn(x): number", InternalCodes.MissingDeclareFnParameterType, "Parameters must have types in function types.", null),
        new("type F = (fn())", InternalCodes.MissingDeclareFnReturnType, "Function types must have a return type.", null),
        new("interface I { name }", InternalCodes.ExpectedInterfaceMemberType, "Expected indexer type, got '}'.", null),
        new("interface I { [int] }", InternalCodes.ExpectedInterfaceMemberType, "Expected indexer type, got '}'.", null),
        new("interface { }", InternalCodes.UnexpectedToken, "Expected interface name, got '{'.", null),
        new("interface I { 123 }", InternalCodes.UnexpectedToken, "Expected property name, got '123'.", null),
        new("after { }", InternalCodes.UnexpectedToken, "Expected expression, got '{'.", null),
        new("after 5s", InternalCodes.UnexpectedEof, "Unexpected end of file.", null),
        new("for : items { }", InternalCodes.UnexpectedToken, "Expected identifier, got ':'.", null),
        new("for x items { }", InternalCodes.UnexpectedToken, "Expected ':', got 'items'.", null),
        new("fn a<>() { }", InternalCodes.UnexpectedToken, "Expected type parameter name, got '>'.", null),
        new("fn a<T(x: T) { }", InternalCodes.UnexpectedToken, "Expected '>', got '('.", null),
        new("let x: List<number, string", InternalCodes.UnexpectedEof, "Expected '>', got EOF.", null),
        new("let x: List<number", InternalCodes.UnexpectedEof, "Expected '>', got EOF.", null),
        new("let x: List<>", InternalCodes.UnexpectedToken, "Expected type, got '>'.", null),
        new("let x: List<number,>", InternalCodes.UnexpectedToken, "Expected type, got '>'.", null),
        new("let x: List<number>>", InternalCodes.UnexpectedToken, "Expected expression, got '>'.", null),
        new("let x: number |", InternalCodes.UnexpectedEof, "Expected type, got EOF.", null),
        new("let x: number &", InternalCodes.UnexpectedEof, "Expected type, got EOF.", null),
        new("let x: (number", InternalCodes.UnexpectedEof, "Expected ')' here to close '(' at character 7, got EOF.", null),
        new("let x: keyof(T | number)", InternalCodes.UnexpectedToken, "Expected ')', got '|'.", null),
        new("trait { }", InternalCodes.UnexpectedToken, "Expected trait name, got '{'.", null),
        new("trait Foo { fn bar() }", InternalCodes.MissingDeclareFnReturnType, "Declared function signatures must have a return type.", null),
        new("trait Foo { fn bar(x): void }", InternalCodes.MissingDeclareFnParameterType, "Parameters must have types in declared function signatures.", null),
        new(
            "trait Foo { fn bar(x: number = 5): void }",
            InternalCodes.UseOfDeclareFnParameterDefaults,
            "Parameters may not have default values in declared function signatures.",
            null
        ),
        new("implement", InternalCodes.UnexpectedEof, "Expected trait name, got EOF.", null),
        new("implement Foo", InternalCodes.UnexpectedEof, "Expected 'for', got EOF.", null),
        new("implement Foo for", InternalCodes.UnexpectedEof, "Expected interface name, got EOF.", null),
        new("implement Foo for Bar", InternalCodes.UnexpectedEof, "Expected '{', got EOF.", null),
        new("implement Foo for Bar {", InternalCodes.UnexpectedEof, "Expected '}', got EOF.", null),
        new("implement Foo for Bar { fn }", InternalCodes.UnexpectedToken, "Expected function name, got '}'.", null),
        new("implement 123 for Bar { }", InternalCodes.UnexpectedToken, "Expected trait name, got '123'.", null),
        new("implement Foo for Bar<T> { }", InternalCodes.UnexpectedToken, "Expected '{', got '<'.", null),
        new("implement Foo 123 Bar { }", InternalCodes.UnexpectedToken, "Expected 'for', got '123'.", null),
        new("implement Foo for 123 { }", InternalCodes.UnexpectedToken, "Expected interface name, got '123'.", null),
        new("nameof::<number>()", InternalCodes.InvalidTypeArguments, "May only get name of type when the type is a type name.", null),
        new("nameof::<T>(1)", InternalCodes.UnexpectedToken, "Expected ')', got '1'.", null),
        new("nameof::<T, U>()", InternalCodes.GenericArity, "Exactly one type parameter is allowed for 'nameof::<T>()'.", null)
    ];

    public static readonly IEnumerable<TheoryDataRow<string, string>> SnapshotFiles = Utility.GetSnapshotFiles("AST", ".ast");

    [Theory]
    [MemberData(nameof(ErrorTestCases))]
    public void Parser_ErrorDiagnostics(string source, string code, string expectedMessage, string? hint = null)
    {
        var diagnostics = Utility.GetParserDiagnostics(source);
        Utility.AssertDiagnostic(diagnostics, code, expectedMessage, hint);
    }

    [Theory]
    [MemberData(nameof(SnapshotFiles))]
    public void Parser_Snapshots(string sourcePath, string snapshotPath)
    {
        var source = File.ReadAllText(sourcePath);
        var result = Utility.Parse(source);
        Utility.AssertNoErrors(result);

        var actual = AstInspector.Inspect(result.Tree).Replace(Environment.NewLine, "\n");
        var expected = File.ReadAllText(snapshotPath).Replace(Environment.NewLine, "\n");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Unfinished_ProducesNull()
    {
        var tree = Utility.GetAST("let");
        Assert.Single(tree.Statements);
        var declaration = Assert.IsType<VariableDeclaration>(tree.Statements.First());
        Assert.Null(declaration.ColonTypeClause);
        Assert.Null(declaration.EqualsValueClause);
    }

    [Fact]
    public void Error_ProducesNullExpression()
    {
        var tree = Utility.GetAST("=");
        Assert.Single(tree.Statements);

        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.First());
        Assert.IsType<NullExpression>(expressionStatement.Expression);
    }

    [Fact]
    public void Error_ProducesNullTypeExpression()
    {
        var tree = Utility.GetAST("type X = fn(a = 69): void");
        Assert.Single(tree.Statements);

        var alias = Assert.IsType<TypeAlias>(tree.Statements.First());
        Assert.IsType<NullTypeExpression>(alias.EqualsTypeClause.Type);
    }

    [Fact]
    public void Error_ProducesNullStatement()
    {
        var tree = Utility.GetAST("if x let y = 1");
        Assert.Single(tree.Statements);

        var @if = Assert.IsType<If>(tree.Statements.First());
        Assert.IsType<NullStatement>(@if.ThenBranch);
    }

    [Theory]
    [InlineData("123", 123L)]
    [InlineData("1e3", 100L)]
    [InlineData("0x100", 256L)]
    [InlineData("0xFF_FF", 65535L)]
    [InlineData("0Xf0D", 3853L)]
    [InlineData("0b011100110", 230L)]
    [InlineData("0b01110_0110", 230L)]
    [InlineData("0B11001", 25L)]
    [InlineData("0o400", 256L)]
    [InlineData("0O2340", 1248L)]
    [InlineData("1.23e3", 1230L)]
    [InlineData("420.69", 420.69d)]
    [InlineData("1.2345e3", 1234.5)]
    [InlineData("1_0_0________.6_9e5_1", long.MaxValue)]
    [InlineData("5s", 5L)]
    [InlineData("500ms", 0.5)]
    [InlineData("20hz", 0.05)]
    [InlineData("2_0.4_5ms", 0.02045)]
    [InlineData("0.5m", 30L)]
    [InlineData("20m", 1800L)]
    [InlineData("2h", 7200L)]
    [InlineData("'hello'", "hello")]
    [InlineData("\"abc\"", "abc")]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("none", null)]
    public void Parses_Literals(string source, object? expectedValue)
    {
        var tree = Utility.GetAST(source);
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var literal = Assert.IsType<Literal>(expressionStatement.Expression);
        Assert.Equal(source, literal.Token.Text);
        if (expectedValue != null)
            Assert.IsType(expectedValue.GetType(), literal.Value);
        else
            Assert.Null(literal.Value);
    }
}