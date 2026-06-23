using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;

namespace Loom.Testing;

[Collection("Assembly")]
public class ResolverTest
{
    #region ThrowsFor
    [Theory]
    [InlineData("mut x; x;")]
    [InlineData("mut x: number; { let x = 42; }; x;")]
    [InlineData("mut x: number; { x; }")]
    public void ThrowsFor_UseOfUninitialized(string source)
    {
        var diagnostics = Utility.GetSemanticModel(source).Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UseOfUninitialized, "Use of uninitialized variable 'x'.");
    }

    [Theory]
    [InlineData("mut x: number; if true x = 69; x;")]
    [InlineData("mut x: number; if true x = 69 else if true x = 420; x;")]
    [InlineData(
        """
                    mut x: number;
                    if outer {
                        if inner {
                            x = 42;
                        }
                    } else {
                        x = 0;
                    }
                    x;
        """
    )]
    public void ThrowsFor_UseOfMaybeUninitialized(string source)
    {
        var diagnostics = Utility.GetSemanticModel(source).Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UseOfMaybeUninitialized, "Variable 'x' might not be initialized on this path.");
    }

    [Fact]
    public void ThrowsFor_UninitializedConst()
    {
        var diagnostics = Utility.GetSemanticModel("let x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.MustHaveInitializer, "Immutable declarations must be initialized.");
    }

    [Fact]
    public void ThrowsFor_DuplicateVariable()
    {
        var diagnostics = Utility.GetSemanticModel("let x = 1; let x = 2;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Variable 'x' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_DuplicateFunction()
    {
        var diagnostics = Utility.GetSemanticModel("fn foo() {} fn foo() {}").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Variable 'foo' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_DuplicateEnum()
    {
        var diagnostics = Utility.GetSemanticModel("enum Abc {} enum Abc{}").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Variable 'Abc' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_DuplicateEnumTypeName()
    {
        var diagnostics = Utility.GetSemanticModel("type Abc = number[] enum Abc{}").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Type 'Abc' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_DuplicateParameter()
    {
        var diagnostics = Utility.GetSemanticModel("fn foo(x: number, x: string) {}").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Parameter 'x' is already declared for this function.");
    }

    [Fact]
    public void ThrowsFor_UndefinedVariable()
    {
        var diagnostics = Utility.GetSemanticModel("x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindName, "Cannot find name 'x'.");
    }

    [Fact]
    public void ThrowsFor_UndefinedType()
    {
        var diagnostics = Utility.GetSemanticModel("let x: Abc = 1").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindName, "Cannot find type 'Abc'.");
    }

    [Fact]
    public void ThrowsFor_AssignToImmutable()
    {
        var diagnostics = Utility.GetSemanticModel("let x = 1; x = 69").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.AssignToImmutable, "Cannot assign to immutable variable 'x'.");
    }

    [Fact]
    public void ThrowsFor_DynamicEnumAccess()
    {
        var diagnostics = Utility.GetSemanticModel("enum Abc { A, B, C }; Abc").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DynamicEnumAccess, "Cannot use enums dynamically because they are compile-time constants.");
    }

    [Fact]
    public void ThrowsFor_VariableInitializedInNestedBlock_UsedOutside()
    {
        var diagnostics = Utility.GetSemanticModel("{ let x = 42; } x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindName, "Cannot find name 'x'.");
    }

    [Fact]
    public void ThrowsFor_ReturnStatementOutsideFunction()
    {
        var diagnostics = Utility.GetSemanticModel("return 42;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.ReturnOutsideFunction, "Return statements can only be used inside of functions.");
    }

    [Fact]
    public void ThrowsFor_DuplicateDeclareVariable()
    {
        var diagnostics = Utility.GetSemanticModel("declare let x: number; declare let x: number;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Variable 'x' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_DuplicateDeclareFunction()
    {
        var diagnostics = Utility.GetSemanticModel("declare fn f(): void; declare fn f(): void;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Variable 'f' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_DuplicateDeclareFunctionParameter()
    {
        var diagnostics = Utility.GetSemanticModel("declare fn f(x: number, x: string): void;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Parameter 'x' is already declared for this function.");
    }

    [Fact]
    public void ThrowsFor_DeclareVariableConflictsWithFunction()
    {
        var diagnostics = Utility.GetSemanticModel("fn foo() {} declare let foo: number;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Variable 'foo' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_DeclareFunctionConflictsWithVariable()
    {
        var diagnostics = Utility.GetSemanticModel("let x = 1; declare fn x(): void;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.DuplicateName, "Variable 'x' is already declared in this scope.");
    }

    [Fact]
    public void ThrowsFor_AssignToDeclaredImmutableVariable()
    {
        var diagnostics = Utility.GetSemanticModel("declare let x: number; x = 42;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.AssignToImmutable, "Cannot assign to immutable variable 'x'.");
    }

    [Fact]
    public void ThrowsFor_DeclaredVariableNotVisibleOutsideBlock()
    {
        var diagnostics = Utility.GetSemanticModel("{ declare let x: number; } x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindName, "Cannot find name 'x'.");
    }

    [Fact]
    public void ThrowsFor_Interface_DuplicateIndexer()
    {
        var diagnostics = Utility.GetSemanticModel("interface I { [number]: string, [string]: bool }").Diagnostics;
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.DuplicateIndexer,
            "Type 'I' may only have one indexer."
        );
    }

    [Fact]
    public void ThrowsFor_Interface_DuplicateProperty()
    {
        var diagnostics = Utility.GetSemanticModel("interface I { x: number, x: string }").Diagnostics;
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.DuplicateName,
            "Property 'x' already exists on type 'I'"
        );
    }

    [Fact]
    public void ThrowsFor_Parameter_MissingTypeAndDefault()
    {
        var diagnostics = Utility.GetSemanticModel("fn foo(x) {}").Diagnostics;
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.MustHaveDefaultOrType,
            "Parameter must have a declared type or default value to infer from."
        );
    }

    [Fact]
    public void ThrowsFor_VariableInitializedInWhile_IsNotDefinitelyInitializedAfterLoop()
    {
        var diagnostics = Utility.GetSemanticModel("mut x: number; while true { x = 1; break; } x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UseOfMaybeUninitialized, "Variable 'x' might not be initialized on this path.");
    }

    [Fact]
    public void ThrowsFor_BreakOutsideLoop()
    {
        var diagnostics = Utility.GetSemanticModel("break").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.BreakOutsideLoop, "Break statements can only be used inside of loops.");
    }

    [Fact]
    public void ThrowsFor_ContinueOutsideLoop()
    {
        var diagnostics = Utility.GetSemanticModel("continue").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.ContinueOutsideLoop, "Continue statements can only be used inside of loops.");
    }

    [Fact]
    public void ThrowsFor_BreakInsideFunctionInsideLoop()
    {
        var diagnostics = Utility.GetSemanticModel("while true { fn inner() { break } }").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.BreakOutsideLoop, "Break statements can only be used inside of loops.");
    }

    [Fact]
    public void ThrowsFor_ContinueInsideFunctionInsideLoop()
    {
        var diagnostics = Utility.GetSemanticModel("while true { fn inner() { continue } }").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.ContinueOutsideLoop, "Continue statements can only be used inside of loops.");
    }

    [Fact]
    public void ThrowsFor_Sealed_Inheritance()
    {
        var diagnostics = Utility.GetSemanticModel("sealed interface A; interface B: A;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InheritFromSealed, "Cannot constrain interface 'B' with sealed interface 'A'.");
    }

    [Theory]
    [InlineData("interface I : number;")]
    [InlineData("interface I : 69;")]
    [InlineData("type A = number; interface I : A;")]
    public void ThrowsFor_Interface_ConstraintNotInterface(string source)
    {
        var diagnostics = Utility.GetSemanticModel(source).Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.NonInterfaceConstraint, "Interfaces may only be constrained by other interfaces.");
    }

    [Fact]
    public void ThrowsFor_DeclaredInterface_Invocation()
    {
        var diagnostics = Utility.GetSemanticModel("declare interface A; let a = new A {}").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.InvokeDeclaredInterface, "Cannot invoke interface 'A' because it was declared as type.");
    }

    [Fact]
    public void ThrowsFor_ReturnInsideAfterBody_OutsideFunction()
    {
        var diagnostics = Utility.GetSemanticModel("after 1s { return 42; }").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.ReturnOutsideFunction, "Return statements can only be used inside of functions.");
    }

    [Fact]
    public void ThrowsFor_AfterCondition_UsesUndefinedVariable()
    {
        var diagnostics = Utility.GetSemanticModel("after unknown { }").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindName, "Cannot find name 'unknown'.");
    }

    [Fact]
    public void ThrowsFor_VariableInitializedInAfterBody_UsedAfter()
    {
        var diagnostics = Utility.GetSemanticModel("mut x: number; after 1s { x = 42; } x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UseOfMaybeUninitialized, "Variable 'x' might not be initialized on this path.");
    }

    [Fact]
    public void ThrowsFor_VariableDeclaredInAfterBody_UsedOutside()
    {
        var diagnostics = Utility.GetSemanticModel("after 1s { let x = 42; } x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.CannotFindName, "Cannot find name 'x'.");
    }

    [Fact]
    public void ThrowsFor_ContinueInsideAfter()
    {
        var diagnostics = Utility.GetSemanticModel("after 1s { continue }").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.ContinueOutsideLoop, "Continue statements can only be used inside of loops.");
    }
    
    [Fact]
    public void ThrowsFor_BreakInsideAfter()
    {
        var diagnostics = Utility.GetSemanticModel("after 1s { break }").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.BreakOutsideLoop, "Break statements can only be used inside of loops.");
    }
    
    [Fact]
    public void ThrowsFor_ReturnInsideAfter()
    {
        var diagnostics = Utility.GetSemanticModel("fn abc { after 1s { return 69 } }").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.ReturnInAfter, "Cannot return a value from an 'after' statement body.");
    }
    
    [Fact]
    public void ThrowsFor_VariableInitializedInForBody_UsedAfter()
    {
        var diagnostics = Utility.GetSemanticModel("mut x: number; for let _ in 1..10 { x = 42; } x;").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UseOfMaybeUninitialized, "Variable 'x' might not be initialized on this path.");
    }
    
    [Fact]
    public void ThrowsFor_DeclareVariable_MissingType()
    {
        var diagnostics = Utility.GetSemanticModel("declare let x").Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.MissingDeclareVariableType, "Declared variable signatures must have a type.");
    }
    #endregion ThrowsFor
    
    [Fact]
    public void ThrowsFor_ForLoop_NonObjectCollection_Number()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("for let x in 42 { }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '42' is not assignable to type 'object'.");
    }

    [Fact]
    public void ThrowsFor_ForLoop_NonObjectCollection_Bool()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("for let x in true { }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type 'true' is not assignable to type 'object'.");
    }

    [Fact]
    public void ThrowsFor_ForLoop_NonObjectCollection_String()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("for let x in \"abc\" { }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type '\"abc\"' is not assignable to type 'object'.");
    }

    [Fact]
    public void ThrowsFor_ForLoop_NonObjectCollection_Optional()
    {
        var diagnostics = Utility.GetTypeCheckerDiagnostics("interface Foo {}; let a: Foo? = new Foo {}; for let x in a { }");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.TypeMismatch, "Type 'Foo?' is not assignable to type 'object'.");
    }

    [Theory]
    [InlineData("fn foo { return 42; let x = 1 }")]
    [InlineData("while true { break; let unreachable = 1; }")]
    [InlineData("while true { continue; let unreachable = 1; }")]
    public void WarnsFor_UnreachableCode(string source)
    {
        var diagnostics = Utility.GetSemanticModel(source).Diagnostics;
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnreachableCode, "Unreachable code detected.");
    }

    #region Allows
    [Theory]
    [InlineData("fn abc { return 69 }")]
    [InlineData("fn abc { if true { return 69 } }")]
    public void Allows_Fn_Return(string source) => Utility.AssertNoErrors(Utility.GetSemanticModel(source));
    
    [Fact]
    public void Allows_VariableInitializedBeforeAfter_UsedAfter() => Utility.AssertNoErrors(Utility.GetSemanticModel("let x = 1; after 1s { } x;"));

    [Fact]
    public void Allows_AfterInsideIf() => Utility.AssertNoErrors(Utility.GetSemanticModel("if true { after 1s { } }"));

    [Fact]
    public void Allows_AfterInsideWhile() => Utility.AssertNoErrors(Utility.GetSemanticModel("while true { after 1s { } }"));

    [Fact]
    public void Allows_AfterBody_WithShadowedVariable() => Utility.AssertNoErrors(Utility.GetSemanticModel("let x = 1; after 1s { let x = 2; x; } x;"));

    [Fact]
    public void Allows_After_WithEmptyBlock() => Utility.AssertNoErrors(Utility.GetSemanticModel("after 1s { }"));

    [Fact]
    public void Allows_NonSealedInterfaceConstraints() => Utility.AssertNoErrors(Utility.GetSemanticModel("interface A; interface B: A;"));
    
    [Fact]
    public void Allows_BreakInsideFor() => Utility.AssertNoErrors(Utility.GetSemanticModel("for let x in 1..10 { break }"));
    
    [Fact]
    public void Allows_ContinueInsideFor() => Utility.AssertNoErrors(Utility.GetSemanticModel("for let x in 1..10 { continue }"));

    [Fact]
    public void Allows_BreakInsideWhile() => Utility.AssertNoErrors(Utility.GetSemanticModel("while true { break }"));

    [Fact]
    public void Allows_ContinueInsideWhile() => Utility.AssertNoErrors(Utility.GetSemanticModel("while true { continue }"));

    [Fact]
    public void Allows_BreakInsideIfInsideWhile() => Utility.AssertNoErrors(Utility.GetSemanticModel("while true { if true { break } }"));

    [Fact]
    public void Allows_ContinueInsideIfInsideWhile() => Utility.AssertNoErrors(Utility.GetSemanticModel("while true { if true { continue } }"));

    [Fact]
    public void VariableInitializedBeforeWhile_IsDefinitelyInitializedAfter()
    {
        var model = Utility.GetSemanticModel("let x = 1; while true { break } x;");
        Utility.AssertNoErrors(model);
    }

    [Fact]
    public void Allows_Interface_WithSingleIndexerAndUniqueProperties() =>
        Utility.AssertNoErrors(Utility.GetSemanticModel("interface I { [number]: string, count: number, name: string }"));

    [Fact]
    public void Allows_SameParameterName_ChainedFnTypes()
    {
        var model = Utility.GetSemanticModel("type X = fn(x: number): void & fn(x: string): bool");
        Utility.AssertNoErrors(model);
    }

    [Fact]
    public void Allows_UsageOfDeclaredVariable()
    {
        var model = Utility.GetSemanticModel("declare let x: number; x;");
        Utility.AssertNoErrors(model);
    }

    [Fact]
    public void Allows_UsageOfDeclaredFunction()
    {
        var model = Utility.GetSemanticModel("declare fn foo(): void; foo;");
        Utility.AssertNoErrors(model);
    }

    [Fact]
    public void Allows_AssignToDeclaredMutableVariable()
    {
        var model = Utility.GetSemanticModel("declare mut counter: number; counter = 1;");
        Utility.AssertNoErrors(model);
    }

    [Fact]
    public void Allows_NestedScopes_WithSameVariableNames() => Utility.AssertNoErrors(Utility.GetSemanticModel("let x = 42; { let x = 69; x; } x;"));

    [Fact]
    public void Allows_ReturnStatementInNestedFunction() => Utility.AssertNoErrors(Utility.GetSemanticModel("fn outer() { fn inner() { return 42; } return 0; }"));

    [Fact]
    public void Allows_ReturnStatementInsideFunction() => Utility.AssertNoErrors(Utility.GetSemanticModel("fn test() { return 42; }"));

    [Fact]
    public void Allows_MultipleInitializations_AcrossBranches() =>
        Utility.AssertNoErrors(
            Utility.GetSemanticModel(
                """
                            let cond1 = true; 
                            let cond2 = true; 
                            mut x: number;
                            if cond1 {
                                x = 1;
                            } else if cond2 {
                                x = 2;
                            } else {
                                x = 3;
                            }
                            x;
                """
            )
        );

    [Fact]
    public void Allows_VariableInitializedInOuterScope_ToBeUsedInInnerScope() => Utility.AssertNoErrors(Utility.GetSemanticModel("let x = true; if x { x }"));

    [Fact]
    public void Allows_VariableInitializedBeforeIf_ToBeUsedAfterIf() =>
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; let x = 42; if condition { } x;"));

    [Fact]
    public void Allows_VariableInitializedInBothIfBranches_ToBeUsedAfterIf() =>
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; mut x: number; if condition { x = 42 } else { x = 0 } x;"));

    [Fact]
    public void Allows_VariableInitializedInThenBranch_UsedInsideThenBranch() =>
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; mut x: number; if condition { x = 42; x; }"));

    [Fact]
    public void Allows_VariableFromOuterScope_ToBeReassignedInInnerScope() =>
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; mut x = 1; if condition { x = 2; } x;"));
    #endregion Allows

    [Fact]
    public void TracksInitialization_ThroughNestedBlocks() => Utility.AssertNoErrors(Utility.GetSemanticModel("mut x: number; { x = 42; } x;"));

    #region Declares
    [Theory]
    [InlineData("Range")]
    [InlineData("Record<string, bool>")]
    public void Declares_IntrinsicType_Symbols(string name) => Utility.AssertNoErrors(Utility.GetSemanticModel($"mut x: {name}"));

    [Fact]
    public void Declares_VariableSymbol()
    {
        var model = Utility.GetSemanticModel("let x = 1; x;");
        Assert.Equal(2, model.Tree.Statements.Count);

        var firstStatement = model.Tree.Statements.First();
        var secondStatement = model.Tree.Statements.Last();
        var variableDeclaration = Assert.IsType<VariableDeclaration>(firstStatement);
        var expressionStatement = Assert.IsType<ExpressionStatement>(secondStatement);
        var identifier = Assert.IsType<Identifier>(expressionStatement.Expression);
        var symbol = model.GetSymbol(identifier);
        Assert.NotNull(symbol);
        Assert.Equal("x", symbol.Name);
        Assert.Equal(SymbolKind.Variable, symbol.Kind);
        Assert.Equal(variableDeclaration, symbol.Declaration);

        var declaringSymbol = model.GetDeclaringSymbol(identifier);
        var declarationSymbol = model.GetDeclarationSymbol(variableDeclaration);
        Assert.NotNull(declaringSymbol);
        Assert.NotNull(declarationSymbol);
        Assert.Equal(declaringSymbol, declarationSymbol);
        Assert.Equal("x", declarationSymbol.Name);
        Assert.Equal(SymbolKind.Variable, declarationSymbol.Kind);
        Assert.Equal(variableDeclaration, declarationSymbol.Declaration);
    }

    [Fact]
    public void Declares_Enum_VariableSymbol()
    {
        var model = Utility.GetSemanticModel("enum Colors { Red, Green, Blue }; Colors.Red");
        Utility.AssertNoErrors(model);

        var declaration = model.Tree.Statements.First();
        var expressionStatement = Assert.IsType<ExpressionStatement>(model.Tree.Statements.Last());
        var qualifiedName = Assert.IsType<QualifiedName>(expressionStatement.Expression);
        var symbol = model.GetSymbol(qualifiedName.Identifier);
        Assert.NotNull(symbol);
        Assert.Equal("Colors", symbol.Name);
        Assert.Equal(SymbolKind.Variable, symbol.Kind);
        Assert.Equal(declaration, symbol.Declaration);
    }

    [Fact]
    public void Declares_EnumTypeSymbol()
    {
        var model = Utility.GetSemanticModel("enum Colors { Red, Green, Blue };");
        var enumDeclaration = Assert.IsType<EnumDeclaration>(model.Tree.Statements.First());
        var symbol = model.GetDeclarationSymbol(enumDeclaration);
        Assert.NotNull(symbol);
        Assert.Equal("Colors", symbol.Name);
        Assert.Equal(SymbolKind.EnumType, symbol.Kind);
        Assert.Equal(enumDeclaration, symbol.Declaration);
    }

    [Fact]
    public void Declares_ParameterSymbol()
    {
        var model = Utility.GetSemanticModel("fn test(x: number) { }");
        var functionDeclaration = Assert.IsType<FunctionDeclaration>(model.Tree.Statements.First());
        Assert.NotNull(functionDeclaration.Parameters);

        var parameter = functionDeclaration.Parameters!.ParameterList.First();
        var symbol = model.GetDeclarationSymbol(parameter);
        Assert.NotNull(symbol);
        Assert.Equal("x", symbol.Name);
        Assert.Equal(SymbolKind.Parameter, symbol.Kind);
        Assert.Equal(parameter, symbol.Declaration);
    }

    [Fact]
    public void Declares_FunctionSymbol()
    {
        var model = Utility.GetSemanticModel("fn test() { }");
        var functionDeclaration = Assert.IsType<FunctionDeclaration>(model.Tree.Statements.First());
        var symbol = model.GetDeclarationSymbol(functionDeclaration);
        Assert.NotNull(symbol);
        Assert.Equal("test", symbol.Name);
        Assert.Equal(SymbolKind.Function, symbol.Kind);
        Assert.Equal(functionDeclaration, symbol.Declaration);
    }

    [Theory]
    [InlineData("sealed interface Foo { foo: number }", true)]
    [InlineData("interface Foo { foo: number }")]
    [InlineData("declare sealed interface Foo { foo: number }", true, typeof(Declare))]
    [InlineData("declare interface Foo { foo: number }", false, typeof(Declare))]
    public void Declares_InterfaceSymbol(string source, bool isSealed = false, Type? declarationType = null)
    {
        var model = Utility.AssertNoErrors(Utility.GetSemanticModel(source));
        var statement = model.Tree.Statements.First();
        Assert.IsType(declarationType ?? typeof(InterfaceDeclaration), statement);

        if (declarationType == typeof(Declare))
            statement = ((Declare)statement).Signature;

        var symbol = model.GetDeclarationSymbol(statement);
        Assert.NotNull(symbol);

        var interfaceSymbol = Assert.IsType<InterfaceSymbol>(symbol);
        Assert.Equal("Foo", interfaceSymbol.Name);
        Assert.Equal(SymbolKind.Interface, interfaceSymbol.Kind);
        Assert.Equal(statement, interfaceSymbol.Declaration);
        Assert.Equal(isSealed, interfaceSymbol.IsSealed);
        Assert.False(interfaceSymbol.IsIntrinsic);
        Assert.False(interfaceSymbol.IsMutable);
    }

    [Fact]
    public void Declares_TypeAliasSymbol()
    {
        var model = Utility.GetSemanticModel("type MyNumber = number");
        var typeAlias = Assert.IsType<TypeAlias>(model.Tree.Statements.First());
        var symbol = model.GetDeclarationSymbol(typeAlias);
        Assert.NotNull(symbol);
        Assert.Equal("MyNumber", symbol.Name);
        Assert.Equal(SymbolKind.Type, symbol.Kind);
        Assert.Equal(typeAlias, symbol.Declaration);
    }

    [Fact]
    public void Declares_TypeParameterSymbol()
    {
        var model = Utility.GetSemanticModel("type Container<T> = T");
        var typeAlias = Assert.IsType<TypeAlias>(model.Tree.Statements.First());
        Assert.NotNull(typeAlias.TypeParameters);

        var typeParameter = typeAlias.TypeParameters.ParameterList.First();
        var symbol = model.GetDeclarationSymbol(typeParameter);
        Assert.NotNull(symbol);
        Assert.Equal("T", symbol.Name);
        Assert.Equal(SymbolKind.Type, symbol.Kind);
        Assert.Equal(typeParameter, symbol.Declaration);
    }

    [Fact]
    public void Declares_DeclareVariableSymbol()
    {
        var model = Utility.GetSemanticModel("declare let x: number");
        Utility.AssertNoErrors(model);

        var declare = Assert.IsType<Declare>(model.Tree.Statements.Single());
        var sig = Assert.IsType<DeclareVariableSignature>(declare.Signature);
        var symbol = model.GetDeclarationSymbol(sig);
        Assert.NotNull(symbol);
        Assert.Equal("x", symbol.Name);
        Assert.Equal(SymbolKind.Variable, symbol.Kind);
        Assert.False(symbol.IsMutable);
        Assert.Equal(sig, symbol.Declaration);
    }

    [Fact]
    public void Declares_DeclareVariableSymbol_Mutable()
    {
        var model = Utility.GetSemanticModel("declare mut y: string");
        var declare = Assert.IsType<Declare>(model.Tree.Statements.Single());
        var sig = Assert.IsType<DeclareVariableSignature>(declare.Signature);
        var symbol = model.GetDeclarationSymbol(sig);
        Assert.NotNull(symbol);
        Assert.Equal("y", symbol.Name);
        Assert.True(symbol.IsMutable);
    }

    [Fact]
    public void Declares_DeclareFunctionSymbol()
    {
        var model = Utility.GetSemanticModel("declare fn add(a: number, b: number): number");
        Utility.AssertNoErrors(model);

        var declare = Assert.IsType<Declare>(model.Tree.Statements.Single());
        var sig = Assert.IsType<DeclareFunctionSignature>(declare.Signature);
        var symbol = model.GetDeclarationSymbol(sig);
        Assert.NotNull(symbol);
        Assert.Equal("add", symbol.Name);
        Assert.Equal(SymbolKind.Function, symbol.Kind);
        Assert.Equal(sig, symbol.Declaration);
    }

    [Fact]
    public void Declares_DeclareFunction_InsideBlock()
    {
        var model = Utility.GetSemanticModel("{ declare fn helper(): string; helper; }");
        Utility.AssertNoErrors(model);
    }
    #endregion Declares
}