using Loom.Diagnostics;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;

namespace Loom.Testing;

public class TypeCheckerTest
{
    [Fact]
    public void ThrowsFor_TypeMismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: number = 'hello'");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"hello\"' is not assignable to type 'number'.");
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
        Utility.AssertDiagnostic(diagnostics, InternalCodes.GenericArity, $"Type 'A' expects 1 type argument(s), but 2 were provided.");
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

    [Theory]
    [InlineData("let x = 42; x")]
    [InlineData("let x = 42; let y = x; y")]
    [InlineData("let x = 42; let y = x; let z = y; z;")]
    public void Checks_Identifier_Resolution(string source)
    {
        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42, literal.Value);
    }

    [Fact]
    public void Checks_Identifier_ResolvesAnnotatedType()
    {
        var type = Utility.GetLastStatementType("let x: number = 42; x");
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
        Assert.Equal(42, literal.Value);
    }

    [Fact]
    public void Checks_VariableDeclaration_WithTypeAnnotation()
    {
        var type = Utility.GetLastStatementType("let x: number = 42");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_NumberLiterals()
    {
        var type = Utility.GetLastStatementType("69");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(69, literal.Value);
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