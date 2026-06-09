using Loom.Diagnostics;
using Loom.TypeChecking.Types;

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
    
    [Fact]
    public void ThrowsFor_MismatchedDefaultType()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("type Id<T: number = \"abc\"> = T");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"abc\"' is not assignable to type 'number'.");
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

    [Fact]
    public void ThrowsFor_NonGenericFunctionCall_ArgumentTypeMismatch()
    {
        const string source = """
            fn add(a: number, b: number) -> a + b
            add(1, "two")
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.TypeMismatch,
            "Type '\"two\"' is not assignable to type 'number'."
        );
    }

    [Fact]
    public void ThrowsFor_GenericFunctionCall_InferenceConflict()
    {
        const string source = """
            fn first<T>(a: T, b: T) -> a
            first(42, "hello")
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InferredGenericConflict, "Inferred type '\"hello\"' for parameter 'T' conflicts with previous '42'.");
    }

    [Fact]
    public void ThrowsFor_FunctionCall_IncorrectGenericArity()
    {
        const string source = """
            fn id<T>(value: T) -> value
            id::<number, string>(69)
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.GenericArity,
            "Function expects 1 type argument, but 2 were provided."
        );
    }

    [Fact]
    public void ThrowsFor_FunctionCall_IncorrectArity()
    {
        const string source = """
            fn id(value: number) -> value
            id(69, 420)
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.InvocationArity,
            "Function expects 1 argument, but 2 were provided."
        );
    }

    [Fact]
    public void ThrowsFor_FunctionCall_WithOptionalParams_IncorrectArity()
    {
        const string source = """
            fn id(value: number, other: number?) -> value
            id(69, 420, 1337)
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.InvocationArity,
            "Function expects 1-2 arguments, but 3 were provided."
        );
    }

    [Fact]
    public void ThrowsFor_GenericFunctionCall_ExplicitTypeArgumentMismatch()
    {
        const string source = """
            fn id<T>(value: T) -> value
            id::<string>(69)
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.TypeMismatch,
            "Type '69' is not assignable to type 'string'."
        );
    }

    [Fact]
    public void ThrowsFor_GenericFunctionCall_WithConstraintViolation()
    {
        const string source = """
            fn identity<T: number>(value: T) -> value
            identity("hello")
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.ConstraintViolation,
            "Type '\"hello\"' does not satisfy constraint 'number' for type parameter 'T'."
        );
    }

    [Fact]
    public void ThrowsFor_GenericTypeAlias_WithConstraintViolation()
    {
        const string source = """
            type Box<T: number> = T
            let x: Box<string> = "hello"
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.ConstraintViolation,
            "Type 'string' does not satisfy constraint 'number' for type parameter 'T'."
        );
    }

    [Fact]
    public void ThrowsFor_GenericTypeAlias_MissingRequiredTypeParameter()
    {
        const string source = """
            type Id<T, U = number> = T
            type X = Id
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(diagnostics, InternalCodes.GenericArity, "Type 'Id<T, U = number>' expects 1-2 type arguments, but 0 were provided.");
    }

    [Fact]
    public void ThrowsFor_GenericFunctionCall_MissingRequiredTypeParameter()
    {
        const string source = """
            fn identity<T, U = number>(value: T?) -> value
            identity()
            """;

        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.CannotInferType,
            "Cannot infer type parameter 'T'. Provide explicit type arguments."
        );
    }

    [Fact]
    public void Checks_GenericFunctionCall_WithRequiredTypeParameter()
    {
        const string source = """
            fn identity<T, U = number>(value: T?) -> value
            identity::<number>()
            """;

        var type = Utility.GetLastStatementType(source);
        var optional = Assert.IsType<OptionalType>(type);
        var primitive = Assert.IsType<PrimitiveType>(optional.NonNullableType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_GenericTypeAlias_WithDefaultAndConstraint()
    {
        const string source = """
            type Container<T: number = 42> = T
            let x: Container = 42
            x
            """;

        var result = Utility.TypeCheck(source);
        Utility.AssertNoErrors(result.Diagnostics);

        var literal = Assert.IsType<LiteralType>(result.ReturnType);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_WithMultipleDefaults()
    {
        const string source = """
            fn pair<A = number, B = string>(a: A, b: B) -> a
            pair(42, "hello")
            """;

        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_WithConstraintSatisfied()
    {
        const string source = """
            fn identity<T: number>(value: T) -> value
            identity(42)
            """;

        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_WithDefaultTypeParameter()
    {
        const string source = """
            fn wrap<T = number>(value: T) -> value
            wrap(42)
            """;

        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_ExplicitTypeArgumentOverridesDefault()
    {
        const string source = """
            fn wrap<T = number>(value: T) -> value
            wrap::<string>("hello")
            """;

        var type = Utility.GetLastStatementType(source);
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.String, primitive.Kind);
    }

    [Fact]
    public void Checks_NonGenericFunctionCall()
    {
        const string source = """
            fn add(a: number, b: number): number -> a + b
            add(1, 2)
            """;

        var type = Utility.GetLastStatementType(source);
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_Function()
    {
        var type = Utility.GetLastStatementType("fn concat(a: string, b: string) -> a + b");
        var functionType = Assert.IsType<FunctionType>(type);
        Assert.True(functionType.ReturnType.Equals(PrimitiveType.String), $"Expected 'string', got '{functionType.ReturnType}'");
        Assert.Empty(functionType.TypeParameters);
        Assert.Equal(2, functionType.ParameterTypes.Count);
        Assert.All(functionType.ParameterTypes, t => Assert.True(t.Equals(PrimitiveType.String), $"Expected 'string', got '{t}'"));
    }

    [Fact]
    public void Checks_GenericFunctionCall_InferredLiteralType()
    {
        const string source = """
            fn id<T>(value: T) -> value
            id(69)
            """;

        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(69L, literal.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_InferredPrimitiveType()
    {
        const string source = """
            fn id<T>(value: T) -> value
            let x: number = 42
            id(x)
            """;

        var type = Utility.GetLastStatementType(source);
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_GenericFunctionCall_WithOptionalReturn()
    {
        const string source = """
            fn opt<T>(value: T): T? -> value
            opt(69)
            """;

        var type = Utility.GetLastStatementType(source);
        var optional = Assert.IsType<OptionalType>(type);
        var inner = Assert.IsType<LiteralType>(optional.NonNullableType);
        Assert.Equal(69L, inner.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_WithMultipleTypeParameters()
    {
        const string source = """
            fn pair<A, B>(a: A, b: B) -> a
            pair(42, "hello")
            """;

        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_InferenceAcrossMultipleParameters()
    {
        const string source = """
            fn first<T>(a: T, b: T) -> a
            first(42, 69)
            """;

        var type = Utility.GetLastStatementType(source);
        Assert.True(type is LiteralType, $"Expected '42', got '{type}'");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_GenericFunctionCall_ExplicitTypeArgument()
    {
        const string source = """
            fn id<T>(value: T) -> value
            id::<number>(69)
            """;

        var type = Utility.GetLastStatementType(source);
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_GenericFunctionCall_ExplicitTypeArgument_WithOptional()
    {
        const string source = """
            fn opt<T>(value: T): T? -> value
            opt::<number>(69)
            """;

        var type = Utility.GetLastStatementType(source);
        var optional = Assert.IsType<OptionalType>(type);
        var inner = Assert.IsType<PrimitiveType>(optional.NonNullableType);
        Assert.Equal(PrimitiveTypeKind.Number, inner.Kind);
    }

    [Fact]
    public void Checks_NonGenericFunctionCall_InferredReturnType()
    {
        const string source = """
            fn concat(a: string, b: string) -> a + b
            concat("hello", " world")
            """;

        var type = Utility.GetLastStatementType(source);
        Assert.True(type.Equals(PrimitiveType.String), $"Expected 'string', got '{type}'");
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
            "number" => PrimitiveType.Number,
            _ => PrimitiveType.Never
        };

        Assert.True(
            type.Equals(expectedType),
            $"Expected '{expectedTypeName}', got '{type}' for expression '{source}'"
        );
    }

    [Theory]
    [InlineData("1 + 1", "number")]
    [InlineData("'a' + 'b'", "string")]
    [InlineData("1 - 2", "number")]
    [InlineData("1 / 2", "number")]
    [InlineData("1 * 2", "number")]
    [InlineData("true && false", "bool")]
    [InlineData("1 < 2", "bool")]
    [InlineData("'a' > 'b'", "bool")]
    [InlineData("1 ?? 1", "1")]
    [InlineData("1 ?? 'a'", "1 | \"a\"")]
    [InlineData("mut x: string? = 'a'; 1 ?? x", "1 | string")]
    [InlineData("mut x: string? = 'a'; x ?? 'foo'", "string")]
    public void Checks_BinaryOperator_ReturnsExpectedType(string source, string expectedTypeName)
    {
        var type = Utility.GetLastStatementType(source);
        Assert.True(
            expectedTypeName == type.ToString(),
            $"Expected '{expectedTypeName}', got '{type}' for expression '{source}'"
        );
    }

    [Theory]
    [InlineData("!true", "bool")]
    [InlineData("~5", "number")]
    [InlineData("~0", "number")]
    [InlineData("-5", "number")]
    [InlineData("-0", "number")]
    [InlineData("-(5)", "number")]
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
    public void Checks_IntersectionTypes()
    {
        var type = Utility.GetLastStatementType("mut x: number & string;");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Never, primitive.Kind);
    }
    
    [Fact]
    public void Checks_UnionTypes()
    {
        var type = Utility.GetLastStatementType("mut x: number | string;");
        var union = Assert.IsType<UnionType>(type);
        Assert.Equal(2, union.Types.Count);
        
        var firstPrimitive = Assert.IsType<PrimitiveType>(union.Types.First());
        var lastPrimitive = Assert.IsType<PrimitiveType>(union.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, firstPrimitive.Kind);
        Assert.Equal(PrimitiveTypeKind.String, lastPrimitive.Kind);
    }
    
    [Fact]
    public void Checks_Mutable_ArrayTypes()
    {
        var type = Utility.GetLastStatementType("mut x: number[mut];");
        var array = Assert.IsType<ArrayType>(type);
        Assert.True(array.IsMutable);
        
        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }
    
    [Fact]
    public void Checks_ArrayTypes()
    {
        var type = Utility.GetLastStatementType("mut x: number[];");
        var array = Assert.IsType<ArrayType>(type);
        Assert.False(array.IsMutable);
        
        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }
    
    [Fact]
    public void Checks_OptionalTypes()
    {
        var type = Utility.GetLastStatementType("mut x: number?;");
        var optional = Assert.IsType<OptionalType>(type);
        var primitive = Assert.IsType<PrimitiveType>(optional.NonNullableType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }
    
    [Fact]
    public void Checks_PrimitiveTypes()
    {
        var type = Utility.GetLastStatementType("mut x: number;");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_LiteralTypes()
    {
        var type = Utility.GetLastStatementType("let x: 69 = 69; x;");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(69L, literal.Value);
    }
    
    [Fact]
    public void Checks_MixedOptional_ArrayLiterals()
    {
        var type = Utility.GetLastStatementType("[1, none]");
        var array = Assert.IsType<ArrayType>(type);
        Assert.False(array.IsMutable);
        
        var optional = Assert.IsType<OptionalType>(array.ElementType);
        var primitive = Assert.IsType<PrimitiveType>(optional.NonNullableType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }
    
    [Fact]
    public void Checks_Mixed_ArrayLiterals()
    {
        var type = Utility.GetLastStatementType("[1, true]");
        var array = Assert.IsType<ArrayType>(type);
        Assert.False(array.IsMutable);
        
        var union = Assert.IsType<UnionType>(array.ElementType);
        Assert.Equal(2, union.Types.Count);

        var number = Assert.IsType<PrimitiveType>(union.Types.First());
        var boolean = Assert.IsType<PrimitiveType>(union.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, number.Kind);
        Assert.Equal(PrimitiveTypeKind.Bool, boolean.Kind);
    }
    
    [Fact]
    public void Checks_ArrayLiterals()
    {
        var type = Utility.GetLastStatementType("[1, 2, 3]");
        var array = Assert.IsType<ArrayType>(type);
        Assert.False(array.IsMutable);
        
        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }
    
    [Fact]
    public void Checks_Mutable_ArrayLiterals()
    {
        var type = Utility.GetLastStatementType("let x = mut [1, 2, 3]; x");
        var array = Assert.IsType<ArrayType>(type);
        Assert.True(array.IsMutable);
        
        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
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