using Loom.Core.Debug;
using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using PrimitiveTypeKind = Loom.Core.TypeChecking.Types.PrimitiveTypeKind;

namespace Loom.Testing;

[Collection("Assembly")]
public class ParserTest
{
    public static readonly List<object[]> ErrorTestCases =
    [
        ["let", InternalCodes.UnexpectedEof, "Expected identifier, got EOF."],
        ["let x:", InternalCodes.UnexpectedEof, "Expected type, got EOF."],
        ["!", InternalCodes.UnexpectedEof, "Unexpected end of file."],
        ["if )", InternalCodes.UnexpectedToken, "Expected expression, got ')'."],
        ["nameof(123)", InternalCodes.InvalidNameOf, "'123' is not a valid name."],
        ["(1 + 2", InternalCodes.UnexpectedEof, "Expected ')' here to close '(' at character 0, got EOF."],
        ["(1 + 2]", InternalCodes.UnexpectedToken, "Expected ')' here to close '(' at character 0, got ']'."],
        ["arr[0", InternalCodes.UnexpectedEof, "Expected ']', got EOF."],
        ["1 = 1", InternalCodes.InvalidAssignmentTarget, "Invalid assignment target."],
        ["a ? b : c = d", InternalCodes.InvalidAssignmentTarget, "Invalid assignment target."],
        ["fn foo", InternalCodes.MissingFunctionBody, "Expected function body, got EOF."],
        ["if true let x = 42", InternalCodes.DeclarationOutsideOfBlock, "Declarations can only be declared inside of a block.", "surround with '{' and '}'"],
        [
            "if true { return 1 } else let x = 42",
            InternalCodes.DeclarationOutsideOfBlock,
            "Declarations can only be declared inside of a block.",
            "surround with '{' and '}'"
        ],
        ["while true let x = 1", InternalCodes.DeclarationOutsideOfBlock, "Declarations can only be declared inside of a block.", "surround with '{' and '}'"],
        ["after 10ms let x = 1", InternalCodes.DeclarationOutsideOfBlock, "Declarations can only be declared inside of a block."],
        ["declare fn foo(a: number)", InternalCodes.MissingDeclareFnReturnType, "Declared function signatures must have a return type."],
        [
            "declare fn foo(a: number = 5): void",
            InternalCodes.UseOfDeclareFnParameterDefaults,
            "Parameters may not have default values in declared function signatures."
        ],
        ["declare fn foo(a): void", InternalCodes.MissingDeclareFnParameterType, "Parameters must have types in declared function signatures."],
        ["declare 123", InternalCodes.ExpectedDeclarationSignature, "Expected declaration signature, got '123'."],
        ["type Fn = fn(number)", InternalCodes.MissingDeclareFnReturnType, "Function types must have a return type."],
        ["type Fn = fn(x: number = 5): number", InternalCodes.UseOfDeclareFnParameterDefaults, "Parameters may not have default values in function types."],
        ["type Fn = fn(x): number", InternalCodes.MissingDeclareFnParameterType, "Parameters must have types in function types."],
        ["type F = (fn())", InternalCodes.MissingDeclareFnReturnType, "Function types must have a return type."],
        ["interface I { name }", InternalCodes.ExpectedInterfaceMemberType, "Expected indexer type, got '}'."],
        ["interface I { [int] }", InternalCodes.ExpectedInterfaceMemberType, "Expected indexer type, got '}'."],
        ["interface { }", InternalCodes.UnexpectedToken, "Expected interface name, got '{'."],
        ["interface I { 123 }", InternalCodes.UnexpectedToken, "Expected property name, got '123'."],
        ["after { }", InternalCodes.UnexpectedToken, "Expected expression, got '{'."],
        ["after 5s", InternalCodes.UnexpectedEof, "Unexpected end of file."],
        ["for : items { }", InternalCodes.UnexpectedToken, "Expected identifier, got ':'."],
        ["for x items { }", InternalCodes.UnexpectedToken, "Expected ':', got 'items'."],
        ["fn a<>() { }", InternalCodes.UnexpectedToken, "Expected type parameter name, got '>'."],
        ["fn a<T(x: T) { }", InternalCodes.UnexpectedToken, "Expected '>', got '('."],
        ["let x: List<number, string", InternalCodes.UnexpectedEof, "Expected '>', got EOF."],
        ["let x: List<number", InternalCodes.UnexpectedEof, "Expected '>', got EOF."],
        ["let x: List<>", InternalCodes.UnexpectedToken, "Expected type, got '>'."],
        ["let x: List<number,>", InternalCodes.UnexpectedToken, "Expected type, got '>'."],
        ["let x: List<number>>", InternalCodes.UnexpectedToken, "Expected expression, got '>'."],
        ["let x: number |", InternalCodes.UnexpectedEof, "Expected type, got EOF."],
        ["let x: number &", InternalCodes.UnexpectedEof, "Expected type, got EOF."],
        ["let x: (number", InternalCodes.UnexpectedEof, "Expected ')' here to close '(' at character 7, got EOF."],
        ["let x: keyof(T | number)", InternalCodes.UnexpectedToken, "Expected ')', got '|'."],
        ["trait { }", InternalCodes.UnexpectedToken, "Expected trait name, got '{'."],
        ["trait Foo { fn bar() }", InternalCodes.MissingDeclareFnReturnType, "Declared function signatures must have a return type."],
        ["trait Foo { fn bar(x): void }", InternalCodes.MissingDeclareFnParameterType, "Parameters must have types in declared function signatures."],
        [
            "trait Foo { fn bar(x: number = 5): void }",
            InternalCodes.UseOfDeclareFnParameterDefaults,
            "Parameters may not have default values in declared function signatures."
        ],
        ["implement", InternalCodes.UnexpectedEof, "Expected trait name, got EOF."],
        ["implement Foo", InternalCodes.UnexpectedEof, "Expected 'for', got EOF."],
        ["implement Foo for", InternalCodes.UnexpectedEof, "Expected interface name, got EOF."],
        ["implement Foo for Bar", InternalCodes.UnexpectedEof, "Expected '{', got EOF."],
        ["implement Foo for Bar {", InternalCodes.UnexpectedEof, "Expected '}', got EOF."],
        ["implement Foo for Bar { fn }", InternalCodes.UnexpectedToken, "Expected function name, got '}'."],
        ["implement 123 for Bar { }", InternalCodes.UnexpectedToken, "Expected trait name, got '123'."],
        ["implement Foo for Bar<T> { }", InternalCodes.UnexpectedToken, "Expected '{', got '<'."],
        ["implement Foo 123 Bar { }", InternalCodes.UnexpectedToken, "Expected 'for', got '123'."],
        ["implement Foo for 123 { }", InternalCodes.UnexpectedToken, "Expected interface name, got '123'."],
        ["nameof::<number>()", InternalCodes.InvalidTypeArguments, "May only get name of type when the type is a type name."],
        ["nameof::<T>(1)", InternalCodes.UnexpectedToken, "Expected ')', got '1'."],
        ["nameof::<T, U>()", InternalCodes.GenericArity, "Exactly one type parameter is allowed for 'nameof::<T>()'."],
        ["match", InternalCodes.UnexpectedEof, "Unexpected end of file."],
        ["match x", InternalCodes.UnexpectedEof, "Expected '{', got EOF."],
        ["match x {", InternalCodes.UnexpectedEof, "Expected '}', got EOF."],
        ["match x { _ ", InternalCodes.UnexpectedEof, "Expected '->', got EOF."],
        ["match x { _ -> }", InternalCodes.UnexpectedToken, "Expected expression, got '}'."],
        ["match x { -> 1 }", InternalCodes.UnexpectedToken, "Expected pattern, got '->'."],
        ["match x { { 123: y } -> 1 }", InternalCodes.UnexpectedToken, "Expected property name, got '123'."],
        ["match x { 1 | -> 2 }", InternalCodes.UnexpectedToken, "Expected pattern, got '->'."],
        ["match x { 0.. -> 1 }", InternalCodes.UnexpectedToken, "Expected range end, got '->'."],
        ["match x { let -> 1 }", InternalCodes.UnexpectedToken, "Expected binding name, got '->'."],
        ["match x { name when -> 1 }", InternalCodes.UnexpectedToken, "Expected type, got '->'."],
        ["match x { _ when -> 1 }", InternalCodes.UnexpectedToken, "Expected expression, got '->'."],
    ];

    public static readonly IEnumerable<object[]> SnapshotFiles = Utility.GetSnapshotFiles("AST", ".ast");

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
    public void Parses_MatchExpression_WildcardArm()
    {
        var tree = Utility.GetAST("match x { _ -> 0 }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var match = Assert.IsType<MatchExpression>(expressionStatement.Expression);

        Assert.Equal("x", Assert.IsType<Identifier>(match.Expression).Name.Text);
        var arm = Assert.Single(match.Arms);
        Assert.IsType<WildcardPattern>(arm.Pattern);
        Assert.Equal(0L, Assert.IsType<Literal>(arm.Body).Value);
    }

    [Fact]
    public void Parses_MatchExpression_ObjectPatternWithBindings()
    {
        var tree = Utility.GetAST("match result { { ok: true, value: v } -> v }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var match = Assert.IsType<MatchExpression>(expressionStatement.Expression);
        var arm = Assert.Single(match.Arms);
        var pattern = Assert.IsType<ObjectPattern>(arm.Pattern);

        Assert.Equal(2, pattern.Fields.Count);
        Assert.Equal("ok", pattern.Fields[0].Name.Text);
        Assert.Equal(true, Assert.IsType<LiteralPattern>(pattern.Fields[0].Pattern).Value);
        Assert.Equal("value", pattern.Fields[1].Name.Text);
        Assert.Equal("v", Assert.IsType<IdentifierPattern>(pattern.Fields[1].Pattern).Name.Text);
        Assert.Equal("v", Assert.IsType<Identifier>(arm.Body).Name.Text);
    }

    [Fact]
    public void Parses_MatchExpression_ObjectPatternShorthand()
    {
        var tree = Utility.GetAST("match value { { value } -> value }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var match = Assert.IsType<MatchExpression>(expressionStatement.Expression);
        var field = Assert.Single(Assert.IsType<ObjectPattern>(Assert.Single(match.Arms).Pattern).Fields);

        Assert.Equal("value", field.Name.Text);
        Assert.Null(field.Colon);
        Assert.Equal("value", Assert.IsType<IdentifierPattern>(field.Pattern).Name.Text);
    }

    [Fact]
    public void Parses_MatchExpression_AsVariableInitializer()
    {
        var tree = Utility.GetAST("let x = match n { 0 -> \"zero\", _ -> \"other\" }");
        var declaration = Assert.IsType<VariableDeclaration>(Assert.Single(tree.Statements));
        var match = Assert.IsType<MatchExpression>(declaration.EqualsValueClause!.Value);

        Assert.Equal(2, match.Arms.Count);
        Assert.Equal(0L, Assert.IsType<LiteralPattern>(match.Arms[0].Pattern).Value);
        Assert.IsType<WildcardPattern>(match.Arms[1].Pattern);
    }

    [Fact]
    public void Parses_MatchExpression_LiteralAndIdentifierArms()
    {
        var tree = Utility.GetAST("""match tag { "lava" -> 1, other -> 2 }""");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var match = Assert.IsType<MatchExpression>(expressionStatement.Expression);

        Assert.Equal(2, match.Arms.Count);
        Assert.Equal("lava", Assert.IsType<LiteralPattern>(match.Arms[0].Pattern).Value);
        Assert.Equal("other", Assert.IsType<IdentifierPattern>(match.Arms[1].Pattern).Name.Text);
    }

    [Fact]
    public void Parses_MatchExpression_OrPattern()
    {
        var tree = Utility.GetAST("match n { 2 | 3 | 4 -> true }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);
        var pattern = Assert.IsType<OrPattern>(arm.Pattern);

        Assert.Equal(3, pattern.Patterns.Count);
        Assert.Equal(2, pattern.Pipes.Count);
        Assert.Equal(2L, Assert.IsType<LiteralPattern>(pattern.Patterns[0]).Value);
        Assert.Equal(3L, Assert.IsType<LiteralPattern>(pattern.Patterns[1]).Value);
        Assert.Equal(4L, Assert.IsType<LiteralPattern>(pattern.Patterns[2]).Value);
    }

    [Fact]
    public void Parses_MatchExpression_RangePatternWithAlternation()
    {
        var tree = Utility.GetAST("match n { 0..5 | 10..15 | 100 -> true }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);
        var pattern = Assert.IsType<OrPattern>(arm.Pattern);

        Assert.Equal(3, pattern.Patterns.Count);
        var first = Assert.IsType<RangePattern>(pattern.Patterns[0]);
        Assert.Equal(0L, Assert.IsType<LiteralPattern>(first.Minimum).Value);
        Assert.Equal(5L, Assert.IsType<LiteralPattern>(first.Maximum).Value);
        var second = Assert.IsType<RangePattern>(pattern.Patterns[1]);
        Assert.Equal(10L, Assert.IsType<LiteralPattern>(second.Minimum).Value);
        Assert.Equal(15L, Assert.IsType<LiteralPattern>(second.Maximum).Value);
        Assert.Equal(100L, Assert.IsType<LiteralPattern>(pattern.Patterns[2]).Value);
    }

    [Fact]
    public void Parses_MatchExpression_TypePatternWithTypedBinding()
    {
        var tree = Utility.GetAST("match value { Foo { some_field: name when string } -> name }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);
        var typePattern = Assert.IsType<TypePattern>(arm.Pattern);

        Assert.Equal("Foo", Assert.IsType<TypeName>(typePattern.Type).Name.Text);
        Assert.NotNull(typePattern.ObjectPattern);
        var field = Assert.Single(typePattern.ObjectPattern.Fields);
        Assert.Equal("some_field", field.Name.Text);
        var typed = Assert.IsType<TypedPattern>(field.Pattern);
        Assert.Equal("name", typed.Name.Text);
        Assert.Equal(PrimitiveTypeKind.String, Assert.IsType<PrimitiveType>(typed.Type).Kind);
    }

    [Fact]
    public void Parses_MatchExpression_TypedCastPattern_Primitive()
    {
        var tree = Utility.GetAST("match value { s when string -> s }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);
        var typed = Assert.IsType<TypedPattern>(arm.Pattern);

        Assert.Equal("s", typed.Name.Text);
        Assert.Equal(PrimitiveTypeKind.String, Assert.IsType<PrimitiveType>(typed.Type).Kind);
        Assert.Null(typed.ObjectPattern);
        Assert.Null(arm.Guard);
        Assert.Equal("s", Assert.IsType<Identifier>(arm.Body).Name.Text);
    }

    [Fact]
    public void Parses_MatchExpression_TypedCastPattern_WithObjectPattern()
    {
        var tree = Utility.GetAST("match value { f when Foo { some_field: let n, other: name when string } -> n }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);
        var typed = Assert.IsType<TypedPattern>(arm.Pattern);

        Assert.Equal("f", typed.Name.Text);
        Assert.Equal("Foo", Assert.IsType<TypeName>(typed.Type).Name.Text);
        Assert.NotNull(typed.ObjectPattern);
        Assert.Equal(2, typed.ObjectPattern.Fields.Count);

        Assert.Equal("some_field", typed.ObjectPattern.Fields[0].Name.Text);
        Assert.Equal("n", Assert.IsType<LetPattern>(typed.ObjectPattern.Fields[0].Pattern).Name.Text);

        Assert.Equal("other", typed.ObjectPattern.Fields[1].Name.Text);
        var nestedTyped = Assert.IsType<TypedPattern>(typed.ObjectPattern.Fields[1].Pattern);
        Assert.Equal("name", nestedTyped.Name.Text);
        Assert.Equal(PrimitiveTypeKind.String, Assert.IsType<PrimitiveType>(nestedTyped.Type).Kind);
    }

    [Fact]
    public void Parses_MatchExpression_TypedCastPattern_WithGuard()
    {
        var tree = Utility.GetAST("match value { f when Foo { some_field: let n } when n > 0 -> n }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);
        var typed = Assert.IsType<TypedPattern>(arm.Pattern);

        Assert.Equal("f", typed.Name.Text);
        Assert.Equal("Foo", Assert.IsType<TypeName>(typed.Type).Name.Text);
        Assert.NotNull(typed.ObjectPattern);
        Assert.NotNull(arm.Guard);
        var guard = Assert.IsType<BinaryOperator>(arm.Guard);
        Assert.Equal("n", Assert.IsType<Identifier>(guard.Left).Name.Text);
        Assert.Equal(0L, Assert.IsType<Literal>(guard.Right).Value);
    }

    [Fact]
    public void Parses_MatchExpression_TypePatternWithLetBinding()
    {
        var tree = Utility.GetAST("match value { Foo { some_field: let name } -> name }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);
        var typePattern = Assert.IsType<TypePattern>(arm.Pattern);

        Assert.NotNull(typePattern.ObjectPattern);
        var field = Assert.Single(typePattern.ObjectPattern.Fields);
        var letPattern = Assert.IsType<LetPattern>(field.Pattern);
        Assert.Equal("name", letPattern.Name.Text);
    }

    [Fact]
    public void Parses_MatchExpression_WhenGuard()
    {
        var tree = Utility.GetAST("match n { x when x > 0 -> x }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var arm = Assert.Single(Assert.IsType<MatchExpression>(expressionStatement.Expression).Arms);

        Assert.Equal("x", Assert.IsType<IdentifierPattern>(arm.Pattern).Name.Text);
        Assert.NotNull(arm.Guard);
        var guard = Assert.IsType<BinaryOperator>(arm.Guard);
        Assert.Equal("x", Assert.IsType<Identifier>(guard.Left).Name.Text);
        Assert.Equal(0L, Assert.IsType<Literal>(guard.Right).Value);
    }

    [Fact]
    public void Match_InvalidPattern_ProducesNullPattern()
    {
        var tree = Utility.GetAST("match x { -> 1 }");
        var expressionStatement = Assert.IsType<ExpressionStatement>(Assert.Single(tree.Statements));
        var match = Assert.IsType<MatchExpression>(expressionStatement.Expression);
        Assert.IsType<NullPattern>(Assert.Single(match.Arms).Pattern);
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