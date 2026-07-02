using Loom.Debug;
using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Text;
using Loom.TypeChecking.Types;
using ArrayType = Loom.Parsing.AST.ArrayType;
using FunctionType = Loom.Parsing.AST.FunctionType;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using LiteralType = Loom.Parsing.AST.LiteralType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using TypeName = Loom.Parsing.AST.TypeName;
using UnionType = Loom.Parsing.AST.UnionType;

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
        ["let x: keyof(T | number)", InternalCodes.UnexpectedToken, "Expected ')', got '|'."]
    ];

    public static IEnumerable<object[]> SnapshotFiles =>
        Directory.EnumerateFiles(AssemblyFixture.Snapshots + "/AST", $"*{FileManager.LoomExtension}")
            .Select(path => new object[] { path, path.Replace(FileManager.LoomExtension, ".ast") });

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
        var tree = Utility.GetAST(source);
        var actual = AstInspector.Inspect(tree).Replace(Environment.NewLine, "\n");
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

    [Fact]
    public void Parses_KeyOf_Basic()
    {
        var tree = Utility.GetAST("let x: keyof(T)");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var keyOf = Assert.IsType<KeyOf>(varDecl.ColonTypeClause!.Type);
        Assert.Equal(SyntaxKind.KeyOfKeyword, keyOf.Keyword.Kind);
        Assert.Equal(SyntaxKind.LParen, keyOf.LeftParen.Kind);
        Assert.Equal(SyntaxKind.RParen, keyOf.RightParen.Kind);
        Assert.IsType<TypeName>(keyOf.Type);
        Assert.Equal("T", ((TypeName)keyOf.Type).Name.Text);
    }

    [Fact]
    public void Parses_KeyOf_Nested()
    {
        var tree = Utility.GetAST("let x: keyof(keyof(T))");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var outer = Assert.IsType<KeyOf>(varDecl.ColonTypeClause!.Type);
        var inner = Assert.IsType<KeyOf>(outer.Type);
        Assert.IsType<TypeName>(inner.Type);
        Assert.Equal("T", ((TypeName)inner.Type).Name.Text);
    }

    [Fact]
    public void Parses_KeyOf_WithPostfix()
    {
        var tree = Utility.GetAST("let x: keyof(T)[]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var array = Assert.IsType<ArrayType>(varDecl.ColonTypeClause!.Type);
        var keyOf = Assert.IsType<KeyOf>(array.ElementType);
        Assert.IsType<TypeName>(keyOf.Type);
    }

    [Fact]
    public void Parses_TernaryOperator_Basic()
    {
        var tree = Utility.GetAST("a ? b : c");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var ternary = Assert.IsType<TernaryOperator>(stmt.Expression);

        Assert.IsType<Identifier>(ternary.Condition);
        Assert.IsType<Identifier>(ternary.ThenBranch);
        Assert.IsType<Identifier>(ternary.ElseBranch);

        Assert.Equal("a", ((Identifier)ternary.Condition).Name.Text);
        Assert.Equal("b", ((Identifier)ternary.ThenBranch).Name.Text);
        Assert.Equal("c", ((Identifier)ternary.ElseBranch).Name.Text);
    }

    [Fact]
    public void Parses_TernaryOperator_WithComplexExpressions()
    {
        var tree = Utility.GetAST("a + b ? c * d : e / f");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var ternary = Assert.IsType<TernaryOperator>(stmt.Expression);
        var condition = Assert.IsType<BinaryOperator>(ternary.Condition);
        Assert.Equal(SyntaxKind.Plus, condition.Operator.Kind);
        Assert.IsType<Identifier>(condition.Left);
        Assert.IsType<Identifier>(condition.Right);

        var trueExpr = Assert.IsType<BinaryOperator>(ternary.ThenBranch);
        Assert.Equal(SyntaxKind.Star, trueExpr.Operator.Kind);
        Assert.IsType<Identifier>(trueExpr.Left);
        Assert.IsType<Identifier>(trueExpr.Right);

        var falseExpr = Assert.IsType<BinaryOperator>(ternary.ElseBranch);
        Assert.Equal(SyntaxKind.Slash, falseExpr.Operator.Kind);
        Assert.IsType<Identifier>(falseExpr.Left);
        Assert.IsType<Identifier>(falseExpr.Right);
    }

    [Fact]
    public void Parses_TernaryOperator_RightAssociative()
    {
        var tree = Utility.GetAST("a ? b : c ? d : e");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var outer = Assert.IsType<TernaryOperator>(stmt.Expression);

        Assert.Equal("a", ((Identifier)outer.Condition).Name.Text);
        Assert.Equal("b", ((Identifier)outer.ThenBranch).Name.Text);

        var inner = Assert.IsType<TernaryOperator>(outer.ElseBranch);
        Assert.Equal("c", ((Identifier)inner.Condition).Name.Text);
        Assert.Equal("d", ((Identifier)inner.ThenBranch).Name.Text);
        Assert.Equal("e", ((Identifier)inner.ElseBranch).Name.Text);
    }

    [Fact]
    public void Parses_TernaryOperator_WithParentheses()
    {
        var tree = Utility.GetAST("(a ? b : c) ? d : e");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var outer = Assert.IsType<TernaryOperator>(stmt.Expression);

        var parenCondition = Assert.IsType<Parenthesized>(outer.Condition);
        var inner = Assert.IsType<TernaryOperator>(parenCondition.Expression);
        Assert.Equal("a", ((Identifier)inner.Condition).Name.Text);
        Assert.Equal("b", ((Identifier)inner.ThenBranch).Name.Text);
        Assert.Equal("c", ((Identifier)inner.ElseBranch).Name.Text);

        Assert.Equal("d", ((Identifier)outer.ThenBranch).Name.Text);
        Assert.Equal("e", ((Identifier)outer.ElseBranch).Name.Text);
    }

    [Fact]
    public void Parses_TernaryOperator_WithAssignment()
    {
        var tree = Utility.GetAST("x = a ? b : c");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var assignment = Assert.IsType<AssignmentOperator>(stmt.Expression);
        Assert.Equal(SyntaxKind.Equals, assignment.Operator.Kind);

        var target = Assert.IsType<Identifier>(assignment.Left);
        Assert.Equal("x", target.Name.Text);

        var ternary = Assert.IsType<TernaryOperator>(assignment.Right);
        Assert.Equal("a", ((Identifier)ternary.Condition).Name.Text);
        Assert.Equal("b", ((Identifier)ternary.ThenBranch).Name.Text);
        Assert.Equal("c", ((Identifier)ternary.ElseBranch).Name.Text);
    }

    [Fact]
    public void Parses_TernaryOperator_WithAsKeyword()
    {
        var tree = Utility.GetAST("a ? b : c as number");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var ternary = Assert.IsType<TernaryOperator>(stmt.Expression);

        Assert.Equal("a", ((Identifier)ternary.Condition).Name.Text);
        Assert.Equal("b", ((Identifier)ternary.ThenBranch).Name.Text);

        var asExpr = Assert.IsType<AsExpression>(ternary.ElseBranch);
        Assert.IsType<Identifier>(asExpr.Expression);
        Assert.IsType<PrimitiveType>(asExpr.Type);
    }

    [Fact]
    public void Parses_TernaryOperator_WithNestedTernaryAndDifferentPrecedence()
    {
        var tree = Utility.GetAST("a ? b : c ? d : e ? f : g");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var outer = Assert.IsType<TernaryOperator>(stmt.Expression);

        Assert.Equal("a", ((Identifier)outer.Condition).Name.Text);
        Assert.Equal("b", ((Identifier)outer.ThenBranch).Name.Text);

        var middle = Assert.IsType<TernaryOperator>(outer.ElseBranch);
        Assert.Equal("c", ((Identifier)middle.Condition).Name.Text);
        Assert.Equal("d", ((Identifier)middle.ThenBranch).Name.Text);

        var inner = Assert.IsType<TernaryOperator>(middle.ElseBranch);
        Assert.Equal("e", ((Identifier)inner.Condition).Name.Text);
        Assert.Equal("f", ((Identifier)inner.ThenBranch).Name.Text);
        Assert.Equal("g", ((Identifier)inner.ElseBranch).Name.Text);
    }

    [Fact]
    public void Parses_TernaryOperator_WithLiterals()
    {
        var tree = Utility.GetAST("true ? 1 : 2");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var ternary = Assert.IsType<TernaryOperator>(stmt.Expression);

        var cond = Assert.IsType<Literal>(ternary.Condition);
        Assert.Equal(true, cond.Value);
        var trueVal = Assert.IsType<Literal>(ternary.ThenBranch);
        Assert.Equal(1L, trueVal.Value);
        var falseVal = Assert.IsType<Literal>(ternary.ElseBranch);
        Assert.Equal(2L, falseVal.Value);
    }

    [Fact]
    public void Parses_TernaryOperator_WithUnaryOperators()
    {
        var tree = Utility.GetAST("!a ? -b : ~c");
        Assert.Single(tree.Statements);

        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var ternary = Assert.IsType<TernaryOperator>(stmt.Expression);

        var cond = Assert.IsType<UnaryOperator>(ternary.Condition);
        Assert.Equal(SyntaxKind.Bang, cond.Operator.Kind);
        Assert.IsType<Identifier>(cond.Operand);

        var trueVal = Assert.IsType<UnaryOperator>(ternary.ThenBranch);
        Assert.Equal(SyntaxKind.Minus, trueVal.Operator.Kind);
        Assert.IsType<Identifier>(trueVal.Operand);

        var falseVal = Assert.IsType<UnaryOperator>(ternary.ElseBranch);
        Assert.Equal(SyntaxKind.Tilde, falseVal.Operator.Kind);
        Assert.IsType<Identifier>(falseVal.Operand);
    }

    [Fact]
    public void Parses_TernaryOperator_InsideFunctionCall()
    {
        var tree = Utility.GetAST("foo(a ? b : c)");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(stmt.Expression);
        Assert.Single(invocation.Arguments.ArgumentList);
        var ternary = Assert.IsType<TernaryOperator>(invocation.Arguments.ArgumentList[0]);
        Assert.Equal("a", ((Identifier)ternary.Condition).Name.Text);
        Assert.Equal("b", ((Identifier)ternary.ThenBranch).Name.Text);
        Assert.Equal("c", ((Identifier)ternary.ElseBranch).Name.Text);
    }

    [Fact]
    public void Parses_ForLoop_WithBlockBody()
    {
        var tree = Utility.GetAST("for x : items { print(x) }");
        Assert.Single(tree.Statements);
        var @for = Assert.IsType<For>(tree.Statements.First());
        Assert.Equal(SyntaxKind.ForKeyword, @for.Keyword.Kind);
        Assert.Single(@for.Names);

        Assert.Equal("x", @for.Names.First().Name.Text);
        Assert.Equal(SyntaxKind.Colon, @for.Colon.Kind);
        Assert.IsType<Identifier>(@for.CollectionExpression);
        var body = Assert.IsType<Block>(@for.Body);
        Assert.Single(body.Statements);
        Assert.IsType<ExpressionStatement>(body.Statements.First());
    }

    [Fact]
    public void Parses_ForLoop_WithExpressionBody()
    {
        var tree = Utility.GetAST("for x : items break");
        Assert.Single(tree.Statements);
        var @for = Assert.IsType<For>(tree.Statements.First());
        Assert.Equal(SyntaxKind.ForKeyword, @for.Keyword.Kind);
        Assert.Single(@for.Names);

        Assert.Equal("x", @for.Names.First().Name.Text);
        Assert.Equal(SyntaxKind.Colon, @for.Colon.Kind);
        Assert.IsType<Identifier>(@for.CollectionExpression);
        Assert.IsType<Break>(@for.Body);
    }

    [Fact]
    public void Parses_ForLoop_WithComplexExpression()
    {
        var tree = Utility.GetAST("for x : getItems() { }");
        var forStmt = Assert.IsType<For>(tree.Statements.First());
        var expr = Assert.IsType<Invocation>(forStmt.CollectionExpression);
        Assert.Equal("getItems", ((Identifier)expr.Expression).Name.Text);
    }

    [Fact]
    public void Parses_NestedForLoops()
    {
        var tree = Utility.GetAST("for x : xs { for y : ys { use(x, y) } }");
        var outer = Assert.IsType<For>(tree.Statements.First());
        var outerBody = Assert.IsType<Block>(outer.Body);
        var inner = Assert.IsType<For>(outerBody.Statements.First());
        var innerBody = Assert.IsType<Block>(inner.Body);
        Assert.IsType<ExpressionStatement>(innerBody.Statements.First());
    }

    [Fact]
    public void Parses_AfterStatement_WithExpressionBody()
    {
        var tree = Utility.GetAST("after 5s doSomething()");
        Assert.Single(tree.Statements);

        var after = Assert.IsType<After>(tree.Statements.First());
        var condition = Assert.IsType<Literal>(after.Duration);
        Assert.Equal(5L, condition.Value);

        var body = Assert.IsType<ExpressionStatement>(after.Body);
        var invocation = Assert.IsType<Invocation>(body.Expression);
        var ident = Assert.IsType<Identifier>(invocation.Expression);
        Assert.Equal("doSomething", ident.Name.Text);
    }

    [Fact]
    public void Parses_AfterStatement_WithNestedIf()
    {
        var tree = Utility.GetAST("after 2s { if condition { break } }");
        Assert.Single(tree.Statements);

        var after = Assert.IsType<After>(tree.Statements.First());
        var block = Assert.IsType<Block>(after.Body);
        Assert.Single(block.Statements);

        var ifStmt = Assert.IsType<If>(block.Statements.First());
        var thenBlock = Assert.IsType<Block>(ifStmt.ThenBranch);
        Assert.Single(thenBlock.Statements);
        Assert.IsType<Break>(thenBlock.Statements.First());
    }

    [Fact]
    public void Parses_AfterBody_WithDeclarationInsideBlock()
    {
        var tree = Utility.GetAST("after 10ms { let x = 1 }");
        var after = Assert.IsType<After>(tree.Statements.First());
        var block = Assert.IsType<Block>(after.Body);
        Assert.IsType<VariableDeclaration>(block.Statements.First());
    }

    [Fact]
    public void Parses_AfterStatement_InsideIfElse()
    {
        var tree = Utility.GetAST("if true { after 1s foo() } else { after 2s bar() }");
        var ifStmt = Assert.IsType<If>(tree.Statements.First());
        var thenBlock = Assert.IsType<Block>(ifStmt.ThenBranch);
        Assert.IsType<After>(thenBlock.Statements.First());
        var elseBlock = Assert.IsType<Block>(ifStmt.ElseBranch!.Branch);
        Assert.IsType<After>(elseBlock.Statements.First());
    }

    [Fact]
    public void Parses_AfterStatement_WithTimeLiteral()
    {
        var tree = Utility.GetAST("after 2.5s { }");
        var after = Assert.IsType<After>(tree.Statements.First());
        Assert.Equal(SyntaxKind.AfterKeyword, after.Keyword.Kind);

        var literal = Assert.IsType<Literal>(after.Duration);
        Assert.Equal(2.5d, literal.Value);
    }

    [Fact]
    public void Parses_InterfaceInvocation_IndexInitializerWithExpression()
    {
        var tree = Utility.GetAST("new Foo { [x + 1]: 42 }");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<InterfaceInvocation>(exprStmt.Expression);
        var init = Assert.IsType<InterfaceInvocationIndexInitializer>(invocation.Body.Initializers.Single());

        var indexExpr = Assert.IsType<BinaryOperator>(init.IndexExpression);
        Assert.Equal(SyntaxKind.Plus, indexExpr.Operator.Kind);
        Assert.IsType<Identifier>(indexExpr.Left);
        Assert.Equal(1L, Assert.IsType<Literal>(indexExpr.Right).Value);
        Assert.Equal(42L, Assert.IsType<Literal>(init.Expression).Value);
    }

    [Fact]
    public void Parses_UnionType_MultipleElements()
    {
        var tree = Utility.GetAST("let x: A | B | C");
        var union = Assert.IsType<UnionType>(((VariableDeclaration)tree.Statements.Single()).ColonTypeClause!.Type);
        Assert.Equal(3, union.Types.Count);
        Assert.Equal(2, union.Pipes.Count);
        Assert.All(union.Types, t => Assert.IsType<TypeName>(t));
    }

    [Fact]
    public void Parses_IntersectionType_MultipleElements()
    {
        var tree = Utility.GetAST("let x: A & B & C");
        var intersection = Assert.IsType<IntersectionType>(((VariableDeclaration)tree.Statements.Single()).ColonTypeClause!.Type);
        Assert.Equal(3, intersection.Types.Count);
        Assert.Equal(2, intersection.Ampersands.Count);
        Assert.All(intersection.Types, t => Assert.IsType<TypeName>(t));
    }

    [Fact]
    public void Parses_FunctionType_EmptyParens()
    {
        var tree = Utility.GetAST("type Fn = fn(): void");
        var alias = Assert.IsType<TypeAlias>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(alias.EqualsTypeClause.Type);
        Assert.NotNull(fnType.Parameters);
        Assert.Empty(fnType.Parameters.ParameterList);
        Assert.IsType<PrimitiveType>(fnType.ReturnType.Type);
    }

    [Fact]
    public void Parses_FunctionType_TypeParametersWithEmptyParens()
    {
        var tree = Utility.GetAST("type Fn = fn<T>(): T");
        var alias = Assert.IsType<TypeAlias>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(alias.EqualsTypeClause.Type);

        Assert.NotNull(fnType.TypeParameters);
        Assert.Single(fnType.TypeParameters.ParameterList);
        Assert.Equal("T", fnType.TypeParameters.ParameterList[0].Name.Text);

        Assert.NotNull(fnType.Parameters);
        Assert.Empty(fnType.Parameters.ParameterList);

        var returnType = Assert.IsType<TypeName>(fnType.ReturnType.Type);
        Assert.Equal("T", returnType.Name.Text);
    }

    [Fact]
    public void Parses_FunctionType_OnlyTypeParameters()
    {
        var tree = Utility.GetAST("type Fn = fn<T>: T");
        var alias = Assert.IsType<TypeAlias>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(alias.EqualsTypeClause.Type);

        Assert.NotNull(fnType.TypeParameters);
        Assert.Single(fnType.TypeParameters.ParameterList);
        Assert.Equal("T", fnType.TypeParameters.ParameterList[0].Name.Text);

        Assert.Null(fnType.Parameters);

        var returnType = Assert.IsType<TypeName>(fnType.ReturnType.Type);
        Assert.Equal("T", returnType.Name.Text);
    }

    [Fact]
    public void Parses_WhileLoop_WithBlockBody()
    {
        var tree = Utility.GetAST("while x > 0 { return x }");
        Assert.Single(tree.Statements);

        var whileStmt = Assert.IsType<While>(tree.Statements.First());
        Assert.Equal(SyntaxKind.WhileKeyword, whileStmt.Keyword.Kind);

        var condition = Assert.IsType<BinaryOperator>(whileStmt.Condition);
        Assert.Equal(SyntaxKind.RArrow, condition.Operator.Kind);

        var body = Assert.IsType<Block>(whileStmt.Body);
        Assert.Single(body.Statements);
        Assert.IsType<Return>(body.Statements.First());
    }

    [Fact]
    public void Parses_WhileLoop_WithExpressionBody()
    {
        var tree = Utility.GetAST("while true break");
        Assert.Single(tree.Statements);

        var whileStmt = Assert.IsType<While>(tree.Statements.First());
        var condition = Assert.IsType<Literal>(whileStmt.Condition);
        Assert.Equal(true, condition.Value);

        var body = Assert.IsType<Break>(whileStmt.Body);
        Assert.Equal(SyntaxKind.BreakKeyword, body.Keyword.Kind);
    }

    [Fact]
    public void Parses_WhileLoop_WithNestedBlock()
    {
        var tree = Utility.GetAST("while a { while b { continue } }");
        Assert.Single(tree.Statements);

        var outerWhile = Assert.IsType<While>(tree.Statements.First());
        var outerBody = Assert.IsType<Block>(outerWhile.Body);
        var innerWhile = Assert.IsType<While>(outerBody.Statements.First());
        var innerBody = Assert.IsType<Block>(innerWhile.Body);
        var continueStmt = Assert.IsType<Continue>(innerBody.Statements.First());
        Assert.Equal(SyntaxKind.ContinueKeyword, continueStmt.Keyword.Kind);
    }

    [Fact]
    public void Parses_BreakStatement()
    {
        var tree = Utility.GetAST("while true { break }");
        var whileStmt = Assert.IsType<While>(tree.Statements.Single());
        var block = Assert.IsType<Block>(whileStmt.Body);
        var breakStmt = Assert.IsType<Break>(block.Statements.First());
        Assert.Equal(SyntaxKind.BreakKeyword, breakStmt.Keyword.Kind);
    }

    [Fact]
    public void Parses_ContinueStatement()
    {
        var tree = Utility.GetAST("while true { continue }");
        var whileStmt = Assert.IsType<While>(tree.Statements.Single());
        var block = Assert.IsType<Block>(whileStmt.Body);
        var continueStmt = Assert.IsType<Continue>(block.Statements.First());
        Assert.Equal(SyntaxKind.ContinueKeyword, continueStmt.Keyword.Kind);
    }

    [Fact]
    public void Parses_InterfaceInvocation_EmptyBody()
    {
        var tree = Utility.GetAST("new Foo {}");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<InterfaceInvocation>(exprStmt.Expression);
        Assert.Equal("Foo", invocation.Name.Token.Text);
        Assert.Null(invocation.TypeArguments);
        Assert.NotNull(invocation.Body);
        Assert.Empty(invocation.Body.Initializers);
        Assert.Equal(SyntaxKind.NewKeyword, invocation.Keyword.Kind);
        Assert.Equal(SyntaxKind.LBrace, invocation.Body.LeftBrace.Kind);
        Assert.Equal(SyntaxKind.RBrace, invocation.Body.RightBrace.Kind);
    }

    [Fact]
    public void Parses_InterfaceInvocation_WithPropertyInitializer()
    {
        var tree = Utility.GetAST("new Foo { bar: 42 }");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<InterfaceInvocation>(exprStmt.Expression);
        Assert.Equal("Foo", ((Identifier)invocation.Name).Name.Text);
        Assert.Null(invocation.TypeArguments);

        var body = invocation.Body;
        Assert.Single(body.Initializers);
        var init = Assert.IsType<InterfaceInvocationPropertyInitializer>(body.Initializers[0]);
        Assert.Equal("bar", init.Name.Text);
        Assert.IsType<Literal>(init.Expression);
        Assert.Equal(42L, ((Literal)init.Expression).Value);
    }

    [Fact]
    public void Parses_InterfaceInvocation_WithIndexInitializer()
    {
        var tree = Utility.GetAST("new Foo { [0]: 'hello' }");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<InterfaceInvocation>(exprStmt.Expression);
        Assert.Equal("Foo", ((Identifier)invocation.Name).Name.Text);
        Assert.Null(invocation.TypeArguments);

        var body = invocation.Body;
        Assert.Single(body.Initializers);
        var init = Assert.IsType<InterfaceInvocationIndexInitializer>(body.Initializers[0]);
        Assert.Equal(SyntaxKind.LBracket, init.LeftBracket.Kind);
        Assert.Equal(SyntaxKind.RBracket, init.RightBracket.Kind);
        var indexExpr = Assert.IsType<Literal>(init.IndexExpression);
        Assert.Equal(0L, indexExpr.Value);
        var valueExpr = Assert.IsType<Literal>(init.Expression);
        Assert.Equal("hello", valueExpr.Value);
    }

    [Fact]
    public void Parses_InterfaceInvocation_MultipleInitializers()
    {
        var tree = Utility.GetAST("new Foo { x: 1, y: 2, [3]: 4 }");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<InterfaceInvocation>(exprStmt.Expression);
        var body = invocation.Body;
        Assert.Equal(3, body.Initializers.Count);

        var first = Assert.IsType<InterfaceInvocationPropertyInitializer>(body.Initializers[0]);
        Assert.Equal("x", first.Name.Text);

        var second = Assert.IsType<InterfaceInvocationPropertyInitializer>(body.Initializers[1]);
        Assert.Equal("y", second.Name.Text);

        var third = Assert.IsType<InterfaceInvocationIndexInitializer>(body.Initializers[2]);
        Assert.Equal(3L, ((Literal)third.IndexExpression).Value);
    }

    [Fact]
    public void Parses_InterfaceInvocation_WithTypeArguments()
    {
        var tree = Utility.GetAST("new Foo::<number, string> { prop: 1 }");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<InterfaceInvocation>(exprStmt.Expression);
        Assert.Equal("Foo", ((Identifier)invocation.Name).Name.Text);
        Assert.NotNull(invocation.TypeArguments);
        Assert.Equal(2, invocation.TypeArguments.ArgumentsList.Count);
        Assert.IsType<PrimitiveType>(invocation.TypeArguments.ArgumentsList[0]);
        Assert.IsType<PrimitiveType>(invocation.TypeArguments.ArgumentsList[1]);

        var body = invocation.Body;
        Assert.Single(body.Initializers);
        Assert.IsType<InterfaceInvocationPropertyInitializer>(body.Initializers[0]);
    }

    [Fact]
    public void Parses_InterfaceInvocation_WithoutTypeArguments()
    {
        var tree = Utility.GetAST("new Foo { prop: 1 }");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<InterfaceInvocation>(exprStmt.Expression);
        Assert.Null(invocation.TypeArguments);
    }

    [Fact]
    public void Parses_InterfaceInvocation_Chained()
    {
        var tree = Utility.GetAST("new Foo { x: 1 }.bar");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var propAccess = Assert.IsType<PropertyAccess>(exprStmt.Expression);
        var invocation = Assert.IsType<InterfaceInvocation>(propAccess.Expression);
        Assert.Equal("Foo", ((Identifier)invocation.Name).Name.Text);
    }

    [Fact]
    public void Parses_InterfaceInvocation_AsArgument()
    {
        var tree = Utility.GetAST("create(new Foo { x: 1 })");
        Assert.Single(tree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var call = Assert.IsType<Invocation>(exprStmt.Expression);
        Assert.Single(call.Arguments.ArgumentList);
        var invocation = Assert.IsType<InterfaceInvocation>(call.Arguments.ArgumentList[0]);
        Assert.Equal("Foo", ((Identifier)invocation.Name).Name.Text);
    }

    [Fact]
    public void Parses_InterfaceInvocation_InsideAssignment()
    {
        var tree = Utility.GetAST("let x = new Foo { a: 1 }");
        Assert.Single(tree.Statements);

        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        Assert.NotNull(varDecl.EqualsValueClause);
        var invocation = Assert.IsType<InterfaceInvocation>(varDecl.EqualsValueClause.Value);
        Assert.Equal("Foo", ((Identifier)invocation.Name).Name.Text);
    }

    [Fact]
    public void Parses_IndexedType_Basic()
    {
        var tree = Utility.GetAST("let x: T[K]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var indexed = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);

        var baseType = Assert.IsType<TypeName>(indexed.Type);
        Assert.Equal("T", baseType.Name.Text);

        var indexType = Assert.IsType<TypeName>(indexed.IndexType);
        Assert.Equal("K", indexType.Name.Text);

        Assert.Equal(SyntaxKind.LBracket, indexed.LeftBracket.Kind);
        Assert.Equal(SyntaxKind.RBracket, indexed.RightBracket.Kind);
    }

    [Fact]
    public void Parses_IndexedType_PrimitiveBase()
    {
        var tree = Utility.GetAST("let x: number[string]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var indexed = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);
        Assert.IsType<PrimitiveType>(indexed.Type);
        Assert.IsType<PrimitiveType>(indexed.IndexType);
    }

    [Fact]
    public void Parses_IndexedType_LiteralIndex()
    {
        var tree = Utility.GetAST("let x: T['length']");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var indexed = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);

        var baseType = Assert.IsType<TypeName>(indexed.Type);
        Assert.Equal("T", baseType.Name.Text);

        var indexType = Assert.IsType<LiteralType>(indexed.IndexType);
        Assert.Equal("'length'", indexType.Token.Text);
    }

    [Fact]
    public void Parses_IndexedType_UnionIndex()
    {
        var tree = Utility.GetAST("let x: T['a' | 'b']");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var indexed = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);

        var unionIndex = Assert.IsType<UnionType>(indexed.IndexType);
        Assert.Equal(2, unionIndex.Types.Count);
        Assert.IsType<LiteralType>(unionIndex.Types.First());
        Assert.IsType<LiteralType>(unionIndex.Types.Last());
    }

    [Fact]
    public void Parses_IndexedType_Chained()
    {
        var tree = Utility.GetAST("let x: T[K][V]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var outer = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);

        var inner = Assert.IsType<IndexedType>(outer.Type);
        var baseType = Assert.IsType<TypeName>(inner.Type);
        Assert.Equal("T", baseType.Name.Text);

        var innerIndex = Assert.IsType<TypeName>(inner.IndexType);
        Assert.Equal("K", innerIndex.Name.Text);

        var outerIndex = Assert.IsType<TypeName>(outer.IndexType);
        Assert.Equal("V", outerIndex.Name.Text);
    }

    [Fact]
    public void Parses_IndexedType_WithPostfixOptional()
    {
        var tree = Utility.GetAST("let x: T[K]?");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var optional = Assert.IsType<OptionalType>(varDecl.ColonTypeClause!.Type);
        var indexed = Assert.IsType<IndexedType>(optional.NonNullableType);

        Assert.IsType<TypeName>(indexed.Type);
        Assert.IsType<TypeName>(indexed.IndexType);
    }

    [Fact]
    public void Parses_IndexedType_WithArray()
    {
        var tree = Utility.GetAST("let x: T[K][]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var array = Assert.IsType<ArrayType>(varDecl.ColonTypeClause!.Type);
        var indexed = Assert.IsType<IndexedType>(array.ElementType);

        Assert.IsType<TypeName>(indexed.Type);
        Assert.IsType<TypeName>(indexed.IndexType);
    }

    [Fact]
    public void Parses_IndexedType_WithComplexBase()
    {
        var tree = Utility.GetAST("let x: (A & B)[C | D]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var indexed = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);

        var parenthesized = Assert.IsType<ParenthesizedType>(indexed.Type);
        var intersection = Assert.IsType<IntersectionType>(parenthesized.Type);
        Assert.Equal(2, intersection.Types.Count);

        var unionIndex = Assert.IsType<UnionType>(indexed.IndexType);
        Assert.Equal(2, unionIndex.Types.Count);
    }

    [Fact]
    public void Parses_IndexedType_NestedFunctionType()
    {
        var tree = Utility.GetAST("let x: (fn(): T)[K]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var indexed = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);

        var parenthesized = Assert.IsType<ParenthesizedType>(indexed.Type);
        Assert.IsType<FunctionType>(parenthesized.Type);
        Assert.IsType<TypeName>(indexed.IndexType);
    }

    [Fact]
    public void Parses_IndexedType_InsideUnion()
    {
        var tree = Utility.GetAST("let x: T[K] | number");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var union = Assert.IsType<UnionType>(varDecl.ColonTypeClause!.Type);
        Assert.Equal(2, union.Types.Count);
        Assert.IsType<IndexedType>(union.Types.First());
        Assert.IsType<PrimitiveType>(union.Types.Last());
    }

    [Fact]
    public void Parses_IndexedType_GenericBase()
    {
        var tree = Utility.GetAST("let x: Array<number>[0]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var indexed = Assert.IsType<IndexedType>(varDecl.ColonTypeClause!.Type);

        var typeName = Assert.IsType<TypeName>(indexed.Type);
        Assert.Equal("Array", typeName.Name.Text);
        Assert.NotNull(typeName.TypeArguments);
        Assert.Single(typeName.TypeArguments.ArgumentsList);

        var indexType = Assert.IsType<LiteralType>(indexed.IndexType);
        Assert.Equal("0", indexType.Token.Text);
    }

    [Fact]
    public void Parses_DeclareFunctionSignature_WithGenericConstraint()
    {
        var tree = Utility.GetAST("declare fn id<T: number>(x: T): T");
        var declare = Assert.IsType<Declare>(tree.Statements.Single());
        var sig = Assert.IsType<DeclareFunctionSignature>(declare.Signature);
        Assert.NotNull(sig.TypeParameters);
        var tp = sig.TypeParameters.ParameterList[0];
        Assert.NotNull(tp.ColonTypeClause);
        Assert.IsType<PrimitiveType>(tp.ColonTypeClause.Type);
    }

    [Fact]
    public void Parses_Sealed_InterfaceDeclaration()
    {
        var tree = Utility.GetAST("sealed interface I;");
        Assert.Single(tree.Statements);

        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.Equal("I", iface.Name.Text);
        Assert.NotNull(iface.SealedKeyword);
        Assert.Null(iface.TypeParameters);
        Assert.Null(iface.ColonTypeListClause);
        Assert.Null(iface.Body);
        Assert.Equal(SyntaxKind.InterfaceKeyword, iface.Keyword.Kind);
        Assert.Equal(SyntaxKind.SealedKeyword, iface.SealedKeyword.Kind);
    }

    [Fact]
    public void Parses_Declare_InterfaceDeclaration()
    {
        var tree = Utility.GetAST("declare interface I;");
        Assert.Single(tree.Statements);

        var declare = Assert.IsType<Declare>(tree.Statements.First());
        var iface = Assert.IsType<InterfaceDeclaration>(declare.Signature);
        Assert.Equal("I", iface.Name.Text);
        Assert.Null(iface.SealedKeyword);
        Assert.Null(iface.TypeParameters);
        Assert.Null(iface.ColonTypeListClause);
        Assert.Null(iface.Body);
        Assert.Equal(SyntaxKind.InterfaceKeyword, iface.Keyword.Kind);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_NoBody()
    {
        var tree = Utility.GetAST("interface I;");
        Assert.Single(tree.Statements);

        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.Equal("I", iface.Name.Text);
        Assert.Null(iface.SealedKeyword);
        Assert.Null(iface.TypeParameters);
        Assert.Null(iface.ColonTypeListClause);
        Assert.Null(iface.Body);
        Assert.Equal(SyntaxKind.InterfaceKeyword, iface.Keyword.Kind);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_NoBody_WithExtras()
    {
        var tree = Utility.GetAST("interface I<T, U>: A, B;");
        Assert.Single(tree.Statements);

        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.Equal("I", iface.Name.Text);
        Assert.Null(iface.SealedKeyword);
        Assert.NotNull(iface.TypeParameters);
        Assert.Equal(2, iface.TypeParameters.ParameterList.Count);
        Assert.NotNull(iface.ColonTypeListClause);
        Assert.Equal(2, iface.ColonTypeListClause.Types.Count);
        Assert.Null(iface.Body);
        Assert.Equal(SyntaxKind.InterfaceKeyword, iface.Keyword.Kind);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_EmptyBody()
    {
        var tree = Utility.GetAST("interface I { }");
        Assert.Single(tree.Statements);

        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.Equal("I", iface.Name.Text);
        Assert.Null(iface.TypeParameters);
        Assert.Null(iface.ColonTypeListClause);
        Assert.NotNull(iface.Body);
        Assert.Empty(iface.Body.Members);
        Assert.Equal(SyntaxKind.InterfaceKeyword, iface.Keyword.Kind);
        Assert.Equal(SyntaxKind.LBrace, iface.Body.LeftBrace.Kind);
        Assert.Equal(SyntaxKind.RBrace, iface.Body.RightBrace.Kind);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_WithProperty()
    {
        var tree = Utility.GetAST("interface IPoint { x: number }");
        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.NotNull(iface.Body);
        Assert.Single(iface.Body.Members);

        var prop = Assert.IsType<PropertyDeclaration>(iface.Body.Members.First());
        Assert.Null(prop.MutKeyword);
        Assert.Equal("x", prop.Name.Text);
        Assert.NotNull(prop.ColonTypeClause);
        Assert.IsType<PrimitiveType>(prop.ColonTypeClause.Type);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_WithMutableProperty()
    {
        var result = Utility.Parse("interface I { mut count: int }");
        Utility.AssertNoErrors(result);

        var iface = Assert.IsType<InterfaceDeclaration>(result.Tree.Statements.First());
        Assert.NotNull(iface.Body);

        var prop = Assert.IsType<PropertyDeclaration>(iface.Body.Members.First());
        Assert.NotNull(prop.MutKeyword);
        Assert.Equal(SyntaxKind.MutKeyword, prop.MutKeyword.Kind);
        Assert.Equal("count", prop.Name.Text);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_WithIndexer()
    {
        var tree = Utility.GetAST("interface I { [number]: string }");
        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.NotNull(iface.Body);

        var idx = Assert.IsType<IndexerDeclaration>(iface.Body.Members.First());
        Assert.Null(idx.MutKeyword);
        Assert.NotNull(idx.IndexType);
        Assert.IsType<PrimitiveType>(idx.IndexType);
        Assert.NotNull(idx.ColonTypeClause);
        Assert.IsType<PrimitiveType>(idx.ColonTypeClause.Type);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_WithMutableIndexer()
    {
        var tree = Utility.GetAST("interface I { mut [string]: number }");
        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.NotNull(iface.Body);

        var idx = Assert.IsType<IndexerDeclaration>(iface.Body.Members.First());
        Assert.NotNull(idx.MutKeyword);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_MultipleMembers()
    {
        var tree = Utility.GetAST("interface I { x: number, y: string, [int]: bool }");
        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.NotNull(iface.Body);
        Assert.Equal(3, iface.Body.Members.Count);
        Assert.IsType<PropertyDeclaration>(iface.Body.Members[0]);
        Assert.IsType<PropertyDeclaration>(iface.Body.Members[1]);
        Assert.IsType<IndexerDeclaration>(iface.Body.Members[2]);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_MembersWithoutCommas()
    {
        var tree = Utility.GetAST("interface I { x: number y: string }");
        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.NotNull(iface.Body);
        Assert.Equal(2, iface.Body.Members.Count);
    }

    [Fact]
    public void Parses_InterfaceDeclaration_GenericWithBaseTypes()
    {
        var tree = Utility.GetAST("interface I<T, U: number> : Base, IDisposable { }");
        var iface = Assert.IsType<InterfaceDeclaration>(tree.Statements.First());
        Assert.Equal("I", iface.Name.Text);
        Assert.NotNull(iface.TypeParameters);
        Assert.Equal(2, iface.TypeParameters.ParameterList.Count);
        Assert.Equal("T", iface.TypeParameters.ParameterList[0].Name.Text);
        Assert.Equal("U", iface.TypeParameters.ParameterList[1].Name.Text);
        Assert.NotNull(iface.TypeParameters.ParameterList[1].ColonTypeClause);
        Assert.NotNull(iface.ColonTypeListClause);
        Assert.Equal(2, iface.ColonTypeListClause.Types.Count);
        Assert.IsType<TypeName>(iface.ColonTypeListClause.Types.First());
        Assert.IsType<TypeName>(iface.ColonTypeListClause.Types.Last());
    }

    [Fact]
    public void Parses_FunctionType_Basic()
    {
        var tree = Utility.GetAST("let callback: fn: void");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(varDecl.ColonTypeClause!.Type);
        var returnType = Assert.IsType<PrimitiveType>(fnType.ReturnType.Type);
        Assert.Null(fnType.TypeParameters);
        Assert.Null(fnType.Parameters);
        Assert.Equal(PrimitiveTypeKind.Void, returnType.Kind);
    }

    [Fact]
    public void Parses_FunctionType_ReturningOptional()
    {
        var tree = Utility.GetAST("let callback: fn(x: number): number?");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(varDecl.ColonTypeClause!.Type);
        var returnType = Assert.IsType<OptionalType>(fnType.ReturnType.Type);
        var inner = Assert.IsType<PrimitiveType>(returnType.NonNullableType);
        Assert.Equal(PrimitiveTypeKind.Number, inner.Kind);
    }

    [Fact]
    public void Parses_FunctionType_ReturningArray()
    {
        var tree = Utility.GetAST("let callback: fn(x: number): number[]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(varDecl.ColonTypeClause!.Type);
        var returnType = Assert.IsType<ArrayType>(fnType.ReturnType.Type);
        Assert.IsType<PrimitiveType>(returnType.ElementType);
    }

    [Fact]
    public void Parses_FunctionType_ReturningUnion()
    {
        var tree = Utility.GetAST("let callback: fn(x: number): number | string");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(varDecl.ColonTypeClause!.Type);
        var returnType = Assert.IsType<UnionType>(fnType.ReturnType.Type);
        Assert.Equal(2, returnType.Types.Count);
    }

    [Fact]
    public void Parses_OptionalFunction_WithParentheses()
    {
        var tree = Utility.GetAST("let callback: (fn(x: number): number)?");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var optional = Assert.IsType<OptionalType>(varDecl.ColonTypeClause!.Type);
        var parenType = Assert.IsType<ParenthesizedType>(optional.NonNullableType);
        var fnType = Assert.IsType<FunctionType>(parenType.Type);
        Assert.NotNull(fnType.Parameters);
        Assert.Single(fnType.Parameters.ParameterList);
    }

    [Fact]
    public void Parses_ArrayOfFunctions_WithParentheses()
    {
        var tree = Utility.GetAST("let callbacks: (fn(x: number): number)[]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var arrayType = Assert.IsType<ArrayType>(varDecl.ColonTypeClause!.Type);
        var parenType = Assert.IsType<ParenthesizedType>(arrayType.ElementType);
        var fnType = Assert.IsType<FunctionType>(parenType.Type);
        Assert.NotNull(fnType.Parameters);
        Assert.Single(fnType.Parameters.ParameterList);
    }

    [Fact]
    public void Parses_FunctionType_WithTypeParameters()
    {
        var tree = Utility.GetAST("let identity: fn<T>(value: T): T");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(varDecl.ColonTypeClause!.Type);

        Assert.NotNull(fnType.TypeParameters);
        Assert.Single(fnType.TypeParameters.ParameterList);
        var tp = fnType.TypeParameters.ParameterList.First();
        Assert.Equal("T", tp.Name.Text);
        Assert.Null(tp.ColonTypeClause);

        Assert.NotNull(fnType.Parameters);
        Assert.Single(fnType.Parameters.ParameterList);
        var param = fnType.Parameters.ParameterList.First();
        Assert.Equal("value", param.Name.Text);
        var paramType = Assert.IsType<TypeName>(param.ColonTypeClause!.Type);
        Assert.Equal("T", paramType.Name.Text);

        var returnType = Assert.IsType<TypeName>(fnType.ReturnType.Type);
        Assert.Equal("T", returnType.Name.Text);
    }

    [Fact]
    public void Parses_FunctionType_WithTypeParametersAndConstraints()
    {
        var tree = Utility.GetAST("let wrap: fn<T: number>(item: T): T[]");
        var varDecl = Assert.IsType<VariableDeclaration>(tree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(varDecl.ColonTypeClause!.Type);

        Assert.NotNull(fnType.TypeParameters);
        Assert.Single(fnType.TypeParameters.ParameterList);
        var tp = fnType.TypeParameters.ParameterList.First();
        Assert.Equal("T", tp.Name.Text);
        Assert.NotNull(tp.ColonTypeClause);
        var constraint = Assert.IsType<PrimitiveType>(tp.ColonTypeClause.Type);
        Assert.Equal(PrimitiveTypeKind.Number, constraint.Kind);

        Assert.NotNull(fnType.Parameters);
        Assert.Single(fnType.Parameters.ParameterList);
        var param = fnType.Parameters.ParameterList.First();
        Assert.Equal("item", param.Name.Text);
        var paramType = Assert.IsType<TypeName>(param.ColonTypeClause!.Type);
        Assert.Equal("T", paramType.Name.Text);

        var returnType = Assert.IsType<ArrayType>(fnType.ReturnType.Type);
        var elementType = Assert.IsType<TypeName>(returnType.ElementType);
        Assert.Equal("T", elementType.Name.Text);
    }

    [Fact]
    public void Parses_DeclareFunctionSignature_Basic()
    {
        var tree = Utility.GetAST("declare fn add(a: number, b: number): number");
        Assert.Single(tree.Statements);

        var declare = Assert.IsType<Declare>(tree.Statements.First());
        var sig = Assert.IsType<DeclareFunctionSignature>(declare.Signature);
        Assert.Equal("add", sig.Name.Text);
        Assert.Null(sig.TypeParameters);
        Assert.NotNull(sig.Parameters);
        Assert.Equal(2, sig.Parameters.ParameterList.Count);
        Assert.All(sig.Parameters.ParameterList, p => Assert.NotNull(p.ColonTypeClause));
        Assert.NotNull(sig.ReturnType);
        Assert.IsType<PrimitiveType>(sig.ReturnType.Type);
    }

    [Fact]
    public void Parses_DeclareFunctionSignature_WithTypeParameters()
    {
        var tree = Utility.GetAST("declare fn id<T>(value: T): T");
        var declare = Assert.IsType<Declare>(tree.Statements.Single());
        var sig = Assert.IsType<DeclareFunctionSignature>(declare.Signature);
        Assert.Equal("id", sig.Name.Text);
        Assert.NotNull(sig.TypeParameters);
        Assert.Single(sig.TypeParameters.ParameterList);
        Assert.Equal("T", sig.TypeParameters.ParameterList.First().Name.Text);
        Assert.NotNull(sig.ReturnType);
        var returnType = Assert.IsType<TypeName>(sig.ReturnType.Type);
        Assert.Equal("T", returnType.Name.Text);
    }

    [Fact]
    public void Parses_DeclareFunctionSignature_EmptyParameters()
    {
        var tree = Utility.GetAST("declare fn rand(): number");
        var declare = Assert.IsType<Declare>(tree.Statements.Single());
        var sig = Assert.IsType<DeclareFunctionSignature>(declare.Signature);
        Assert.NotNull(sig.Parameters);
        Assert.Empty(sig.Parameters.ParameterList);
    }

    [Fact]
    public void Parses_DeclareFunctionSignature_NoParameters()
    {
        var tree = Utility.GetAST("declare fn exit: void");
        var declare = Assert.IsType<Declare>(tree.Statements.Single());
        var sig = Assert.IsType<DeclareFunctionSignature>(declare.Signature);
        Assert.Null(sig.Parameters);
        Assert.NotNull(sig.ReturnType);
        var ret = Assert.IsType<PrimitiveType>(sig.ReturnType.Type);
        Assert.Equal(PrimitiveTypeKind.Void, ret.Kind);
    }

    [Fact]
    public void Parses_DeclareVariableSignature_Let()
    {
        var tree = Utility.GetAST("declare let x: number");
        var declare = Assert.IsType<Declare>(tree.Statements.Single());
        var sig = Assert.IsType<DeclareVariableSignature>(declare.Signature);
        Assert.Equal("x", sig.Name.Text);
        Assert.Equal(SyntaxKind.LetKeyword, sig.Keyword.Kind);
        Assert.NotNull(sig.ColonTypeClause);
        Assert.IsType<PrimitiveType>(sig.ColonTypeClause.Type);
    }

    [Fact]
    public void Parses_DeclareVariableSignature_Mut()
    {
        var tree = Utility.GetAST("declare mut y: string");
        var declare = Assert.IsType<Declare>(tree.Statements.Single());
        var sig = Assert.IsType<DeclareVariableSignature>(declare.Signature);
        Assert.Equal("y", sig.Name.Text);
        Assert.Equal(SyntaxKind.MutKeyword, sig.Keyword.Kind);
        Assert.NotNull(sig.ColonTypeClause);
        Assert.IsType<PrimitiveType>(sig.ColonTypeClause.Type);
    }

    [Fact]
    public void Parses_Type_Precedence()
    {
        var tree = Utility.GetAST("let x: number? | bool & string");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);
        var union = Assert.IsType<UnionType>(variableDeclaration.ColonTypeClause.Type);
        Assert.Single(union.Pipes);
        Assert.Equal(2, union.Types.Count);

        var numberOptionalType = Assert.IsType<OptionalType>(union.Types.First());
        var intersection = Assert.IsType<IntersectionType>(union.Types.Last());
        Assert.Single(intersection.Ampersands);
        Assert.Equal(2, intersection.Types.Count);

        var boolType = Assert.IsType<PrimitiveType>(intersection.Types.First());
        var stringType = Assert.IsType<PrimitiveType>(intersection.Types.Last());
        var numberType = Assert.IsType<PrimitiveType>(numberOptionalType.NonNullableType);
        Assert.Equal(PrimitiveTypeKind.Bool, boolType.Kind);
        Assert.Equal(PrimitiveTypeKind.String, stringType.Kind);
        Assert.Equal(PrimitiveTypeKind.Number, numberType.Kind);
    }

    [Fact]
    public void Parses_IntersectionType()
    {
        var tree = Utility.GetAST("let x: number & string");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);
        var intersection = Assert.IsType<IntersectionType>(variableDeclaration.ColonTypeClause.Type);
        Assert.Single(intersection.Ampersands);
        Assert.Equal(2, intersection.Types.Count);

        var numberType = Assert.IsType<PrimitiveType>(intersection.Types.First());
        var stringType = Assert.IsType<PrimitiveType>(intersection.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, numberType.Kind);
        Assert.Equal(PrimitiveTypeKind.String, stringType.Kind);
    }

    [Fact]
    public void Parses_UnionType()
    {
        var tree = Utility.GetAST("let x: number | string");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);
        var union = Assert.IsType<UnionType>(variableDeclaration.ColonTypeClause.Type);
        Assert.Single(union.Pipes);
        Assert.Equal(2, union.Types.Count);

        var numberType = Assert.IsType<PrimitiveType>(union.Types.First());
        var stringType = Assert.IsType<PrimitiveType>(union.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, numberType.Kind);
        Assert.Equal(PrimitiveTypeKind.String, stringType.Kind);
    }

    [Fact]
    public void Parses_ArrayType_WithOptionals()
    {
        var tree = Utility.GetAST("let x: Abc?[]?");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);

        var outerOptional = Assert.IsType<OptionalType>(variableDeclaration.ColonTypeClause.Type);
        var array = Assert.IsType<ArrayType>(outerOptional.NonNullableType);
        var innerOptional = Assert.IsType<OptionalType>(array.ElementType);
        var typeName = Assert.IsType<TypeName>(innerOptional.NonNullableType);
        Assert.Equal("Abc", typeName.Name.Text);
    }

    [Fact]
    public void Parses_ArrayType_Mutable()
    {
        var tree = Utility.GetAST("let x: Abc[mut]");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);

        var array = Assert.IsType<ArrayType>(variableDeclaration.ColonTypeClause.Type);
        Assert.NotNull(array.MutKeyword);
        Assert.Equal(SyntaxKind.MutKeyword, array.MutKeyword.Kind);

        var typeName = Assert.IsType<TypeName>(array.ElementType);
        Assert.Equal("Abc", typeName.Name.Text);
    }

    [Fact]
    public void Parses_ArrayType()
    {
        var tree = Utility.GetAST("let x: Abc[]");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);

        var array = Assert.IsType<ArrayType>(variableDeclaration.ColonTypeClause.Type);
        var typeName = Assert.IsType<TypeName>(array.ElementType);
        Assert.Null(array.MutKeyword);
        Assert.Equal("Abc", typeName.Name.Text);
        Assert.Equal(SyntaxKind.LBracket, array.LeftBracket.Kind);
        Assert.Equal(SyntaxKind.RBracket, array.RightBracket.Kind);
    }

    [Fact]
    public void Parses_OptionalType()
    {
        var tree = Utility.GetAST("let x: Abc?");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);

        var optional = Assert.IsType<OptionalType>(variableDeclaration.ColonTypeClause.Type);
        var typeName = Assert.IsType<TypeName>(optional.NonNullableType);
        Assert.Equal("Abc", typeName.Name.Text);
    }

    [Fact]
    public void Parses_TypeName()
    {
        var tree = Utility.GetAST("let x: Abc");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);

        var typeName = Assert.IsType<TypeName>(variableDeclaration.ColonTypeClause.Type);
        Assert.Equal("Abc", typeName.Name.Text);
    }

    [Theory]
    [InlineData("69")]
    [InlineData("10hz")]
    [InlineData("0x69")]
    [InlineData("0b1011")]
    [InlineData("'abc'")]
    [InlineData("\"abc\"")]
    [InlineData("true")]
    [InlineData("false")]
    public void Parses_LiteralType(string type)
    {
        var tree = Utility.GetAST($"let x: {type}");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);

        var literalType = Assert.IsType<LiteralType>(variableDeclaration.ColonTypeClause.Type);
        Assert.Equal(type, literalType.Token.Text);
    }

    [Fact]
    public void Parses_ParenthesizedType()
    {
        var tree = Utility.GetAST("let x: (number)");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variableDeclaration.ColonTypeClause);

        var parenthesized = Assert.IsType<ParenthesizedType>(variableDeclaration.ColonTypeClause.Type);
        var primitive = Assert.IsType<PrimitiveType>(parenthesized.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
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