using Loom.Diagnostics;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;

namespace Loom.Testing;

[Collection("Assembly")]
public class TypeCheckerTest
{
    [Fact]
    public void ThrowsFor_Variable_DeclaredType_Mismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: number = 'hello'");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"hello\"' is not assignable to type 'number'.");
    }
    
    [Fact]
    public void ThrowsFor_Assignment_DeclaredType_Mismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("mut x = 69; x = false");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type 'false' is not assignable to type 'number'.");
    }

    [Fact]
    public void ThrowsFor_GenericTypeMismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("type Id<T> = T; let x: Id<number> = 'hello'");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"hello\"' is not assignable to type 'number'.");
    }

    [Fact]
    public void ThrowsFor_UndefinedIdentifier()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("x");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindSymbol, "Cannot find symbol for declaration of variable 'x'.");
    }

    [Fact]
    public void ThrowsFor_UndefinedType()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: A = 1");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindSymbol, "Cannot find symbol for declaration of type 'A'.");
    }

    [Fact]
    public void ThrowsFor_NonGeneric()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("type A = number; let x: A<number> = 1");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.NotGeneric, "Type 'A' is not generic and cannot receive type arguments.");
    }

    [Fact]
    public void ThrowsFor_IncorrectGenericArity()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("type A<T> = T; let x: A<number, bool> = 1");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.GenericArity, "Type 'A<T>' expects 1 type argument, but 2 were provided.");
    }

    [Theory]
    [InlineData("1 + true")]
    [InlineData("true + 1")]
    [InlineData("'abc' + 69")]
    [InlineData("'hello' - 'world'")]
    [InlineData("1 - 'hello'")]
    [InlineData("true * false")]
    [InlineData("1 / true")]
    [InlineData("1 // '2'")]
    [InlineData("5 % '3'")]
    [InlineData("1 ^ true")]
    [InlineData("1 & '2'")]
    [InlineData("1 | true")]
    [InlineData("1 << '2'")]
    [InlineData("1 >> true")]
    [InlineData("1 >>> '2'")]
    [InlineData("true && 5")]
    [InlineData("'hello' && false")]
    [InlineData("5 || 'world'")]
    [InlineData("true || 42")]
    public void ThrowsFor_BinaryOperator_InvalidOperandTypes(string source)
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Assert.Contains(diagnostics.Set, d => d.Code is InternalCodes.TypeMismatch or InternalCodes.InvalidBinaryOp);
    }
    
    [Theory]
    [InlineData("!5")]
    [InlineData("!'hello'")]
    [InlineData("!42")]
    [InlineData("~true")]
    [InlineData("~'hello'")]
    [InlineData("-true")]
    [InlineData("-'hello'")]
    [InlineData("-false")]
    public void ThrowsFor_UnaryOperator_InvalidOperandType(string source)
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Assert.Contains(diagnostics.Set, d => d.Code is InternalCodes.TypeMismatch or InternalCodes.InvalidUnaryOp);
    }

    [Theory]
    [InlineData("1 == '1'", "bool")]
    [InlineData("1 != '1'", "bool")]
    public void Checks_BinaryOperator_WithMixedTypes_ReturnsExpectedType(string source, string expectedTypeName)
    {
        var type = Utility.GetLastStatementType(source);
        var expectedType = expectedTypeName switch
        {
            "string" => PrimitiveType.String,
            "bool" => PrimitiveType.Bool,
            _ => PrimitiveType.Never
        };

        Assert.True(
            type.Equals(expectedType),
            $"Expected '{expectedTypeName}', got '{type}' for expression '{source}'"
        );
    }

    [Theory]
    [InlineData("!true", "bool")]  // logical not on bool -> bool
    [InlineData("~5", "number")]   // bitwise not on number -> number
    [InlineData("~0", "number")]   // bitwise not on number -> number
    [InlineData("-5", "number")]   // unary minus on number -> number
    [InlineData("-0", "number")]   // unary minus on number -> number
    [InlineData("-(5)", "number")] // unary minus on parenthesized number -> number
    public void Checks_UnaryOperator_ValidOperand_ReturnsExpectedType(string source, string expectedTypeName)
    {
        var type = Utility.GetLastStatementType(source);
        var expectedType = expectedTypeName == "bool" ? PrimitiveType.Bool : PrimitiveType.Number;
        Assert.True(type.Equals(expectedType), $"Expected '{expectedTypeName}', got '{type}' for expression '{source}'");
    }

    [Theory]
    [InlineData("type A = number")]
    [InlineData("type A = number; let x: A = 1")]
    [InlineData("type Id<T> = T; let x: Id<number> = 1")]
    [InlineData("type Id<T> = T; type X = Id<number>; let x: X = 1")]
    [InlineData("type Id<T> = T; let x: Id<number> = 1; x")]
    [InlineData("type Id<T> = T; let x = 1; let y: Id<number> = x; y")]
    [InlineData("type Const<A, B> = A; let x: Const<number, string> = 1")]
    [InlineData("type Id<T> = T; type NumId = Id<number>; type X = NumId; let x: X = 1")]
    public void Checks_TypeAlias_Resolution(string source)
    {
        var narrowType = Utility.GetLastStatementType(source);
        var type = narrowType is InstantiatedType instantiated ? instantiated.Expand() : narrowType;
        Assert.True(
            type.Equals(PrimitiveType.Number),
            $"Expected 'number', got '{type}'"
        );
    }
    
    [Fact]
    public void Checks_Assignment_Resolution()
    {
        var type = Utility.GetLastStatementType("mut x = 42; x = 69");
        var literal = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, literal.Kind);
    }

    [Theory]
    [InlineData("let x = 42; x")]
    [InlineData("let x = 42; let y = x; y")]
    [InlineData("let x = 42; let y = x; let z = y; z;")]
    public void Checks_Identifier_Resolution(string source)
    {
        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_Identifier_ResolvesAnnotatedType()
    {
        var type = Utility.GetLastStatementType("let x: number = 42; x;");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_VariableDeclaration_WidenedInference()
    {
        var type = Utility.GetLastStatementType("mut x = 42");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_VariableDeclaration_Inference()
    {
        var constType = Utility.GetLastStatementType("let x = 42");
        var literal = Assert.IsType<LiteralType>(constType);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_VariableDeclaration_WithTypeAnnotation()
    {
        var type = Utility.GetLastStatementType("let x: number = 42");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }
    
    [Fact]
    public void Checks_LiteralTypes()
    {
        var type = Utility.GetLastStatementType("let x: 69 = 69; x;");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(69L, literal.Value);
    }

    [Fact]
    public void Checks_NumberLiterals()
    {
        var type = Utility.GetLastStatementType("69");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(69L, literal.Value);
    }

    [Fact]
    public void Checks_StringLiterals()
    {
        var type = Utility.GetLastStatementType("\"hello\"");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal("hello", literal.Value);
    }

    [Fact]
    public void Checks_BoolLiterals()
    {
        var trueType = Utility.GetLastStatementType("true");
        var trueLiteral = Assert.IsType<LiteralType>(trueType);
        Assert.Equal(true, trueLiteral.Value);

        var falseType = Utility.GetLastStatementType("false");
        var falseLiteral = Assert.IsType<LiteralType>(falseType);
        Assert.Equal(false, falseLiteral.Value);
    }
}