using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using Microsoft.VisualBasic.CompilerServices;
using ArrayType = Loom.Parsing.AST.ArrayType;
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
    public void ThrowsFor_UnexpectedEof()
    {
        var diagnostics = Utility.GetParserDiagnostics("!");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedEof, "Unexpected end of file.");
    }
    
    [Fact]
    public void ThrowsFor_UnexpectedToken()
    {
        var diagnostics = Utility.GetParserDiagnostics("if )");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedToken, "Unexpected token.");
    }

    [Fact]
    public void ThrowsFor_InvalidNameOf()
    {
        var diagnostics = Utility.GetParserDiagnostics("nameof(123)");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidNameOf, "'123' is not a valid name.");
    }

    [Theory]
    [InlineData("(1 + 2")]
    [InlineData("(1 + 2]", "']'")]
    public void ThrowsFor_UnterminatedParens(string source, string got = "EOF")
    {
        var diagnostics = Utility.GetParserDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            got == "EOF" ? InternalCodes.UnexpectedEof : InternalCodes.UnexpectedToken,
            $"Expected ')' here to close '(' at character 0, got {got}."
        );
    }

    [Fact]
    public void ThrowsFor_UnterminatedBrackets()
    {
        var diagnostics = Utility.GetParserDiagnostics("arr[0");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedEof, "Expected ']', got EOF.");
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
    public void ThrowsFor_DeclarationOutsideOfBlock_InIfThenBranch()
    {
        var diagnostics = Utility.GetParserDiagnostics("if true let x = 42");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.DeclarationOutsideOfBlock,
            "Declarations can only be declared inside of a block.",
            "surround with '{' and '}'"
        );
    }

    [Fact]
    public void ThrowsFor_DeclarationOutsideOfBlock_InIfElseBranch()
    {
        var diagnostics = Utility.GetParserDiagnostics("if true { return 1 } else let x = 42");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.DeclarationOutsideOfBlock,
            "Declarations can only be declared inside of a block.",
            "surround with '{' and '}'"
        );
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

    [Fact]
    public void Parses_IfElseStatement_WithExpressionElseBranch()
    {
        var tree = Utility.GetAST("if condition thenBranch else elseBranch");
        Assert.Single(tree.Statements);

        var ifStatement = Assert.IsType<If>(tree.Statements.First());
        var thenBranch = Assert.IsType<ExpressionStatement>(ifStatement.ThenBranch);
        var elseBranch = Assert.IsType<ExpressionStatement>(ifStatement.ElseBranch!.Branch);

        Assert.IsType<Identifier>(thenBranch.Expression);
        Assert.IsType<Identifier>(elseBranch.Expression);
    }

    [Fact]
    public void Parses_IfStatement_WithComplexCondition()
    {
        var tree = Utility.GetAST("if x > 5 && y < 10 { return 1 }");
        Assert.Single(tree.Statements);

        var ifStatement = Assert.IsType<If>(tree.Statements.First());
        var condition = Assert.IsType<BinaryOperator>(ifStatement.Condition);
        Assert.Equal(SyntaxKind.AmpersandAmpersand, condition.Operator.Kind);

        var left = Assert.IsType<BinaryOperator>(condition.Left);
        var right = Assert.IsType<BinaryOperator>(condition.Right);
        Assert.Equal(SyntaxKind.RArrow, left.Operator.Kind);
        Assert.Equal(SyntaxKind.LArrow, right.Operator.Kind);
    }

    [Fact]
    public void Parses_NestedIfStatements()
    {
        var tree = Utility.GetAST("if x > 0 { if y > 0 { return 1 } }");
        Assert.Single(tree.Statements);

        var outerIf = Assert.IsType<If>(tree.Statements.First());
        var outerThen = Assert.IsType<Block>(outerIf.ThenBranch);
        Assert.Single(outerThen.Statements);

        var innerIf = Assert.IsType<If>(outerThen.Statements.First());
        var innerCondition = Assert.IsType<BinaryOperator>(innerIf.Condition);
        Assert.Equal(SyntaxKind.RArrow, innerCondition.Operator.Kind);

        var innerThen = Assert.IsType<Block>(innerIf.ThenBranch);
        Assert.Single(innerThen.Statements);
        Assert.IsType<Return>(innerThen.Statements.First());
    }

    [Fact]
    public void Parses_IfStatement_WithVariableDeclarationInside()
    {
        var result = Utility.Parse("if true { let x = 42 }");
        Utility.AssertNoErrors(result);
        Assert.Single(result.Tree.Statements);

        var ifStatement = Assert.IsType<If>(result.Tree.Statements.First());
        var thenBranch = Assert.IsType<Block>(ifStatement.ThenBranch);
        Assert.Single(thenBranch.Statements);
        Assert.IsType<VariableDeclaration>(thenBranch.Statements.First());
    }

    [Fact]
    public void Parses_IfElseIfChain_WithMultipleElseIfBranches()
    {
        var tree = Utility.GetAST("if a { return 1 } else if b { return 2 } else if c { return 3 } else { return 4 }");
        Assert.Single(tree.Statements);

        var firstIf = Assert.IsType<If>(tree.Statements.First());
        Assert.IsType<Identifier>(firstIf.Condition);
        Assert.IsType<Block>(firstIf.ThenBranch);

        Assert.NotNull(firstIf.ElseBranch);
        var secondIf = Assert.IsType<If>(firstIf.ElseBranch.Branch);
        Assert.IsType<Identifier>(secondIf.Condition);

        Assert.NotNull(secondIf.ElseBranch);
        var thirdIf = Assert.IsType<If>(secondIf.ElseBranch.Branch);
        Assert.IsType<Identifier>(thirdIf.Condition);

        Assert.NotNull(thirdIf.ElseBranch);
        var elseBranch = Assert.IsType<Block>(thirdIf.ElseBranch.Branch);
        Assert.Single(elseBranch.Statements);
    }

    [Fact]
    public void Parses_IfStatement_WithMultipleStatementsInBlock()
    {
        var tree = Utility.GetAST("if true { let x = 1; let y = 2; return x + y }");
        Assert.Single(tree.Statements);

        var ifStatement = Assert.IsType<If>(tree.Statements.First());
        var thenBranch = Assert.IsType<Block>(ifStatement.ThenBranch);
        Assert.Equal(3, thenBranch.Statements.Count);

        Assert.IsType<VariableDeclaration>(thenBranch.Statements[0]);
        Assert.IsType<VariableDeclaration>(thenBranch.Statements[1]);
        Assert.IsType<Return>(thenBranch.Statements[2]);
    }

    [Fact]
    public void Parses_IfStatement_WithNoElseBranch()
    {
        var tree = Utility.GetAST("if condition { return 42 }");
        Assert.Single(tree.Statements);

        var ifStatement = Assert.IsType<If>(tree.Statements.First());
        Assert.Null(ifStatement.ElseBranch);
    }

    [Fact]
    public void Parses_IfStatement_WithEmptyThenBranch()
    {
        var tree = Utility.GetAST("if condition { }");
        Assert.Single(tree.Statements);

        var ifStatement = Assert.IsType<If>(tree.Statements.First());
        var thenBranch = Assert.IsType<Block>(ifStatement.ThenBranch);
        Assert.Empty(thenBranch.Statements);
    }

    [Fact]
    public void Parses_IfStatement_WithEmptyElseBranch()
    {
        var tree = Utility.GetAST("if condition { return 1 } else { }");
        Assert.Single(tree.Statements);

        var ifStatement = Assert.IsType<If>(tree.Statements.First());
        Assert.NotNull(ifStatement.ElseBranch);

        var elseBranch = Assert.IsType<Block>(ifStatement.ElseBranch.Branch);
        Assert.Empty(elseBranch.Statements);
    }

    [Fact]
    public void Parses_EnumDeclaration_WithImplicitValues()
    {
        var tree = Utility.GetAST("enum Abc { A, B, C }");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var enumDecl = Assert.IsType<EnumDeclaration>(statement);
        Assert.Equal("Abc", enumDecl.Name.Text);
        Assert.Equal(SyntaxKind.EnumKeyword, enumDecl.Keyword.Kind);
        Assert.Null(enumDecl.ColonTypeClause);
        Assert.Equal(SyntaxKind.LBrace, enumDecl.LeftBrace.Kind);
        Assert.Equal(SyntaxKind.RBrace, enumDecl.RightBrace.Kind);
        Assert.Equal(3, enumDecl.Members.Count);

        Assert.Equal("A", enumDecl.Members[0].Name.Text);
        Assert.Null(enumDecl.Members[0].EqualsValueClause);

        Assert.Equal("B", enumDecl.Members[1].Name.Text);
        Assert.Null(enumDecl.Members[1].EqualsValueClause);

        Assert.Equal("C", enumDecl.Members[2].Name.Text);
        Assert.Null(enumDecl.Members[2].EqualsValueClause);
    }

    [Fact]
    public void Parses_EnumDeclaration_WithExplicitValues()
    {
        var tree = Utility.GetAST("enum Status { Active = 1, Inactive = 0, Pending = 2 }");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var enumDecl = Assert.IsType<EnumDeclaration>(statement);
        Assert.Equal("Status", enumDecl.Name.Text);
        Assert.Equal(3, enumDecl.Members.Count);

        Assert.Equal("Active", enumDecl.Members[0].Name.Text);
        Assert.NotNull(enumDecl.Members[0].EqualsValueClause);
        var activeValue = Assert.IsType<Literal>(enumDecl.Members[0].EqualsValueClause!.Value);
        Assert.Equal(1L, activeValue.Value);

        Assert.Equal("Inactive", enumDecl.Members[1].Name.Text);
        Assert.NotNull(enumDecl.Members[1].EqualsValueClause);
        var inactiveValue = Assert.IsType<Literal>(enumDecl.Members[1].EqualsValueClause!.Value);
        Assert.Equal(0L, inactiveValue.Value);

        Assert.Equal("Pending", enumDecl.Members[2].Name.Text);
        Assert.NotNull(enumDecl.Members[2].EqualsValueClause);
        var pendingValue = Assert.IsType<Literal>(enumDecl.Members[2].EqualsValueClause!.Value);
        Assert.Equal(2L, pendingValue.Value);
    }

    [Fact]
    public void Parses_EnumDeclaration_WithMixedImplicitAndExplicitValues()
    {
        var tree = Utility.GetAST("enum Mixed { A, B = 69, C }");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var enumDecl = Assert.IsType<EnumDeclaration>(statement);
        Assert.Equal("Mixed", enumDecl.Name.Text);
        Assert.Equal(3, enumDecl.Members.Count);

        Assert.Equal("A", enumDecl.Members[0].Name.Text);
        Assert.Null(enumDecl.Members[0].EqualsValueClause);

        Assert.Equal("B", enumDecl.Members[1].Name.Text);
        Assert.NotNull(enumDecl.Members[1].EqualsValueClause);
        var bValue = Assert.IsType<Literal>(enumDecl.Members[1].EqualsValueClause!.Value);
        Assert.Equal(69L, bValue.Value);

        Assert.Equal("C", enumDecl.Members[2].Name.Text);
        Assert.Null(enumDecl.Members[2].EqualsValueClause);
    }

    [Fact]
    public void Parses_EnumDeclaration_WithStringBackedValues()
    {
        var tree = Utility.GetAST("enum Colors : string { Red = \"FF0000\", Green = \"00FF00\", Blue = \"0000FF\" }");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var enumDecl = Assert.IsType<EnumDeclaration>(statement);
        Assert.Equal("Colors", enumDecl.Name.Text);
        Assert.NotNull(enumDecl.ColonTypeClause);
        var baseType = Assert.IsType<PrimitiveType>(enumDecl.ColonTypeClause.Type);
        Assert.Equal(PrimitiveTypeKind.String, baseType.Kind);
        Assert.Equal(3, enumDecl.Members.Count);

        Assert.Equal("Red", enumDecl.Members[0].Name.Text);
        Assert.NotNull(enumDecl.Members[0].EqualsValueClause);
        var redValue = Assert.IsType<Literal>(enumDecl.Members[0].EqualsValueClause!.Value);
        Assert.Equal("FF0000", redValue.Value);

        Assert.Equal("Green", enumDecl.Members[1].Name.Text);
        Assert.NotNull(enumDecl.Members[1].EqualsValueClause);
        var greenValue = Assert.IsType<Literal>(enumDecl.Members[1].EqualsValueClause!.Value);
        Assert.Equal("00FF00", greenValue.Value);

        Assert.Equal("Blue", enumDecl.Members[2].Name.Text);
        Assert.NotNull(enumDecl.Members[2].EqualsValueClause);
        var blueValue = Assert.IsType<Literal>(enumDecl.Members[2].EqualsValueClause!.Value);
        Assert.Equal("0000FF", blueValue.Value);
    }

    [Fact]
    public void Parses_EmptyEnumDeclaration()
    {
        var tree = Utility.GetAST("enum Empty { }");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var enumDecl = Assert.IsType<EnumDeclaration>(statement);
        Assert.Equal("Empty", enumDecl.Name.Text);
        Assert.Empty(enumDecl.Members);
        Assert.Equal(SyntaxKind.LBrace, enumDecl.LeftBrace.Kind);
        Assert.Equal(SyntaxKind.RBrace, enumDecl.RightBrace.Kind);
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
    public void Parses_QualifiedName_Basic()
    {
        var tree = Utility.GetAST("a.b");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var qualifiedName = Assert.IsType<QualifiedName>(expressionStatement.Expression);
        Assert.Equal("a", qualifiedName.Identifier.Name.Text);
        Assert.Single(qualifiedName.Names);

        var name = qualifiedName.Names.First();
        Assert.Equal("b", name.Name.Text);
        Assert.Equal(SyntaxKind.Identifier, name.Name.Kind);
        Assert.Equal(SyntaxKind.Dot, name.Dot.Kind);
    }

    [Fact]
    public void Parses_PropertyAccess_Basic()
    {
        var tree = Utility.GetAST("\'a\'.b");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var propertyAccess = Assert.IsType<PropertyAccess>(expressionStatement.Expression);
        var literal = Assert.IsType<Literal>(propertyAccess.Expression);
        Assert.Equal("a", literal.Value);
        Assert.Single(propertyAccess.Names);

        var name = propertyAccess.Names.First();
        Assert.Equal("b", name.Name.Text);
        Assert.Equal(SyntaxKind.Identifier, name.Name.Kind);
        Assert.Equal(SyntaxKind.Dot, name.Dot.Kind);
    }

    [Fact]
    public void Parses_ElementAccess_Basic()
    {
        var tree = Utility.GetAST("arr[0]");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var elementAccess = Assert.IsType<ElementAccess>(expressionStatement.Expression);
        Assert.Equal(SyntaxKind.LBracket, elementAccess.LeftBracket.Kind);
        Assert.Equal(SyntaxKind.RBracket, elementAccess.RightBracket.Kind);

        var identifier = Assert.IsType<Identifier>(elementAccess.Expression);
        Assert.Equal("arr", identifier.Name.Text);

        var index = Assert.IsType<Literal>(elementAccess.IndexExpression);
        Assert.Equal(0L, index.Value);

        Assert.Equal(SyntaxKind.LBracket, elementAccess.LeftBracket.Kind);
        Assert.Equal(SyntaxKind.RBracket, elementAccess.RightBracket.Kind);
    }

    [Fact]
    public void Parses_ElementAccess_WithExpressionIndex()
    {
        var tree = Utility.GetAST("arr[i + 1]");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var elementAccess = Assert.IsType<ElementAccess>(expressionStatement.Expression);

        var identifier = Assert.IsType<Identifier>(elementAccess.Expression);
        Assert.Equal("arr", identifier.Name.Text);

        var binary = Assert.IsType<BinaryOperator>(elementAccess.IndexExpression);
        Assert.Equal(SyntaxKind.Plus, binary.Operator.Kind);

        var left = Assert.IsType<Identifier>(binary.Left);
        var right = Assert.IsType<Literal>(binary.Right);
        Assert.Equal("i", left.Name.Text);
        Assert.Equal(1L, right.Value);
    }

    [Fact]
    public void Parses_ElementAccess_AsAssignmentTarget()
    {
        var tree = Utility.GetAST("arr[0] = 42");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var assignment = Assert.IsType<AssignmentOperator>(expressionStatement.Expression);

        var elementAccess = Assert.IsType<ElementAccess>(assignment.Left);
        var identifier = Assert.IsType<Identifier>(elementAccess.Expression);
        Assert.Equal("arr", identifier.Name.Text);

        var index = Assert.IsType<Literal>(elementAccess.IndexExpression);
        Assert.Equal(0L, index.Value);

        var value = Assert.IsType<Literal>(assignment.Right);
        Assert.Equal(42L, value.Value);
    }

    [Fact]
    public void Parses_ElementAccess_Chained()
    {
        var tree = Utility.GetAST("arr[0][1][2]");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());

        var outer = Assert.IsType<ElementAccess>(expressionStatement.Expression);
        var outerIndex = Assert.IsType<Literal>(outer.IndexExpression);
        Assert.Equal(2L, outerIndex.Value);

        var middle = Assert.IsType<ElementAccess>(outer.Expression);
        var middleIndex = Assert.IsType<Literal>(middle.IndexExpression);
        Assert.Equal(1L, middleIndex.Value);

        var inner = Assert.IsType<ElementAccess>(middle.Expression);
        var innerIndex = Assert.IsType<Literal>(inner.IndexExpression);
        Assert.Equal(0L, innerIndex.Value);

        var identifier = Assert.IsType<Identifier>(inner.Expression);
        Assert.Equal("arr", identifier.Name.Text);
    }

    [Fact]
    public void Parses_ElementAccess_WithInvocation()
    {
        var tree = Utility.GetAST("getArr()[0]");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var elementAccess = Assert.IsType<ElementAccess>(expressionStatement.Expression);

        var invocation = Assert.IsType<Invocation>(elementAccess.Expression);
        var identifier = Assert.IsType<Identifier>(invocation.Expression);
        Assert.Equal("getArr", identifier.Name.Text);

        var index = Assert.IsType<Literal>(elementAccess.IndexExpression);
        Assert.Equal(0L, index.Value);
    }

    [Fact]
    public void Parses_ElementAccess_WithNestedInvocation()
    {
        var tree = Utility.GetAST("arr[0]()");
        var expressionStatement = Assert.IsType<ExpressionStatement>(tree.Statements.Single());
        var invocation = Assert.IsType<Invocation>(expressionStatement.Expression);

        var elementAccess = Assert.IsType<ElementAccess>(invocation.Expression);
        var identifier = Assert.IsType<Identifier>(elementAccess.Expression);
        Assert.Equal("arr", identifier.Name.Text);

        var index = Assert.IsType<Literal>(elementAccess.IndexExpression);
        Assert.Equal(0L, index.Value);
        Assert.Empty(invocation.Arguments.ArgumentList);
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

    [Fact]
    public void Parses_RangeLiteral()
    {
        var tree = Utility.GetAST("1..5");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var rangeLiteral = Assert.IsType<RangeLiteral>(expressionStatement.Expression);
        Assert.Equal(1L, Assert.IsType<Literal>(rangeLiteral.Minimum).Value);
        Assert.Equal(5L, Assert.IsType<Literal>(rangeLiteral.Maximum).Value);
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
    public void Parses_Mutable_ArrayLiteral()
    {
        var tree = Utility.GetAST("let x = mut [69, 420]");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var variable = Assert.IsType<VariableDeclaration>(statement);
        Assert.NotNull(variable.EqualsValueClause);

        var array = Assert.IsType<ArrayLiteral>(variable.EqualsValueClause.Value);
        Assert.Equal(2, array.Expressions.Count);
        Assert.NotNull(array.MutKeyword);
        Assert.Equal(SyntaxKind.MutKeyword, array.MutKeyword.Kind);

        var firstLiteral = Assert.IsType<Literal>(array.Expressions.First());
        var lastLiteral = Assert.IsType<Literal>(array.Expressions.Last());
        Assert.Equal(69L, firstLiteral.Value);
        Assert.Equal(420L, lastLiteral.Value);
    }

    [Fact]
    public void Parses_Nested_ArrayLiteral()
    {
        var tree = Utility.GetAST("[69, 420, [1, 2, 3]]");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var array = Assert.IsType<ArrayLiteral>(expressionStatement.Expression);
        Assert.Equal(3, array.Expressions.Count);

        var firstLiteral = Assert.IsType<Literal>(array.Expressions[0]);
        var lastLiteral = Assert.IsType<Literal>(array.Expressions[1]);
        Assert.Equal(69L, firstLiteral.Value);
        Assert.Equal(420L, lastLiteral.Value);

        var nestedArray = Assert.IsType<ArrayLiteral>(array.Expressions[2]);
        Assert.Equal(3, nestedArray.Expressions.Count);

        var firstNestedLiteral = Assert.IsType<Literal>(nestedArray.Expressions[0]);
        var secondNestedLiteral = Assert.IsType<Literal>(nestedArray.Expressions[1]);
        var thirdNestedLiteral = Assert.IsType<Literal>(nestedArray.Expressions[2]);
        Assert.Equal(1L, firstNestedLiteral.Value);
        Assert.Equal(2L, secondNestedLiteral.Value);
        Assert.Equal(3L, thirdNestedLiteral.Value);
    }

    [Fact]
    public void Parses_ArrayLiteral()
    {
        var tree = Utility.GetAST("[69, 420]");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var array = Assert.IsType<ArrayLiteral>(expressionStatement.Expression);
        Assert.Equal(2, array.Expressions.Count);
        Assert.Null(array.MutKeyword);
        Assert.Equal(SyntaxKind.LBracket, array.LeftBracket.Kind);
        Assert.Equal(SyntaxKind.RBracket, array.RightBracket.Kind);

        var firstLiteral = Assert.IsType<Literal>(array.Expressions.First());
        var lastLiteral = Assert.IsType<Literal>(array.Expressions.Last());
        Assert.Equal(69L, firstLiteral.Value);
        Assert.Equal(420L, lastLiteral.Value);
    }

    [Fact]
    public void Parses_NameOf()
    {
        var tree = Utility.GetAST("nameof(abc)");
        Assert.Single(tree.Statements);

        var statement = tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(statement);
        var nameOf = Assert.IsType<NameOf>(expressionStatement.Expression);
        Assert.Equal(SyntaxKind.NameOfKeyword, nameOf.Keyword.Kind);
        Assert.Equal(SyntaxKind.LParen, nameOf.LeftParen.Kind);
        Assert.Equal(SyntaxKind.RParen, nameOf.RightParen.Kind);
        Assert.Equal(SyntaxKind.Identifier, nameOf.Name.Token.Kind);
        Assert.Equal("abc", nameOf.Name.Token.Text);
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