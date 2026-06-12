using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;

namespace Loom.Testing;

[Collection("Assembly")]
public class ResolverTest
{
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
    [InlineData("""
                    mut x: number;
                    if outer {
                        if inner {
                            x = 42;
                        }
                    } else {
                        x = 0;
                    }
                    x;
        """)]
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
    public void Allows_NestedScopes_WithSameVariableNames()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("let x = 42; { let x = 69; x; } x;"));
    }
    
    [Fact]
    public void Allows_ReturnStatementInNestedFunction()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("fn outer() { fn inner() { return 42; } return 0; }"));
    }
    
    [Fact]
    public void Allows_ReturnStatementInsideFunction()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("fn test() { return 42; }"));
    }

    [Fact]
    public void Allows_MultipleInitializations_AcrossBranches()
    {
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
    }

    [Fact]
    public void Allows_VariableInitializedInOuterScope_ToBeUsedInInnerScope()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("let x = true; if x { x }"));
    }

    [Fact]
    public void Allows_VariableInitializedBeforeIf_ToBeUsedAfterIf()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; let x = 42; if condition { } x;"));
    }

    [Fact]
    public void Allows_VariableInitializedInBothIfBranches_ToBeUsedAfterIf()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; mut x: number; if condition { x = 42 } else { x = 0 } x;"));
    }

    [Fact]
    public void Allows_VariableInitializedInThenBranch_UsedInsideThenBranch()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; mut x: number; if condition { x = 42; x; }"));
    }

    [Fact]
    public void Allows_VariableFromOuterScope_ToBeReassignedInInnerScope()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("let condition = true; mut x = 1; if condition { x = 2; } x;"));
    }

    [Fact]
    public void TracksInitialization_ThroughNestedBlocks()
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel("mut x: number; { x = 42; } x;"));
    }

    [Theory]
    [InlineData("Range")]
    public void Declares_IntrinsicType_Symbols(string name)
    {
        Utility.AssertNoErrors(Utility.GetSemanticModel($"mut x: {name}"));
    }

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
}