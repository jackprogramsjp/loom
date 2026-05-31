using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using Microsoft.VisualBasic.CompilerServices;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Type = System.Type;
using TypeName = Loom.Parsing.AST.TypeName;
using UnionType = Loom.Parsing.AST.UnionType;

namespace Loom.Testing;

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
    [InlineData("a ^ b", SyntaxKind.Carat)]
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
    public void Parses_BinaryOperator(string source, SyntaxKind expectedOperator)
    {
        var tree = Utility.GetAST(source);
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var binaryOperator = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
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
        Assert.Equal(SyntaxKind.Carat, exponentiation.Operator.Kind);

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
        Assert.Equal(69, literal.Value);
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
    [InlineData("123", typeof(int))]
    [InlineData("420.69", typeof(double))]
    [InlineData("'hello'", typeof(string))]
    [InlineData("\"abc\"", typeof(string))]
    [InlineData("true", typeof(bool))]
    [InlineData("false", typeof(bool))]
    [InlineData("none", null)]
    public void Parses_Literals(string source, Type? expectedType)
    {
        var tree = Utility.GetAST(source);
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var literal = Assert.IsType<Literal>(expressionStatement.Expression);
        Assert.Equal(source, literal.Token.Text);
        if (expectedType != null)
            Assert.IsType(expectedType, literal.Value);
        else
            Assert.Null(literal.Value);
    }
}