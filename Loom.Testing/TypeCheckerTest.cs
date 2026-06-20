using Loom.Diagnostics;
using Loom.TypeChecking;
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

    [Fact]
    public void ThrowsFor_QualifiedName_InvalidTarget()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("fn foo -> 42; foo.abc");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Cannot access property 'abc' on type 'fn(): 42'.");
    }

    [Fact]
    public void ThrowsFor_PropertyAccess_InvalidTarget()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("(69).abc");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Cannot access property 'abc' on type '69'.");
    }

    [Fact]
    public void ThrowsFor_ElementAccess_InvalidTarget()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("fn foo -> 42; foo[0]");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Cannot index value of type 'fn(): 42'.");
    }

    [Fact]
    public void ThrowsFor_ElementAccess_InvalidIndex()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let arr: number[] = [1, 2, 3]; arr[true]");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.InvalidAccess,
            "Expression of type 'true' cannot be used to index type 'number[]'. Index is not of type 'number'."
        );
    }

    [Fact]
    public void ThrowsFor_ElementAccess_ImmutableAssignment()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let arr: number[] = [1, 2, 3]; arr[0] = 69;");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.AssignToImmutable, "Cannot assign to immutable index 'number'.");
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
    public void ThrowsFor_NonNumeric_RangeLiteral()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("'a'..'b'");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"a\"' is not assignable to type 'number'.");
    }

    [Fact]
    public void ThrowsFor_QualifiedName_Chained_AfterNumber()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let r = (1..10); r.minimum.foo");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Cannot access property 'foo' on type 'number'.");
    }

    [Fact]
    public void ThrowsFor_QualifiedName_Chained_MissingIntermediate()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let r = (1..10); r.missing.next");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.InvalidAccess,
            "Expression of type '\"missing\"' cannot be used to index type 'Range'. Property 'missing' does not exist on type 'Range'."
        );
    }

    [Fact]
    public void ThrowsFor_PropertyAccess_Chained_AfterNumber()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("(1..10).minimum.foo");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Cannot access property 'foo' on type 'number'.");
    }

    [Fact]
    public void ThrowsFor_PropertyAccess_Chained_MissingIntermediate()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("(1..10).missing.next");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.InvalidAccess,
            "Expression of type '\"missing\"' cannot be used to index type 'Range'. Property 'missing' does not exist on type 'Range'."
        );
    }

    [Fact]
    public void ThrowsFor_StringEnumMemberWithoutInitializer()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("enum Colors : string { Red, Green = \"00FF00\" }");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.StringEnumMemberMustHaveInitializer,
            "Member 'Red' of string enum 'Colors' must have an initializer."
        );
    }

    [Fact]
    public void ThrowsFor_InvalidEnumBaseType()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("enum Flags : bool { Flag1 = true }");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.InvalidEnumBaseType,
            "Invalid enum base type.",
            "valid types are 'string' and 'number'"
        );
    }

    [Fact]
    public void ThrowsFor_EnumMemberNonConstantInitializer()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("enum Test { A = 1 + 2, B }");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.DynamicEnumMemberInitializer,
            "Enum member initializers must be constant values."
        );
    }

    [Fact]
    public void ThrowsFor_EnumTypeMismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("enum Status { Active, Inactive } let x: Status = 5");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '5' is not assignable to type '0 | 1'.");
    }

    [Fact]
    public void ThrowsFor_IfStatement_NonBooleanCondition()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("if 42 { 1 }");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.TypeMismatch,
            "Type '42' is not assignable to type 'bool'."
        );
    }

    [Fact]
    public void ThrowsFor_IfStatement_WithOptionalCondition()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: bool? = true; if x { 1 }");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.TypeMismatch,
            "Type 'bool?' is not assignable to type 'bool'."
        );
    }

    [Fact]
    public void ThrowsFor_Never_InBinaryOperation()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x = none; if x != none x + 1");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidBinaryOp, "No binary operation for 'never' + 'number'.");
    }

    [Fact]
    public void ThrowsFor_Never_InUnaryOperation()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x = none; if x != none { -x }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidUnaryOp, "No unary operation for -never.");
    }

    [Fact]
    public void ThrowsFor_InvalidCall()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x = 1; x()");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidInvocation, "Cannot call value of type '1'.");
    }

    [Fact]
    public void ThrowsFor_Cast_Mismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("69 as string");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '69' is not assignable to type 'string'.");
    }

    [Fact]
    public void ThrowsFor_Interface_ConstraintNotInterface()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I : number { }");
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.InvalidInterfaceConstraint,
            "Interfaces may only be constrained by other interfaces."
        );
    }

    [Fact]
    public void ThrowsFor_AssignToImmutable_ObjectIndexer()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics(
            "interface ImmutRecord<K, V> { [K]: V }; let x = none as never as ImmutRecord<string, bool>; x['abc'] = false"
        );

        Utility.AssertDiagnostic(diagnostics, InternalCodes.AssignToImmutable, "Cannot assign to immutable index 'string'.");
    }

    [Fact]
    public void ThrowsFor_AssignToImmutable_ObjectProperty()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface Obj { prop: number }; let x = none as never as Obj; x.prop = 69");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.AssignToImmutable, "Cannot assign to immutable property 'prop'.");
    }

    [Fact]
    public void ThrowsFor_AssignToImmutable_Nested_ObjectProperty()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics(
            "interface Inner { prop: number } interface Obj { inner: Inner }; let x = none as never as Obj; x.inner.prop = 69"
        );

        Utility.AssertDiagnostic(diagnostics, InternalCodes.AssignToImmutable, "Cannot assign to immutable property 'prop'.");
    }

    [Theory]
    [InlineData("let s = \"abcdef\"; s[1..3] = \"abc\"", "string[Range]")]
    [InlineData("let s = \"abcdef\"; s[1] = \"a\"", "string[number]")]
    [InlineData("let a = mut [1, 2, 3]; a[1..2] = [69, 420]", "number[mut][Range]")]
    public void ThrowsFor_AccessMacro_Assignment(string source, string assignType)
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics(source);
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, $"Cannot assign to '{assignType}' because the expression will be replaced by a macro.");
    }

    [Fact]
    public void ThrowsFor_IndexedType_InvalidTarget()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("type X = number['abc']");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Type '\"abc\"' cannot be used to index type 'number'.");
    }

    [Fact]
    public void ThrowsFor_ParameterDefaultMismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("fn foo(a: number = 'oops') {}");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"oops\"' is not assignable to type 'number'.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_NotAnInterface()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x = 1; new x {}");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidInvocation, "Type '1' is not an interface.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_PropertyNotFound()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I { x: number } new I { foo: 1 }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Property 'foo' does not exist on interface 'I'.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_PropertyTypeMismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I { x: number } new I { x: 'abc' }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"abc\"' is not assignable to type 'number'.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_IndexerRequired()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I { } new I { [0]: 1 }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidAccess, "Interface 'I' does not have an indexer.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_IndexerTypeMismatch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I { [number]: string } new I { ['text']: 1 }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"text\"' is not assignable to type 'number'.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_MissingPropertyInitializer()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I { x: number, y: number } new I { x: 1 }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.IncompleteInterfaceInvocation, "Missing property initializer for 'y' in interface 'I'.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_GenericWrongArity()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I<T> { value: T } new I::<number, string> { value: 1 }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.GenericArity, "Interface 'I<T>' expects 1 type argument, but 2 were provided.");
    }

    [Fact]
    public void ThrowsFor_InterfaceInvocation_GenericConstraintViolation()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface I<T: number> { value: T } new I::<string> { value: 'x' }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.ConstraintViolation, "Type 'string' does not satisfy constraint 'number' for type parameter 'T'.");
    }

    [Fact]
    public void WarnsFor_NullCoalescing_NonOptional()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("1 ?? 2");
        Assert.Contains(diagnostics.Set, d => d.Code == InternalCodes.RedundantCode);
    }
    
    [Fact]
    public void Checks_InterfaceInvocation_ChainedPropertyAccess()
    {
        var type = Utility.GetLastStatementType("interface I { x: number } new I { x: 1 }.x");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }
    
    [Fact]
    public void Checks_InterfaceInvocation_NonGeneric_PropertyInitializers()
    {
        var type = Utility.GetLastStatementType("interface I { x: number, y: string } new I { x: 42, y: 'hello' }");
        var iface = Assert.IsType<InterfaceType>(type);
        Assert.Equal("I", iface.Name);
    }

    [Fact]
    public void Checks_InterfaceInvocation_NonGeneric_IndexInitializer()
    {
        var type = Utility.GetLastStatementType("interface I { [string]: number } new I { ['key']: 42 }");
        var iface = Assert.IsType<InterfaceType>(type);
        Assert.Equal("I", iface.Name);
    }

    [Fact]
    public void Checks_InterfaceInvocation_Generic_ExplicitTypeArgs()
    {
        var type = Utility.GetLastStatementType("interface I<T> { value: T } new I::<number> { value: 42 }");
        var iface = Assert.IsType<InterfaceType>(type);
        Assert.Equal("I", iface.Name);
        var prop = iface.ObjectType.Properties.Single();
        Assert.True(prop.ValueType.Equals(PrimitiveType.Number));
    }

    [Fact]
    public void Checks_InterfaceInvocation_Generic_DefaultTypeArgs()
    {
        var result = Utility.TypeCheck("interface I<T = number> { value: T } new I { value: 42 }");
        Utility.AssertNoErrors(result);
        
        var iface = Assert.IsType<InterfaceType>(result.ReturnType);
        Assert.Equal("I", iface.Name);
    }

    [Fact]
    public void Checks_InterfaceInheritance_MemberAccess()
    {
        var type = Utility.GetLastStatementType("interface A { x: number } interface B : A { } let b = none as never as B; b.x");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_InterfaceInheritance_IndexerAccess()
    {
        var type = Utility.GetLastStatementType("interface A { [string]: number } interface B : A { } let b = none as never as B; b['hello']");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_CompoundAssignment_Add()
    {
        var type = Utility.GetLastStatementType("mut x = 1; x += 2");
        Assert.True(type.IsAssignableTo(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_CompoundAssignment_Concat()
    {
        var type = Utility.GetLastStatementType("mut s = 'a'; s += 'b'");
        Assert.True(type.IsAssignableTo(PrimitiveType.String), $"Expected 'string', got '{type}'");
    }

    [Fact]
    public void Checks_ReturnTypeInference_BlockMultipleReturns()
    {
        var type = Utility.GetLastStatementType("fn abs(x: number) { if x >= 0 { return x } else { return -x } }");
        var fnType = Assert.IsType<FunctionType>(type);
        Assert.True(fnType.ReturnType.Equals(PrimitiveType.Number), $"Expected 'number', got '{fnType.ReturnType}'");
    }

    [Theory]
    [InlineData("interface I { foo: number }; type Foo = I['foo'];")]
    [InlineData("type Foo = number[][number]")]
    public void Checks_IndexedType(string source)
    {
        var type = Utility.GetLastStatementType(source);
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_InterfaceDeclaration_Empty()
    {
        var type = Utility.GetLastStatementType("interface I { }");
        var iface = Assert.IsType<InterfaceType>(type);
        Assert.Equal("I", iface.Name);
        Assert.Empty(iface.Constraints);
        Assert.Null(iface.ObjectType.Indexer);
        Assert.Empty(iface.ObjectType.Properties);
    }

    [Fact]
    public void Checks_InterfaceDeclaration_WithProperties()
    {
        var type = Utility.GetLastStatementType("interface I { x: number, y: string }");
        var iface = Assert.IsType<InterfaceType>(type);
        Assert.Equal(2, iface.ObjectType.Properties.Count);

        var x = iface.ObjectType.Properties[0];
        Assert.Equal("x", x.Name);
        Assert.False(x.IsMutable);
        Assert.True(x.ValueType.Equals(PrimitiveType.Number));

        var y = iface.ObjectType.Properties[1];
        Assert.Equal("y", y.Name);
        Assert.False(y.IsMutable);
        Assert.True(y.ValueType.Equals(PrimitiveType.String));
    }

    [Fact]
    public void Checks_InterfaceDeclaration_WithMutableProperty()
    {
        var type = Utility.GetLastStatementType("interface I { mut count: number }");
        var iface = Assert.IsType<InterfaceType>(type);
        var prop = iface.ObjectType.Properties.Single();
        Assert.True(prop.IsMutable);
        Assert.True(prop.ValueType.Equals(PrimitiveType.Number));
    }

    [Fact]
    public void Checks_InterfaceDeclaration_WithIndexer()
    {
        var type = Utility.GetLastStatementType("interface I { [number]: string }");
        var iface = Assert.IsType<InterfaceType>(type);
        Assert.NotNull(iface.ObjectType.Indexer);
        Assert.True(iface.ObjectType.Indexer.KeyType.Equals(PrimitiveType.Number));
        Assert.True(iface.ObjectType.Indexer.ValueType.Equals(PrimitiveType.String));
        Assert.False(iface.ObjectType.Indexer.IsMutable);
    }

    [Fact]
    public void Checks_InterfaceDeclaration_WithMutableIndexer()
    {
        var type = Utility.GetLastStatementType("interface I { mut [string]: bool }");
        var iface = Assert.IsType<InterfaceType>(type);
        Assert.True(iface.ObjectType.Indexer!.IsMutable);
        Assert.True(iface.ObjectType.Indexer.KeyType.Equals(PrimitiveType.String));
        Assert.True(iface.ObjectType.Indexer.ValueType.Equals(PrimitiveType.Bool));
    }

    [Fact]
    public void Checks_InterfaceDeclaration_WithConstraint()
    {
        var type = Utility.GetLastStatementType("interface A { } interface B : A { }");
        var ifaceB = Assert.IsType<InterfaceType>(type);
        Assert.Single(ifaceB.Constraints);
        Assert.Equal("A", ifaceB.Constraints.First().Name);
    }

    [Fact]
    public void Checks_InterfaceDeclaration_WithMultipleConstraints()
    {
        var type = Utility.GetLastStatementType("interface A { } interface B { } interface C : A, B { }");
        var ifaceC = Assert.IsType<InterfaceType>(type);
        Assert.Equal(2, ifaceC.Constraints.Count);
        Assert.Equal("A", ifaceC.Constraints.First().Name);
        Assert.Equal("B", ifaceC.Constraints.Last().Name);
    }

    [Fact]
    public void Checks_InterfaceDeclaration_Generic()
    {
        var type = Utility.GetLastStatementType("interface I<T> { value: T }");
        var generic = Assert.IsType<GenericType>(type);
        Assert.Single(generic.Parameters);
        Assert.Equal("T", generic.Parameters.First().Name);

        var iface = Assert.IsType<InterfaceType>(generic.UnderlyingType);
        Assert.Equal("I", iface.Name);

        var prop = iface.ObjectType.Properties.Single();
        Assert.Equal("value", prop.Name);

        var typeParam = Assert.IsType<TypeParameter>(prop.ValueType);
        Assert.Equal("T", typeParam.Name);
    }

    [Fact]
    public void Checks_AsExpression_Chained_Unknown()
    {
        var type = Utility.GetLastStatementType("69 as unknown as number");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_AsExpression_Chained_Never()
    {
        var type = Utility.GetLastStatementType("69 as never as number");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_AsExpression_WithUnknown()
    {
        var type = Utility.GetLastStatementType("69 as unknown");
        Assert.True(type.Equals(PrimitiveType.Unknown), $"Expected 'unknown', got '{type}'");
    }

    [Fact]
    public void Checks_AsExpression_WithNever()
    {
        var type = Utility.GetLastStatementType("69 as never");
        Assert.True(type.Equals(PrimitiveType.Never), $"Expected 'never', got '{type}'");
    }

    [Fact]
    public void Checks_AsExpression()
    {
        var type = Utility.GetLastStatementType("69 as number");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_DeclareVariable_HasDeclaredType()
    {
        var type = Utility.GetLastStatementType("declare let x: number");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_DeclareMutableVariable_HasDeclaredType()
    {
        var type = Utility.GetLastStatementType("declare mut y: string");
        Assert.True(type.Equals(PrimitiveType.String), $"Expected 'string', got '{type}'");
    }

    [Fact]
    public void Checks_DeclareFunction_HasFunctionType()
    {
        var type = Utility.GetLastStatementType("declare fn add(a: number, b: number): bool");
        var fnType = Assert.IsType<FunctionType>(type);
        Assert.True(fnType.ReturnType.Equals(PrimitiveType.Bool), $"Expected 'bool', got '{fnType.ReturnType}'");
        Assert.Equal(2, fnType.ParameterTypes.Count);
        Assert.All(fnType.ParameterTypes, t => Assert.True(t.Equals(PrimitiveType.Number), $"Expected 'number', got '{t}'"));
    }

    [Fact]
    public void Checks_DeclareFunction_Generic_HasGenericFunctionType()
    {
        var type = Utility.GetLastStatementType("declare fn id<T>(value: T): T");
        var fnType = Assert.IsType<FunctionType>(type);
        Assert.Single(fnType.TypeParameters);
        Assert.Equal("T", fnType.TypeParameters[0].Name);

        var paramType = Assert.IsType<TypeParameter>(fnType.ParameterTypes[0]);
        Assert.Equal("T", paramType.Name);
        var returnType = Assert.IsType<TypeParameter>(fnType.ReturnType);
        Assert.Equal("T", returnType.Name);
    }

    [Fact]
    public void Checks_DeclareVariable_CanBeUsed()
    {
        var type = Utility.GetLastStatementType("declare let x: number; x");
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_DeclareFunction_CanBeInvoked()
    {
        const string source = """
            declare fn add(a: number, b: number): number;
            add(1, 2)
            """;

        var type = Utility.GetLastStatementType(source);
        Assert.True(type.Equals(PrimitiveType.Number), $"Expected 'number', got '{type}'");
    }

    [Fact]
    public void Checks_DeclareFunction_Generic_CanBeInvoked()
    {
        const string source = """
            declare fn id<T>(value: T): T;
            id(42)
            """;

        var type = Utility.GetLastStatementType(source);
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_DeclareFunction_VoidReturnType()
    {
        var type = Utility.GetLastStatementType("declare fn print(msg: string): void");
        var fnType = Assert.IsType<FunctionType>(type);
        Assert.True(fnType.ReturnType.Equals(PrimitiveType.Void), $"Expected 'void', got '{fnType.ReturnType}'");
    }

    [Fact]
    public void Narrowing_NonNullable()
    {
        Utility.AssertNoErrors(Utility.GetTypeCheckerDiagnostics("let x: number? = 69; if x != none x + 420"));
    }

    [Fact]
    public void Narrowing_EqualsLiteral_ThenBranch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: number | string = 42; if x == 42 { x + 1 }");
        Utility.AssertNoErrors(diagnostics);
    }

    [Fact]
    public void Narrowing_EqualsLiteral_ElseBranch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: number | string = 42; if x == 42 { } else { x + 1 }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvalidBinaryOp, "No binary operation for 'string' + 'number'.");
    }

    [Fact]
    public void Narrowing_NotEqualsLiteral_ThenBranch()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: number | string = 'hi'; if x != 'hi' { x + 1 }");
        Utility.AssertNoErrors(diagnostics);
    }

    [Fact]
    public void Narrowing_BooleanLiteral()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("let x: bool | number = true; if x == true { x && false }");
        Utility.AssertNoErrors(diagnostics);
    }

    [Fact]
    public void Narrowing_Chained()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics(
            "interface Inner { val: number? } interface Outer { inner: Inner } let obj = none as never as Outer; if obj.inner.val == 10 { obj.inner.val + 1 }"
        );

        Utility.AssertNoErrors(diagnostics);
    }

    [Fact]
    public void Checks_EnumTypeAnnotation()
    {
        var type = Utility.GetLastStatementType("enum Status { Active, Inactive } let x: Status = Status.Active");
        var union = Assert.IsType<UnionType>(type);
        Assert.Equal(2, union.Types.Count);
        Assert.Equal(0d, Assert.IsType<LiteralType>(union.Types.First()).Value);
        Assert.Equal(1d, Assert.IsType<LiteralType>(union.Types.Last()).Value);
    }

    [Fact]
    public void Checks_EnumWithNumberBaseTypeExplicit()
    {
        var type = Utility.GetLastStatementType("enum Values : number { One = 1, Two = 2, Three = 3 }");
        var objectType = Assert.IsType<ObjectType>(type);
        Assert.Equal(3, objectType.Properties.Count);

        var oneType = Assert.IsType<LiteralType>(objectType.Properties[0].ValueType);
        Assert.Equal(1d, oneType.Value);

        var twoType = Assert.IsType<LiteralType>(objectType.Properties[1].ValueType);
        Assert.Equal(2d, twoType.Value);

        var threeType = Assert.IsType<LiteralType>(objectType.Properties[2].ValueType);
        Assert.Equal(3d, threeType.Value);
    }

    [Fact]
    public void Checks_EnumWithNumberBaseTypeImplicit()
    {
        var type = Utility.GetLastStatementType("enum Values : number { A, B, C }");
        var objectType = Assert.IsType<ObjectType>(type);
        Assert.Equal(3, objectType.Properties.Count);

        var aType = Assert.IsType<LiteralType>(objectType.Properties[0].ValueType);
        Assert.Equal(0d, aType.Value);

        var bType = Assert.IsType<LiteralType>(objectType.Properties[1].ValueType);
        Assert.Equal(1d, bType.Value);

        var cType = Assert.IsType<LiteralType>(objectType.Properties[2].ValueType);
        Assert.Equal(2d, cType.Value);
    }

    [Fact]
    public void Checks_EmptyEnum()
    {
        var type = Utility.GetLastStatementType("enum Empty { }");
        var objectType = Assert.IsType<ObjectType>(type);
        Assert.Empty(objectType.Properties);
    }

    [Fact]
    public void Checks_EnumDeclaration_WithImplicitNumberValues()
    {
        var type = Utility.GetLastStatementType("enum Abc { A, B, C }");
        var objectType = Assert.IsType<ObjectType>(type);
        Assert.Equal(3, objectType.Properties.Count);

        Assert.Equal("A", objectType.Properties[0].Name);
        var aType = Assert.IsType<LiteralType>(objectType.Properties[0].ValueType);
        Assert.Equal(0d, aType.Value);

        Assert.Equal("B", objectType.Properties[1].Name);
        var bType = Assert.IsType<LiteralType>(objectType.Properties[1].ValueType);
        Assert.Equal(1d, bType.Value);

        Assert.Equal("C", objectType.Properties[2].Name);
        var cType = Assert.IsType<LiteralType>(objectType.Properties[2].ValueType);
        Assert.Equal(2d, cType.Value);
    }

    [Fact]
    public void Checks_EnumDeclaration_WithExplicitNumberValues()
    {
        var type = Utility.GetLastStatementType("enum Status { Active = 1, Inactive = 0, Pending = 2 }");
        var objectType = Assert.IsType<ObjectType>(type);
        Assert.Equal(3, objectType.Properties.Count);

        Assert.Equal("Active", objectType.Properties[0].Name);
        var activeType = Assert.IsType<LiteralType>(objectType.Properties[0].ValueType);
        Assert.Equal(1d, activeType.Value);

        Assert.Equal("Inactive", objectType.Properties[1].Name);
        var inactiveType = Assert.IsType<LiteralType>(objectType.Properties[1].ValueType);
        Assert.Equal(0d, inactiveType.Value);

        Assert.Equal("Pending", objectType.Properties[2].Name);
        var pendingType = Assert.IsType<LiteralType>(objectType.Properties[2].ValueType);
        Assert.Equal(2d, pendingType.Value);
    }

    [Fact]
    public void Checks_EnumDeclaration_WithMixedImplicitAndExplicitValues()
    {
        var type = Utility.GetLastStatementType("enum Mixed { A, B = 69, C }");
        var objectType = Assert.IsType<ObjectType>(type);
        Assert.Equal(3, objectType.Properties.Count);

        Assert.Equal("A", objectType.Properties[0].Name);
        var aType = Assert.IsType<LiteralType>(objectType.Properties[0].ValueType);
        Assert.Equal(0d, aType.Value);

        Assert.Equal("B", objectType.Properties[1].Name);
        var bType = Assert.IsType<LiteralType>(objectType.Properties[1].ValueType);
        Assert.Equal(69d, bType.Value);

        Assert.Equal("C", objectType.Properties[2].Name);
        var cType = Assert.IsType<LiteralType>(objectType.Properties[2].ValueType);
        Assert.Equal(70d, cType.Value);
    }

    [Fact]
    public void Checks_EnumDeclaration_WithStringBackedValues()
    {
        var type = Utility.GetLastStatementType("enum Colors : string { Red = \"FF0000\", Green = \"00FF00\", Blue = \"0000FF\" }");
        var objectType = Assert.IsType<ObjectType>(type);
        Assert.Equal(3, objectType.Properties.Count);

        Assert.Equal("Red", objectType.Properties[0].Name);
        var redType = Assert.IsType<LiteralType>(objectType.Properties[0].ValueType);
        Assert.Equal("FF0000", redType.Value);

        Assert.Equal("Green", objectType.Properties[1].Name);
        var greenType = Assert.IsType<LiteralType>(objectType.Properties[1].ValueType);
        Assert.Equal("00FF00", greenType.Value);

        Assert.Equal("Blue", objectType.Properties[2].Name);
        var blueType = Assert.IsType<LiteralType>(objectType.Properties[2].ValueType);
        Assert.Equal("0000FF", blueType.Value);
    }

    [Fact]
    public void Checks_EnumMemberAccess()
    {
        var type = Utility.GetLastStatementType("enum Status { Active, Inactive }; Status.Active");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(0d, literal.Value);
    }

    [Fact]
    public void Checks_EnumMemberAccess_WithExplicitValue()
    {
        var type = Utility.GetLastStatementType("enum Priority { Low = 10, High = 20 } Priority.High");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(20d, literal.Value);
    }

    [Fact]
    public void Checks_QualifiedName_SingleDot_OnRange()
    {
        var type = Utility.GetLastStatementType("let r = (1..10); r.minimum");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);

        type = Utility.GetLastStatementType("let r = (1..10); r.maximum");
        primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_PropertyAccess_SingleDot_OnRange()
    {
        var type = Utility.GetLastStatementType("(1..10).minimum");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);

        type = Utility.GetLastStatementType("(1..10).maximum");
        primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_RangeLiteral_ElementAccess()
    {
        var type = Utility.GetLastStatementType("let x = [1, 2, 3]; x[1..10]");
        var array = Assert.IsType<ArrayType>(type);
        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_RangeLiteral_StringElementAccess()
    {
        var type = Utility.GetLastStatementType("let x = 'abcdef'; x[1..3]");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.String, primitive.Kind);
    }

    [Fact]
    public void Checks_StringElementAccess()
    {
        var type = Utility.GetLastStatementType("let x = 'abcdef'; x[1]");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.String, primitive.Kind);
    }

    [Fact]
    public void Checks_RangeLiteral()
    {
        var type = Utility.GetLastStatementType("1..10");
        Assert.True(type.Equals(IntrinsicTypes.Range), $"Expected 'Range', got '{type}'");
    }

    [Fact]
    public void Checks_NameOf()
    {
        var type = Utility.GetLastStatementType("let x = 1; nameof(x)");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal("x", literal.Value);
    }

    [Fact]
    public void Checks_NameOf_QualifiedName()
    {
        var type = Utility.GetLastStatementType("let r = 1..10; nameof(r.minimum)");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal("r.minimum", literal.Value);
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
    public void Checks_ElementAccess_NestedArray()
    {
        var type = Utility.GetLastStatementType("let matrix = [[1, 2], [3, 4]]; matrix[0][1]");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_ElementAccess_AsAssignmentTarget()
    {
        var type = Utility.GetLastStatementType("let arr = mut [1, 2, 3]; arr[0] = 42;");
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(42L, literal.Value);
    }

    [Fact]
    public void Checks_ElementAccess_ArrayIndexWithExpression()
    {
        var type = Utility.GetLastStatementType("let arr = [10, 20, 30]; let i = 1; arr[i + 1]");
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Theory]
    [InlineData("let arr = [1, 2, 3]; arr[0]")]
    [InlineData("let arr = mut [1, 2, 3]; arr[0]")]
    [InlineData("mut arr = mut [1, 2, 3]; arr[0]")]
    [InlineData("mut arr = [1, 2, 3]; arr[0]")]
    public void Checks_ElementAccess_ArrayIndex(string source)
    {
        var type = Utility.GetLastStatementType(source);
        var primitive = Assert.IsType<PrimitiveType>(type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
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

        var result = Utility.AssertNoErrors(Utility.TypeCheck(source));
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
        var literal = Assert.IsType<LiteralType>(type);
        Assert.Equal(69L, literal.Value);
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
    public void Checks_FunctionTypes()
    {
        var type = Utility.GetLastStatementType("mut x: fn<T>(x: T): T?;");
        var function = Assert.IsType<FunctionType>(type);
        Assert.Single(function.TypeParameters);
        Assert.Single(function.ParameterTypes);

        var typeParameter = function.TypeParameters.First();
        Assert.Equal("T", typeParameter.Name);
        Assert.Null(typeParameter.Constraint);
        Assert.Null(typeParameter.DefaultType);

        Assert.IsType<OptionalType>(function.ReturnType);
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
        var type = Utility.GetLastStatementType("let x: number[mut] = [];");
        var array = Assert.IsType<ArrayType>(type);
        Assert.True(array.IsMutable);

        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Checks_ArrayTypes()
    {
        var type = Utility.GetLastStatementType("let x: number[] = [];");
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
    public void Checks_Widened_Empty_ArrayLiterals()
    {
        var type = Utility.GetLastStatementType("mut x = []");
        var array = Assert.IsType<ArrayType>(type);
        Assert.False(array.IsMutable);

        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Unknown, primitive.Kind);
    }

    [Fact]
    public void Checks_Empty_ArrayLiterals()
    {
        var type = Utility.GetLastStatementType("[]");
        var array = Assert.IsType<ArrayType>(type);
        Assert.False(array.IsMutable);

        var primitive = Assert.IsType<PrimitiveType>(array.ElementType);
        Assert.Equal(PrimitiveTypeKind.Never, primitive.Kind);
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