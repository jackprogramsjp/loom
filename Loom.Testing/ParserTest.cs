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
        Assert.Single(alias.TypeParameters.Parameters);

        var param = alias.TypeParameters.Parameters.First();
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
        Assert.Equal(2, alias.TypeParameters.Parameters.Count);

        var a = alias.TypeParameters.Parameters.First();
        var b = alias.TypeParameters.Parameters.Last();
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