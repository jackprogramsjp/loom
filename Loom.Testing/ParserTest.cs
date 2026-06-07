using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using Microsoft.VisualBasic.CompilerServices;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using LiteralType = Loom.Parsing.AST.LiteralType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Type = System.Type;
using TypeName = Loom.Parsing.AST.TypeName;
using UnionType = Loom.Parsing.AST.UnionType;

namespace Loom.Testing;

[Collection("Assembly")]
public class ParserTest
{
    [Fact]
    public void ThrowsFor_ExpectedIdentifier()
    {
        var diagnostics = Utility.GetParserDiagnostics("let");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedEof, "Expected identifier, got EOF.");
    }

    [Fact]
    public void ThrowsFor_ExpectedType()
    {
        var diagnostics = Utility.GetParserDiagnostics("let x:");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedEof, "Expected type, got EOF.");
    }

    [Fact]
    public void ThrowsFor_UnexpectedToken()
    {
        var diagnostics = Utility.GetParserDiagnostics("!");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedToken, "Unexpected token.");
    }

    [Fact]
    public void ThrowsFor_UnterminatedParens()
    {
        var diagnostics = Utility.GetParserDiagnostics("(1 + 2");
        var diagnostics2 = Utility.GetParserDiagnostics("(1 + 2]");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedEof, "Expected ')' here to close '(' at character 0, got EOF.");
        Utility.AssertDiagnostic(diagnostics2, InternalCodes.UnexpectedToken, "Expected ')' here to close '(' at character 0, got ']'.");
    }

    [Fact]
    public void ThrowsFor_InvalidAssignmentTarget()
    {
        var diagnostics = Utility.GetParserDiagnostics("1 = 1");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAssignmentTarget, "Invalid assignment target.");
    }

    [Fact]
    public void ThrowsFor_MissingFunctionBody()
    {
        var diagnostics = Utility.GetParserDiagnostics("fn foo");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.MissingFunctionBody, "Expected function body, got EOF.");
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

    [Fact]
    public void Parses_TypeAlias_GenericWithDefault()
    {
        var tree = Utility.GetAST("type Id<T = number> = T");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var alias = Assert.IsType<TypeAlias>(statement);
        Assert.Equal("Id", alias.Name.Text);
        Assert.Equal(SyntaxKind.TypeKeyword, alias.Keyword.Kind);
        Assert.Equal(SyntaxKind.Equals, alias.EqualsTypeClause.EqualsToken.Kind);
        Assert.NotNull(alias.TypeParameters);
        Assert.Single(alias.TypeParameters.ParameterList);

        var param = alias.TypeParameters.ParameterList.First();
        Assert.Equal("T", param.Name.Text);
        Assert.NotNull(param.EqualsTypeClause);

        var primitive = Assert.IsType<PrimitiveType>(param.EqualsTypeClause.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);

        var typeName = Assert.IsType<TypeName>(alias.EqualsTypeClause.Type);
        Assert.Equal("T", typeName.Name.Text);
    }

    [Fact]
    public void Parses_TypeAlias_Generic()
    {
        var tree = Utility.GetAST("type Intersect<A, B> = A & B");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var alias = Assert.IsType<TypeAlias>(statement);
        Assert.Equal("Intersect", alias.Name.Text);
        Assert.Equal(SyntaxKind.TypeKeyword, alias.Keyword.Kind);
        Assert.Equal(SyntaxKind.Equals, alias.EqualsTypeClause.EqualsToken.Kind);
        Assert.NotNull(alias.TypeParameters);
        Assert.Equal(2, alias.TypeParameters.ParameterList.Count);

        var a = alias.TypeParameters.ParameterList.First();
        var b = alias.TypeParameters.ParameterList.Last();
        Assert.Equal("A", a.Name.Text);
        Assert.Null(a.EqualsTypeClause);
        Assert.Equal("B", b.Name.Text);
        Assert.Null(b.EqualsTypeClause);
        Assert.IsType<IntersectionType>(alias.EqualsTypeClause.Type);
    }

    [Fact]
    public void Parses_TypeAlias()
    {
        var tree = Utility.GetAST("type A = number");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var alias = Assert.IsType<TypeAlias>(statement);
        Assert.Equal("A", alias.Name.Text);
        Assert.Equal(SyntaxKind.TypeKeyword, alias.Keyword.Kind);
        Assert.Equal(SyntaxKind.Equals, alias.EqualsTypeClause.EqualsToken.Kind);
        Assert.Null(alias.TypeParameters);

        var primitive = Assert.IsType<PrimitiveType>(alias.EqualsTypeClause.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Parses_FunctionDeclaration_WithDefaultParameter()
    {
        var tree = Utility.GetAST("fn greet(name: string = \"world\") { return \"Hello \" + name; }");
        var fn = Assert.IsType<FunctionDeclaration>(tree.Statements.Single());
        Assert.NotNull(fn.Parameters);
        Assert.Single(fn.Parameters.ParameterList);
        
        var parameter = fn.Parameters.ParameterList.First();
        Assert.Equal("name", parameter.Name.Text);
        Assert.NotNull(parameter.EqualsValueClause);
        
        var literal = Assert.IsType<Literal>(parameter.EqualsValueClause.Value);
        Assert.Equal("\"world\"", literal.Token.Text);
    }

    [Fact]
    public void Parses_FunctionDeclaration_WithReturnTypeAnnotation()
    {
        var tree = Utility.GetAST("fn sum(a: number, b: number): number { return a + b; }");
        var fn = Assert.IsType<FunctionDeclaration>(tree.Statements.Single());
        Assert.NotNull(fn.ReturnType);
        
        var returnType = Assert.IsType<PrimitiveType>(fn.ReturnType.Type);
        Assert.Equal(PrimitiveTypeKind.Number, returnType.Kind);
    }

    [Fact]
    public void Parses_FunctionDeclaration_ExpressionBody()
    {
        var tree = Utility.GetAST("fn double(x: number) -> x * 2");
        var fn = Assert.IsType<FunctionDeclaration>(tree.Statements.Single());
        Assert.Null(fn.ReturnType);

        var body = Assert.IsType<ExpressionBody>(fn.Body);
        var binary = Assert.IsType<BinaryOperator>(body.Expression);
        Assert.Equal(SyntaxKind.Star, binary.Operator.Kind);
    }

    [Fact]
    public void Parses_FunctionDeclaration_WithTypeParametersAndConstraints()
    {
        var tree = Utility.GetAST("fn wrap<T>(value: T): T { return value; }");
        var fn = Assert.IsType<FunctionDeclaration>(tree.Statements.Single());
        Assert.NotNull(fn.TypeParameters);
        Assert.Single(fn.TypeParameters.ParameterList);
        Assert.Equal("T", fn.TypeParameters.ParameterList.First().Name.Text);
    }

    [Fact]
    public void Parses_Empty_ExpressionBody_Function()
    {
        var tree = Utility.GetAST("fn abc -> none");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var fn = Assert.IsType<FunctionDeclaration>(statement);
        Assert.Equal("abc", fn.Name.Text);
        Assert.Equal(SyntaxKind.FnKeyword, fn.Keyword.Kind);
        Assert.Null(fn.TypeParameters);
        Assert.Null(fn.Parameters);
        Assert.Null(fn.ReturnType);

        var body = Assert.IsType<ExpressionBody>(fn.Body);
        var literal = Assert.IsType<Literal>(body.Expression);
        Assert.Null(literal.Value);
    }

    [Fact]
    public void Parses_Empty_Function()
    {
        var tree = Utility.GetAST("fn abc {}");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var fn = Assert.IsType<FunctionDeclaration>(statement);
        Assert.Equal("abc", fn.Name.Text);
        Assert.Equal(SyntaxKind.FnKeyword, fn.Keyword.Kind);
        Assert.Null(fn.TypeParameters);
        Assert.Null(fn.Parameters);
        Assert.Null(fn.ReturnType);

        var block = Assert.IsType<Block>(fn.Body);
        Assert.Empty(block.Statements);
    }

    [Theory]
    [InlineData("mut x;", true, false, null)]
    [InlineData("mut x = 1;", true, true, null)]
    [InlineData("mut x: number = 1;", true, true, PrimitiveTypeKind.Number)]
    [InlineData("let x;", false, false, null)]
    [InlineData("let x = 1;", false, true, null)]
    [InlineData("let x: bool = false;", false, true, PrimitiveTypeKind.Bool)]
    public void Parses_VariableDeclaration(string source, bool isMutable, bool hasInitializer, PrimitiveTypeKind? type)
    {
        var tree = Utility.GetAST(source);
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(statement);
        Assert.Equal("x", variableDeclaration.Name.Text);
        Assert.Equal(isMutable, variableDeclaration.Keyword.Kind == SyntaxKind.MutKeyword);
        Assert.Equal(hasInitializer, variableDeclaration.EqualsValueClause != null);
        if (type == null) return;

        Assert.NotNull(variableDeclaration.ColonTypeClause);
        var primitive = Assert.IsType<PrimitiveType>(variableDeclaration.ColonTypeClause.Type);
        Assert.Equal(type, primitive.Kind);
    }

    [Fact]
    public void Parses_Invocation_NoArguments()
    {
        var tree = Utility.GetAST("foo()");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(expressionStatement.Expression);
        var identifier = Assert.IsType<Identifier>(invocation.Expression);
        Assert.Equal("foo", identifier.Name.Text);
        Assert.Null(invocation.TypeArguments);
        Assert.NotNull(invocation.Arguments);
        Assert.Empty(invocation.Arguments.ArgumentList);
    }

    [Fact]
    public void Parses_Invocation_WithArguments()
    {
        var tree = Utility.GetAST("add(1, 2, 3)");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(expressionStatement.Expression);
        var identifier = Assert.IsType<Identifier>(invocation.Expression);
        Assert.Equal("add", identifier.Name.Text);
        Assert.NotNull(invocation.Arguments);
        Assert.Equal(3, invocation.Arguments.ArgumentList.Count);
        Assert.All(invocation.Arguments.ArgumentList, a => Assert.IsType<Literal>(a));
    }

    [Fact]
    public void Parses_Invocation_WithTypeArguments()
    {
        var tree = Utility.GetAST("identity::<number>(42)");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(expressionStatement.Expression);
        Assert.Equal("identity", ((Identifier)invocation.Expression).Name.Text);
        Assert.NotNull(invocation.TypeArguments);
        Assert.Single(invocation.TypeArguments.ArgumentsList);

        var typeArgument = invocation.TypeArguments.ArgumentsList.First();
        var primitive = Assert.IsType<PrimitiveType>(typeArgument);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
        Assert.NotNull(invocation.Arguments);
        Assert.Single(invocation.Arguments.ArgumentList);

        var argument = invocation.Arguments.ArgumentList.First();
        var literal = Assert.IsType<Literal>(argument);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Parses_Invocation_WithComplexExpressionAsCallee()
    {
        var tree = Utility.GetAST("(getFn())(x, y)");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(expressionStatement.Expression);
        var paren = Assert.IsType<Parenthesized>(invocation.Expression);
        var innerInvocation = Assert.IsType<Invocation>(paren.Expression);
        var identifier = Assert.IsType<Identifier>(innerInvocation.Expression);
        Assert.Equal("getFn", identifier.Name.Text);
        Assert.Empty(innerInvocation.Arguments.ArgumentList);
        Assert.Equal(2, invocation.Arguments.ArgumentList.Count);
        Assert.All(invocation.Arguments.ArgumentList, a => Assert.IsType<Identifier>(a));
    }

    [Fact]
    public void Parses_Invocation_Chained()
    {
        var tree = Utility.GetAST("foo(1, 2)(2)(3)");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var outer = Assert.IsType<Invocation>(stmt.Expression);
        Assert.Single(outer.Arguments.ArgumentList);
        var middle = Assert.IsType<Invocation>(outer.Expression);
        Assert.Single(middle.Arguments.ArgumentList);
        var inner = Assert.IsType<Invocation>(middle.Expression);
        Assert.Equal(2, inner.Arguments.ArgumentList.Count);
        Assert.IsType<Identifier>(inner.Expression);
    }

    [Fact]
    public void Parses_Invocation_WithBinaryOperatorAsCallee_Invalid()
    {
        var tree = Utility.GetAST("(a + b)()");
        var stmt = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(stmt.Expression);
        var paren = Assert.IsType<Parenthesized>(invocation.Expression);
        var binary = Assert.IsType<BinaryOperator>(paren.Expression);
        Assert.Equal(SyntaxKind.Plus, binary.Operator.Kind);
    }

    [Fact]
    public void Parses_Invocation_WithNestedTypeArguments()
    {
        var tree = Utility.GetAST("generic::<List<A<number>>>(items)");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(expressionStatement.Expression);
        Assert.NotNull(invocation.TypeArguments);
        Assert.Single(invocation.TypeArguments.ArgumentsList);
        Assert.Equal(SyntaxKind.ColonColonLArrow, invocation.TypeArguments.LeftArrow.Kind);
        Assert.Equal(SyntaxKind.RArrow, invocation.TypeArguments.RightArrow.Kind);

        var typeArgument = invocation.TypeArguments.ArgumentsList.First();
        var typeName = Assert.IsType<TypeName>(typeArgument);
        Assert.Equal("List", typeName.Name.Text);
        Assert.NotNull(typeName.TypeArguments);
        Assert.Single(typeName.TypeArguments.ArgumentsList);
        Assert.Equal(SyntaxKind.LArrow, typeName.TypeArguments.LeftArrow.Kind);
        Assert.Equal(SyntaxKind.RArrow, typeName.TypeArguments.RightArrow.Kind);

        var middleType = Assert.IsType<TypeName>(typeName.TypeArguments.ArgumentsList.First());
        Assert.Equal("A", middleType.Name.Text);
        Assert.NotNull(middleType.TypeArguments);
        Assert.Single(middleType.TypeArguments.ArgumentsList);
        Assert.Equal(SyntaxKind.LArrow, middleType.TypeArguments.LeftArrow.Kind);
        Assert.Equal(SyntaxKind.RArrow, middleType.TypeArguments.RightArrow.Kind);

        var innerType = middleType.TypeArguments.ArgumentsList.First();
        var primitive = Assert.IsType<PrimitiveType>(innerType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Theory]
    [InlineData("a + b", SyntaxKind.Plus)]
    [InlineData("a - b", SyntaxKind.Minus)]
    [InlineData("a * b", SyntaxKind.Star)]
    [InlineData("a / b", SyntaxKind.Slash)]
    [InlineData("a // b", SyntaxKind.SlashSlash)]
    [InlineData("a % b", SyntaxKind.Percent)]
    [InlineData("a ^ b", SyntaxKind.Caret)]
    [InlineData("a & b", SyntaxKind.Ampersand)]
    [InlineData("a | b", SyntaxKind.Pipe)]
    [InlineData("a ~ b", SyntaxKind.Tilde)]
    [InlineData("a << b", SyntaxKind.LArrowLArrow)]
    [InlineData("a >> b", SyntaxKind.RArrowRArrow)]
    [InlineData("a >>> b", SyntaxKind.RArrowRArrowRArrow)]
    [InlineData("a || b", SyntaxKind.PipePipe)]
    [InlineData("a && b", SyntaxKind.AmpersandAmpersand)]
    [InlineData("a ?? b", SyntaxKind.QuestionQuestion)]
    [InlineData("a < b", SyntaxKind.LArrow)]
    [InlineData("a <= b", SyntaxKind.LArrowEquals)]
    [InlineData("a > b", SyntaxKind.RArrow)]
    [InlineData("a >= b", SyntaxKind.RArrowEquals)]
    [InlineData("a == b", SyntaxKind.EqualsEquals)]
    [InlineData("a != b", SyntaxKind.BangEquals)]
    [InlineData("a = b", SyntaxKind.Equals, true)]
    [InlineData("a += b", SyntaxKind.PlusEquals, true)]
    [InlineData("a -= b", SyntaxKind.MinusEquals, true)]
    [InlineData("a *= b", SyntaxKind.StarEquals, true)]
    [InlineData("a /= b", SyntaxKind.SlashEquals, true)]
    [InlineData("a //= b", SyntaxKind.SlashSlashEquals, true)]
    [InlineData("a %= b", SyntaxKind.PercentEquals, true)]
    [InlineData("a ^= b", SyntaxKind.CaretEquals, true)]
    [InlineData("a &= b", SyntaxKind.AmpersandEquals, true)]
    [InlineData("a |= b", SyntaxKind.PipeEquals, true)]
    [InlineData("a ~= b", SyntaxKind.TildeEquals, true)]
    [InlineData("a >>= b", SyntaxKind.RArrowRArrowEquals, true)]
    [InlineData("a >>>= b", SyntaxKind.RArrowRArrowRArrowEquals, true)]
    [InlineData("a <<= b", SyntaxKind.LArrowLArrowEquals, true)]
    [InlineData("a &&= b", SyntaxKind.AmpersandAmpersandEquals, true)]
    [InlineData("a ||= b", SyntaxKind.PipePipeEquals, true)]
    [InlineData("a ??= b", SyntaxKind.QuestionQuestionEquals, true)]
    public void Parses_BinaryOperator(string source, SyntaxKind expectedOperator, bool isAssignment = false)
    {
        var tree = Utility.GetAST(source);
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var binaryOperator = Assert.IsAssignableFrom<BinaryOperator>(expressionStatement.Expression);
        if (isAssignment)
        {
            Assert.IsType<AssignmentOperator>(binaryOperator);
            Assert.IsAssignableFrom<AssignmentTarget>(binaryOperator.Left);
        }

        Assert.IsType<Identifier>(binaryOperator.Left);
        Assert.IsType<Identifier>(binaryOperator.Right);
        Assert.Equal(expectedOperator, binaryOperator.Operator.Kind);
    }

    [Theory]
    [InlineData("-69", SyntaxKind.Minus)]
    [InlineData("~420", SyntaxKind.Tilde)]
    [InlineData("!false", SyntaxKind.Bang)]
    public void Parses_UnaryOperator(string source, SyntaxKind expectedOperator)
    {
        var tree = Utility.GetAST(source);
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var unaryOperator = Assert.IsType<UnaryOperator>(expressionStatement.Expression);
        Assert.IsType<Literal>(unaryOperator.Operand);
        Assert.Equal(expectedOperator, unaryOperator.Operator.Kind);
    }

    [Fact]
    public void Parses_ArithmeticOperator_Precedence()
    {
        // (-a) + (b * ((~c) ^ d))
        var tree = Utility.GetAST("-a + b * ~c ^ d");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var addition = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
        Assert.Equal(SyntaxKind.Plus, addition.Operator.Kind);

        var minusA = Assert.IsType<UnaryOperator>(addition.Left);
        Assert.IsType<Identifier>(minusA.Operand);
        Assert.Equal(SyntaxKind.Minus, minusA.Operator.Kind);

        var multiplication = Assert.IsType<BinaryOperator>(addition.Right);
        Assert.Equal(SyntaxKind.Star, multiplication.Operator.Kind);
        Assert.IsType<Identifier>(multiplication.Left);

        var exponentiation = Assert.IsType<BinaryOperator>(multiplication.Right);
        Assert.Equal(SyntaxKind.Caret, exponentiation.Operator.Kind);

        var tildeC = Assert.IsType<UnaryOperator>(exponentiation.Left);
        Assert.IsType<Identifier>(tildeC.Operand);
        Assert.Equal(SyntaxKind.Tilde, tildeC.Operator.Kind);
        Assert.IsType<Identifier>(exponentiation.Right);
    }

    [Fact]
    public void Parses_Parenthesized()
    {
        var tree = Utility.GetAST("(69)");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var parenthesized = Assert.IsType<Parenthesized>(expressionStatement.Expression);
        var literal = Assert.IsType<Literal>(parenthesized.Expression);
        Assert.Equal(69L, literal.Value);
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("ball_sack")]
    [InlineData("siGmA12Df32")]
    public void Parses_Identifiers(string source)
    {
        var tree = Utility.GetAST(source);
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var identifier = Assert.IsType<Identifier>(expressionStatement.Expression);
        Assert.Equal(source, identifier.Name.Text);
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