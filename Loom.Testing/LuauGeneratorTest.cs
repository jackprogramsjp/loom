using Loom.Diagnostics;
using Loom.Luau;
using Loom.Luau.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using ElementAccess = Loom.Luau.AST.ElementAccess;
using ExpressionStatement = Loom.Luau.AST.ExpressionStatement;
using FunctionType = Loom.Luau.AST.FunctionType;
using Identifier = Loom.Luau.AST.Identifier;
using IntersectionType = Loom.Luau.AST.IntersectionType;
using OptionalType = Loom.Luau.AST.OptionalType;
using Parenthesized = Loom.Luau.AST.Parenthesized;
using ParenthesizedType = Loom.Luau.AST.ParenthesizedType;
using PrimitiveType = Loom.Luau.AST.PrimitiveType;
using PropertyAccess = Loom.Luau.AST.PropertyAccess;
using Return = Loom.Luau.AST.Return;
using TypeAlias = Loom.Luau.AST.TypeAlias;
using TypeName = Loom.Luau.AST.TypeName;
using UnaryOperator = Loom.Luau.AST.UnaryOperator;
using UnionType = Loom.Luau.AST.UnionType;

namespace Loom.Testing;

[Collection("Assembly")]
public class LuauGeneratorTest
{
    [Fact]
    public void ThrowsFor_BitwiseAssignment_NotImplemented()
    {
        var diagnostics = Utility.GetGeneratorDiagnostics("mut x = 1; x &= 2");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.NotImplemented, "Luau generation for bitwise assignment operators is not yet supported.");
    }

    [Theory]
    [InlineData("declare let x: number;")]
    [InlineData("declare mut x: number;")]
    [InlineData("declare fn x(): number;")]
    public void Generates_Nothing(string source) => Assert.Empty(Utility.GetLuauAST(source).Statements);

    [Fact]
    public void Generates_AfterStatement_WithExpressionBody()
    {
        var luauTree = Utility.GetLuauAST("after 1s foo()");
        Assert.Single(luauTree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var call = Assert.IsType<Call>(exprStmt.Expression);
        var propAccess = Assert.IsType<PropertyAccess>(call.Callee);
        var target = Assert.IsType<Identifier>(propAccess.Target);
        Assert.Equal("task", target.Name);
        Assert.Single(propAccess.Names);
        Assert.Equal("delay", propAccess.Names.First());
        Assert.Equal(2, call.Arguments.Count);

        var duration = Assert.IsType<NumberLiteral>(call.Arguments.First());
        Assert.Equal(1, duration.Value);

        var anonFn = Assert.IsType<AnonymousFunction>(call.Arguments.Last());
        Assert.Null(anonFn.TypeParameters);
        Assert.Empty(anonFn.Parameters);
        Assert.IsType<UnitType>(anonFn.ReturnType);
        Assert.Single(anonFn.Body.Statements);

        var bodyStmt = anonFn.Body.Statements.First();
        var innerCall = Assert.IsType<Call>(Assert.IsType<ExpressionStatement>(bodyStmt).Expression);
        var innerIdent = Assert.IsType<Identifier>(innerCall.Callee);
        Assert.Equal("foo", innerIdent.Name);
        Assert.Empty(innerCall.Arguments);
    }

    [Fact]
    public void Generates_AfterStatement_WithBlockBody()
    {
        var luauTree = Utility.GetLuauAST("after 2s { foo(); bar() }");
        Assert.Single(luauTree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var call = Assert.IsType<Call>(exprStmt.Expression);
        var propAccess = Assert.IsType<PropertyAccess>(call.Callee);
        Assert.Equal("task", ((Identifier)propAccess.Target).Name);
        Assert.Equal("delay", propAccess.Names.First());

        Assert.Equal(2, call.Arguments.Count);
        var duration = Assert.IsType<NumberLiteral>(call.Arguments.First());
        Assert.Equal(2, duration.Value);

        var anonFn = Assert.IsType<AnonymousFunction>(call.Arguments.Last());
        Assert.Empty(anonFn.Parameters);
        Assert.Equal(2, anonFn.Body.Statements.Count);

        var firstStmt = Assert.IsType<Call>(Assert.IsType<ExpressionStatement>(anonFn.Body.Statements.First()).Expression);
        var secondStmt = Assert.IsType<Call>(Assert.IsType<ExpressionStatement>(anonFn.Body.Statements.Last()).Expression);
        Assert.Equal("foo", ((Identifier)firstStmt.Callee).Name);
        Assert.Equal("bar", ((Identifier)secondStmt.Callee).Name);
    }

    [Fact]
    public void Generates_AfterStatement_WithComplexDuration()
    {
        var luauTree = Utility.GetLuauAST("after x + 1 { }");
        Assert.Single(luauTree.Statements);

        var exprStmt = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var call = Assert.IsType<Call>(exprStmt.Expression);
        var propAccess = Assert.IsType<PropertyAccess>(call.Callee);
        Assert.Equal("task", ((Identifier)propAccess.Target).Name);
        Assert.Equal("delay", propAccess.Names.First());

        var duration = Assert.IsType<BinaryOperator>(call.Arguments.First());
        Assert.Equal("+", duration.Operator);
        Assert.IsType<Identifier>(duration.Left);
        Assert.IsType<NumberLiteral>(duration.Right);

        var anonFn = Assert.IsType<AnonymousFunction>(call.Arguments.Last());
        Assert.Empty(anonFn.Body.Statements);
    }

    [Fact]
    public void Generates_AfterStatement_WithNestedAfter()
    {
        var luauTree = Utility.GetLuauAST("after 1s { after 2s foo() }");
        Assert.Single(luauTree.Statements);

        var outerCall = Assert.IsType<Call>(Assert.IsType<ExpressionStatement>(luauTree.Statements.First()).Expression);
        var outerAnon = Assert.IsType<AnonymousFunction>(outerCall.Arguments[1]);
        Assert.Single(outerAnon.Body.Statements);

        var innerExpr = Assert.IsType<ExpressionStatement>(outerAnon.Body.Statements.First());
        var innerCall = Assert.IsType<Call>(innerExpr.Expression);
        var innerProp = Assert.IsType<PropertyAccess>(innerCall.Callee);
        Assert.Equal("task", ((Identifier)innerProp.Target).Name);
        Assert.Equal("delay", innerProp.Names.First());

        var innerDuration = Assert.IsType<NumberLiteral>(innerCall.Arguments.First());
        Assert.Equal(2, innerDuration.Value);

        var innerAnon = Assert.IsType<AnonymousFunction>(innerCall.Arguments.Last());
        Assert.Single(innerAnon.Body.Statements);
        var innerBodyCall = Assert.IsType<Call>(Assert.IsType<ExpressionStatement>(innerAnon.Body.Statements.First()).Expression);
        Assert.Equal("foo", ((Identifier)innerBodyCall.Callee).Name);
    }

    [Fact]
    public void Generates_AfterStatement_WithVariableReferenceInBody()
    {
        var luauTree = Utility.GetLuauAST("let x = 42; after 1s { print(x) }", typeCheck: true);
        Assert.Equal(2, luauTree.Statements.Count);

        var varDecl = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        Assert.Equal("x", varDecl.Name);

        var exprStmt = Assert.IsType<ExpressionStatement>(luauTree.Statements.Last());
        var call = Assert.IsType<Call>(exprStmt.Expression);
        var anonFn = Assert.IsType<AnonymousFunction>(call.Arguments.Last());
        Assert.Single(anonFn.Body.Statements);

        var printCall = Assert.IsType<Call>(Assert.IsType<ExpressionStatement>(anonFn.Body.Statements.First()).Expression);
        var callee = Assert.IsType<Identifier>(printCall.Callee);
        Assert.Equal("print", callee.Name);
        Assert.Single(printCall.Arguments);
        var arg = Assert.IsType<Identifier>(printCall.Arguments.First());
        Assert.Equal("x", arg.Name);
    }

    [Fact]
    public void Generates_AfterStatement_WithReturnInside()
    {
        var luauTree = Utility.GetLuauAST("fn test() { after 1s { return 42 } }", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var fn = Assert.IsType<Function>(luauTree.Statements.First());
        Assert.Single(fn.Body.Statements);

        var afterStmt = Assert.IsType<ExpressionStatement>(fn.Body.Statements.First());
        var call = Assert.IsType<Call>(afterStmt.Expression);
        var anonFn = Assert.IsType<AnonymousFunction>(call.Arguments[1]);
        Assert.Single(anonFn.Body.Statements);

        var returnStmt = Assert.IsType<Return>(anonFn.Body.Statements.First());
        var returnValue = Assert.IsType<NumberLiteral>(returnStmt.Expression);
        Assert.Equal(42, returnValue.Value);
    }

    [Fact]
    public void Generates_WhileLoop_WithBlockBody()
    {
        var luauTree = Utility.GetLuauAST("while true { break }");
        Assert.Single(luauTree.Statements);

        var whileStatement = Assert.IsType<WhileStatement>(luauTree.Statements.First());
        var condition = Assert.IsType<BooleanLiteral>(whileStatement.Condition);
        Assert.True(condition.Value);

        var body = whileStatement.Body;
        Assert.Single(body.Statements);
        Assert.IsType<Break>(body.Statements.First());
    }

    [Fact]
    public void Generates_WhileLoop_WithExpressionBody()
    {
        var luauTree = Utility.GetLuauAST("while true continue");
        Assert.Single(luauTree.Statements);

        var whileStatement = Assert.IsType<WhileStatement>(luauTree.Statements.First());
        var body = whileStatement.Body;
        Assert.Single(body.Statements);
        Assert.IsType<Continue>(body.Statements.First());
    }

    [Fact]
    public void Generates_Break()
    {
        var luauTree = Utility.GetLuauAST("while true { break }");
        var whileStatement = Assert.IsType<WhileStatement>(luauTree.Statements.First());
        var block = whileStatement.Body;
        var breakStmt = Assert.IsType<Break>(block.Statements.First());
        Assert.Equal("break", breakStmt.Render());
    }

    [Fact]
    public void Generates_Continue()
    {
        var luauTree = Utility.GetLuauAST("while true { continue }");
        var whileStatement = Assert.IsType<WhileStatement>(luauTree.Statements.First());
        var block = whileStatement.Body;
        var continueStmt = Assert.IsType<Continue>(block.Statements.First());
        Assert.Equal("continue", continueStmt.Render());
    }

    [Fact]
    public void Generates_NestedWhileLoops_WithBreakAndContinue()
    {
        var luauTree = Utility.GetLuauAST(
            """
                    while a {
                        while b {
                            break
                        }
                        continue
                    }
            """
        );

        Assert.Single(luauTree.Statements);

        var outerWhile = Assert.IsType<WhileStatement>(luauTree.Statements.First());
        var outerBody = outerWhile.Body;
        Assert.Equal(2, outerBody.Statements.Count);

        var innerWhile = Assert.IsType<WhileStatement>(outerBody.Statements.First());
        var innerBody = innerWhile.Body;
        Assert.Single(innerBody.Statements);
        Assert.IsType<Break>(innerBody.Statements.First());

        var outerContinue = Assert.IsType<Luau.AST.Continue>(outerBody.Statements[1]);
        Assert.Equal("continue", outerContinue.Render());
    }

    [Fact]
    public void Generates_InterfaceInvocation_EmptyBody()
    {
        var luauTree = Utility.GetLuauAST("interface I { } new I {}", typeCheck: true);
        Assert.True(luauTree.Statements.Count >= 2);
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Empty(table.Initializers);
    }

    [Fact]
    public void Generates_InterfaceInvocation_PropertyInitializer()
    {
        var luauTree = Utility.GetLuauAST("interface I { x: number } new I { x: 1 }", typeCheck: true);
        Assert.True(luauTree.Statements.Count >= 2);
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Single(table.Initializers);
        var propInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        Assert.Equal("x", propInit.PropertyName);
        var value = Assert.IsType<NumberLiteral>(propInit.Value);
        Assert.Equal(1, value.Value);
    }

    [Fact]
    public void Generates_InterfaceInvocation_IndexInitializer()
    {
        var luauTree = Utility.GetLuauAST("interface I { [number]: string } new I { [0]: 'hello' }", typeCheck: true);
        Assert.True(luauTree.Statements.Count >= 2);
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Single(table.Initializers);
        var indexInit = Assert.IsType<ComputedPropertyTableInitializer>(table.Initializers[0]);
        var indexValue = Assert.IsType<NumberLiteral>(indexInit.Key);
        Assert.Equal(0, indexValue.Value);
        var value = Assert.IsType<StringLiteral>(indexInit.Value);
        Assert.Equal("hello", value.Value);
    }

    [Fact]
    public void Generates_InterfaceInvocation_MixedInitializers()
    {
        var luauTree = Utility.GetLuauAST("interface I { x: number, [string]: bool } new I { x: 1, ['key']: true }", typeCheck: true);
        Assert.True(luauTree.Statements.Count >= 2);
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Equal(2, table.Initializers.Count);
        var propInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        Assert.Equal("x", propInit.PropertyName);
        var indexInit = Assert.IsType<ComputedPropertyTableInitializer>(table.Initializers[1]);
        var key = Assert.IsType<StringLiteral>(indexInit.Key);
        Assert.Equal("key", key.Value);
        var val = Assert.IsType<BooleanLiteral>(indexInit.Value);
        Assert.True(val.Value);
    }

    [Fact]
    public void Generates_InterfaceInvocation_ChainedProperty()
    {
        var luauTree = Utility.GetLuauAST("interface I { x: number } let _ = new I { x: 1 }.x", typeCheck: true);
        Assert.True(luauTree.Statements.Count >= 2);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var propAccess = Assert.IsType<PropertyAccess>(variable.Initializer);
        Assert.Equal("x", propAccess.Names[0]);

        var table = Assert.IsType<Table>(propAccess.Target);
        Assert.Single(table.Initializers);
        var propInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        Assert.Equal("x", propInit.PropertyName);
    }

    [Fact]
    public void Generates_IfStatement_SingleExpressionThenBranch()
    {
        var luauTree = Utility.GetLuauAST("if true return 1");
        Assert.Single(luauTree.Statements);

        var ifStatement = Assert.IsType<IfStatement>(luauTree.Statements.First());
        Assert.Single(ifStatement.ThenBranch.Statements);
        Assert.IsType<Return>(ifStatement.ThenBranch.Statements[0]);
    }

    [Fact]
    public void Generates_IfStatement_SingleExpressionElseBranch()
    {
        var luauTree = Utility.GetLuauAST("if true { return 1 } else return 0");
        Assert.Single(luauTree.Statements);

        var ifStatement = Assert.IsType<IfStatement>(luauTree.Statements.First());
        Assert.NotNull(ifStatement.ElseBranch);
        Assert.Single(ifStatement.ElseBranch.Statements);
        Assert.IsType<Return>(ifStatement.ElseBranch.Statements[0]);
    }

    [Fact]
    public void Generates_Declared_InterfaceDeclaration()
    {
        var luauTree = Utility.GetLuauAST("declare interface I;", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("I", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        Assert.Null(tableType.Indexer);
        Assert.Empty(tableType.Properties);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_NoBody()
    {
        var luauTree = Utility.GetLuauAST("interface I;", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("I", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        Assert.Null(tableType.Indexer);
        Assert.Empty(tableType.Properties);
    }

    [Fact]
    public void Generates_CompoundAssignment()
    {
        var luauTree = Utility.GetLuauAST("mut x = 1; x += 2");
        Assert.Equal(2, luauTree.Statements.Count);

        var exprStmt = Assert.IsType<ExpressionStatement>(luauTree.Statements[1]);
        var binary = Assert.IsType<BinaryOperator>(exprStmt.Expression);
        Assert.Equal("x", ((Identifier)binary.Left).Name);
        Assert.Equal(2, ((NumberLiteral)binary.Right).Value);
        Assert.Equal("+=", binary.Operator);
    }

    [Fact]
    public void Generates_BitwiseAssignment_NotImplemented()
    {
        var luauTree = Utility.GetLuauAST("mut x = 1; x &= 2");
        Assert.Equal(2, luauTree.Statements.Count);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var binary = Assert.IsType<BinaryOperator>(variable.Initializer);
        Assert.Equal("???", binary.Operator);
        Assert.IsType<Identifier>(binary.Left);
        Assert.IsType<NumberLiteral>(binary.Right);
    }

    [Fact]
    public void Generates_ElementAccess_StringIndex()
    {
        var luauTree = Utility.GetLuauAST("interface I { [string]: number } let x = none as never as I; x['key']", typeCheck: true);
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.Last());
        var elementAccess = Assert.IsType<ElementAccess>(variable.Initializer);
        var target = Assert.IsType<Identifier>(elementAccess.Target);
        Assert.Equal("x", target.Name);
        var index = Assert.IsType<StringLiteral>(elementAccess.Index);
        Assert.Equal("key", index.Value);
    }

    [Fact]
    public void Generates_PropertyAccessAssignment()
    {
        var luauTree = Utility.GetLuauAST("interface I { mut prop: number } let obj = none as never as I; obj.prop = 42", typeCheck: true);
        Assert.True(luauTree.Statements.Count >= 3);
        var exprStmt = Assert.IsType<ExpressionStatement>(luauTree.Statements.Last());
        var assignment = Assert.IsType<BinaryOperator>(exprStmt.Expression);
        var left = Assert.IsType<PropertyAccess>(assignment.Left);
        var right = Assert.IsType<NumberLiteral>(assignment.Right);
        Assert.Equal("obj", ((Identifier)left.Target).Name);
        Assert.Equal("prop", left.Names[0]);
        Assert.Equal(42, right.Value);
    }

    [Fact]
    public void Generates_QualifiedNameAssignment()
    {
        var luauTree = Utility.GetLuauAST("interface Mod { mut value: number } let mod = none as never as Mod; mod.value = 99", typeCheck: true);
        Assert.True(luauTree.Statements.Count >= 3);
        var exprStmt = Assert.IsType<ExpressionStatement>(luauTree.Statements.Last());
        var assignment = Assert.IsType<BinaryOperator>(exprStmt.Expression);
        var left = Assert.IsType<PropertyAccess>(assignment.Left);
        var right = Assert.IsType<NumberLiteral>(assignment.Right);
        Assert.Equal("mod", ((Identifier)left.Target).Name);
        Assert.Equal("value", left.Names[0]);
        Assert.Equal(99, right.Value);
    }

    [Fact]
    public void Generates_IdentifierAssignment()
    {
        var luauTree = Utility.GetLuauAST("mut a = 0; let x = a = 1");
        Assert.Equal(3, luauTree.Statements.Count);

        var aVar = Assert.IsType<LocalVariable>(luauTree.Statements[0]);
        Assert.Equal("a", aVar.Name);

        var postreq = Assert.IsType<ExpressionStatement>(luauTree.Statements[1]);
        var assignment = Assert.IsType<BinaryOperator>(postreq.Expression);
        Assert.Equal("a", ((Identifier)assignment.Left).Name);
        var rhs = Assert.IsType<NumberLiteral>(assignment.Right);
        Assert.Equal(1, rhs.Value);

        var xVar = Assert.IsType<ConstVariable>(luauTree.Statements[2]);
        Assert.Equal("x", xVar.Name);
        var init = Assert.IsType<Identifier>(xVar.Initializer);
        Assert.Equal("a", init.Name);
    }

    [Fact]
    public void Generates_FunctionType_WithLiteralReturnType()
    {
        var luauTree = Utility.GetLuauAST("type X = fn(): 0");
        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.Single());
        var fnType = Assert.IsType<FunctionType>(typeAlias.Type);
        var returnType = Assert.IsType<PrimitiveType>(fnType.ReturnType);
        Assert.Equal(PrimitiveTypeKind.Number, returnType.Kind);
    }

    [Fact]
    public void Generates_VariableDeclaration_WithoutInitializer()
    {
        var luauTree = Utility.GetLuauAST("let x: number;");
        Assert.Single(luauTree.Statements);
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        Assert.Equal("x", variable.Name);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_Empty()
    {
        var luauTree = Utility.GetLuauAST("interface I { }", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("I", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        Assert.Null(tableType.Indexer);
        Assert.Empty(tableType.Properties);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_WithProperties()
    {
        var luauTree = Utility.GetLuauAST("interface I { x: number, y: string }", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        Assert.Equal(2, tableType.Properties.Count);

        var propX = tableType.Properties[0];
        Assert.Equal("x", propX.Name);
        Assert.Equal(LuauVisibility.Read, propX.Visibility);
        Assert.IsType<PrimitiveType>(propX.Type);
        Assert.Equal(PrimitiveTypeKind.Number, ((PrimitiveType)propX.Type).Kind);

        var propY = tableType.Properties[1];
        Assert.Equal("y", propY.Name);
        Assert.Equal(LuauVisibility.Read, propY.Visibility);
        Assert.IsType<PrimitiveType>(propY.Type);
        Assert.Equal(PrimitiveTypeKind.String, ((PrimitiveType)propY.Type).Kind);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_WithMutableProperty()
    {
        var luauTree = Utility.GetLuauAST("interface I { mut count: number }", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        var prop = tableType.Properties.Single();
        Assert.Equal("count", prop.Name);
        Assert.Null(prop.Visibility);
        Assert.IsType<PrimitiveType>(prop.Type);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_WithIndexer()
    {
        var luauTree = Utility.GetLuauAST("interface I { [number]: string }", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        Assert.NotNull(tableType.Indexer);

        var keyType = Assert.IsType<PrimitiveType>(tableType.Indexer.KeyType);
        var valueType = Assert.IsType<PrimitiveType>(tableType.Indexer.ValueType);
        Assert.Equal(PrimitiveTypeKind.Number, keyType.Kind);
        Assert.Equal(PrimitiveTypeKind.String, valueType.Kind);
        Assert.Empty(tableType.Properties);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_WithStringIndexer()
    {
        var luauTree = Utility.GetLuauAST("interface I { [string]: bool }", typeCheck: true);
        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.Single());
        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        Assert.NotNull(tableType.Indexer);
        Assert.IsType<PrimitiveType>(tableType.Indexer.ValueType);
        Assert.Equal(PrimitiveTypeKind.Boolean, ((PrimitiveType)tableType.Indexer.ValueType).Kind);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_WithIndexerAndProperties()
    {
        var luauTree = Utility.GetLuauAST("interface I { [number]: string, name: string, mut counter: number }", typeCheck: true);
        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.Single());
        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        Assert.NotNull(tableType.Indexer);
        Assert.Equal(2, tableType.Properties.Count);
        Assert.Contains(tableType.Properties, p => p is { Name: "name", Visibility: LuauVisibility.Read });
        Assert.Contains(tableType.Properties, p => p is { Name: "counter", Visibility: null });
    }

    [Fact]
    public void Generates_InterfaceDeclaration_WithSingleConstraint()
    {
        var luauTree = Utility.GetLuauAST("interface Base {}; interface I : Base { }", typeCheck: true);
        Assert.Equal(2, luauTree.Statements.Count);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.Last());
        var intersection = Assert.IsType<IntersectionType>(typeAlias.Type);
        Assert.Equal(2, intersection.Types.Count);

        var constraintType = Assert.IsType<TypeName>(intersection.Types[0]);
        Assert.Equal("Base", constraintType.Name);

        var tableType = Assert.IsType<TableType>(intersection.Types[1]);
        Assert.Empty(tableType.Properties);
        Assert.Null(tableType.Indexer);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_WithMultipleConstraints()
    {
        var luauTree = Utility.GetLuauAST("interface A {} interface B {} interface I : A, B { }", typeCheck: true);
        Assert.Equal(3, luauTree.Statements.Count);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.Last());
        var intersection = Assert.IsType<IntersectionType>(typeAlias.Type);
        Assert.Equal(3, intersection.Types.Count);
        Assert.Equal("A", ((TypeName)intersection.Types[0]).Name);
        Assert.Equal("B", ((TypeName)intersection.Types[1]).Name);
        Assert.IsType<TableType>(intersection.Types[2]);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_Generic()
    {
        var luauTree = Utility.GetLuauAST("interface Container<T> { value: T }", typeCheck: true);
        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.Single());
        Assert.Single(typeAlias.TypeParameters.Parameters);
        Assert.Equal("T", typeAlias.TypeParameters.Parameters[0].Name);
        Assert.False(typeAlias.TypeParameters.Parameters[0].OfFunction);

        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        var prop = tableType.Properties.Single();
        Assert.Equal("value", prop.Name);
        var propType = Assert.IsType<TypeName>(prop.Type);
        Assert.Equal("T", propType.Name);
    }

    [Fact]
    public void Generates_InterfaceDeclaration_GenericWithConstraintAndDefault()
    {
        var luauTree = Utility.GetLuauAST("interface Repo<T: number = 42> { item: T }", typeCheck: true);
        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.Single());
        Assert.Single(typeAlias.TypeParameters.Parameters);

        var tp = typeAlias.TypeParameters.Parameters.First();
        Assert.Equal("T", tp.Name);
        Assert.NotNull(tp.DefaultType);
        Assert.IsType<PrimitiveType>(tp.DefaultType);
        Assert.Equal(PrimitiveTypeKind.Number, ((PrimitiveType)tp.DefaultType).Kind);

        var tableType = Assert.IsType<TableType>(typeAlias.Type);
        var prop = tableType.Properties.Single();
        Assert.Equal("item", prop.Name);

        var intersection = Assert.IsType<IntersectionType>(prop.Type);
        Assert.Equal(2, intersection.Types.Count);
        Assert.IsType<TypeName>(intersection.Types.First());
    }

    [Theory]
    [InlineData("none")]
    [InlineData("void")]
    public void Generates_FunctionType_WithPrimitiveReturn_ThatUsesUnitConversion(string returnType)
    {
        var luauTree = Utility.GetLuauAST($"type Callback = fn(): {returnType}");
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        var functionType = Assert.IsType<FunctionType>(typeAlias.Type);
        Assert.Empty(functionType.ParameterTypes);
        Assert.IsType<UnitType>(functionType.ReturnType);

        var rendered = functionType.Render();
        Assert.Contains("()", rendered);
    }

    [Fact]
    public void Generates_FunctionType()
    {
        var luauTree = Utility.GetLuauAST("type Optional = fn(x: number, y: string?): bool");
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        var functionType = Assert.IsType<FunctionType>(typeAlias.Type);
        Assert.Equal(2, functionType.ParameterTypes.Count);
        Assert.IsType<PrimitiveType>(functionType.ParameterTypes.First());
        Assert.IsType<OptionalType>(functionType.ParameterTypes.Last());
        Assert.IsType<PrimitiveType>(functionType.ReturnType);
    }

    [Fact]
    public void Generates_IndexedType()
    {
        var luauTree = Utility.GetLuauAST("type Foo = number[]; type X = Foo[number]");
        Assert.Equal(2, luauTree.Statements.Count);

        var alias = Assert.IsType<TypeAlias>(luauTree.Statements.Last());
        Assert.Empty(alias.TypeParameters.Parameters);

        var indexTypeFn = Assert.IsType<TypeName>(alias.Type);
        Assert.Equal("index", indexTypeFn.Name);
        Assert.Equal(2, indexTypeFn.TypeArguments.Count);

        var self = Assert.IsType<TypeName>(indexTypeFn.TypeArguments.First());
        Assert.Equal("Foo", self.Name);
        Assert.Empty(self.TypeArguments);

        var inner = Assert.IsType<PrimitiveType>(indexTypeFn.TypeArguments.Last());
        Assert.Equal(PrimitiveTypeKind.Number, inner.Kind);
    }

    [Fact]
    public void Generates_Array_TableType()
    {
        var luauTree = Utility.GetLuauAST("mut x: number[];");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var table = Assert.IsType<TableType>(variable.DeclaredType);
        Assert.NotNull(table.Indexer);
        Assert.Null(table.Indexer.KeyType);

        var inner = Assert.IsType<PrimitiveType>(table.Indexer.ValueType);
        Assert.Equal(PrimitiveTypeKind.Number, inner.Kind);
    }

    [Fact]
    public void Generates_IntersectionType()
    {
        var luauTree = Utility.GetLuauAST("mut x: number & bool");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var intersection = Assert.IsType<IntersectionType>(variable.DeclaredType);
        Assert.Equal(2, intersection.Types.Count);

        var left = Assert.IsType<PrimitiveType>(intersection.Types.First());
        var right = Assert.IsType<PrimitiveType>(intersection.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, left.Kind);
        Assert.Equal(PrimitiveTypeKind.Boolean, right.Kind);
    }

    [Fact]
    public void Generates_UnionType()
    {
        var luauTree = Utility.GetLuauAST("mut x: number | bool");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var union = Assert.IsType<UnionType>(variable.DeclaredType);
        Assert.Equal(2, union.Types.Count);

        var left = Assert.IsType<PrimitiveType>(union.Types.First());
        var right = Assert.IsType<PrimitiveType>(union.Types.Last());
        Assert.Equal(PrimitiveTypeKind.Number, left.Kind);
        Assert.Equal(PrimitiveTypeKind.Boolean, right.Kind);
    }

    [Fact]
    public void Generates_OptionalType()
    {
        var luauTree = Utility.GetLuauAST("mut x: number?;");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var optional = Assert.IsType<OptionalType>(variable.DeclaredType);
        var inner = Assert.IsType<PrimitiveType>(optional.Inner);
        Assert.Equal(PrimitiveTypeKind.Number, inner.Kind);
    }

    [Fact]
    public void Generates_BooleanLiteralType()
    {
        var luauTree = Utility.GetLuauAST("mut x: true;");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var literalType = Assert.IsType<BooleanLiteralType>(variable.DeclaredType);
        Assert.Equal("true", literalType.Render());
    }

    [Fact]
    public void Generates_StringLiteralType()
    {
        var luauTree = Utility.GetLuauAST("mut x: 'abc';");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var literalType = Assert.IsType<StringLiteralType>(variable.DeclaredType);
        Assert.Equal("\"abc\"", literalType.Render());
    }

    [Fact]
    public void Generates_Unusable_LiteralType()
    {
        var luauTree = Utility.GetLuauAST("mut x: 42069;");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var primitive = Assert.IsType<PrimitiveType>(variable.DeclaredType);
        Assert.Equal("number", primitive.Render());
    }

    [Fact]
    public void Generates_ParenthesizedType()
    {
        var luauTree = Utility.GetLuauAST("mut x: (number);");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var parenthesized = Assert.IsType<ParenthesizedType>(variable.DeclaredType);
        var primitive = Assert.IsType<PrimitiveType>(parenthesized.Type);
        Assert.Equal("number", primitive.Render());
    }

    [Theory]
    [InlineData("number")]
    [InlineData("string")]
    [InlineData("bool", "boolean")]
    [InlineData("never")]
    [InlineData("unknown")]
    [InlineData("none", "nil")]
    [InlineData("void", "nil")]
    public void Generates_PrimitiveTypes(string name, string? expected = null)
    {
        var luauTree = Utility.GetLuauAST($"mut x: {name};");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var primitive = Assert.IsType<PrimitiveType>(variable.DeclaredType);
        Assert.Equal(expected ?? name, primitive.Render());
    }

    [Fact]
    public void Generates_SimpleIfStatement()
    {
        var luauTree = Utility.GetLuauAST("if true { return 1 }");
        Assert.Single(luauTree.Statements);

        var ifStatement = Assert.IsType<IfStatement>(luauTree.Statements.First());
        var condition = Assert.IsType<BooleanLiteral>(ifStatement.Condition);
        Assert.True(condition.Value);
        Assert.Single(ifStatement.ThenBranch.Statements);

        var returnStatement = Assert.IsType<Return>(ifStatement.ThenBranch.Statements.First());
        var returnValue = Assert.IsType<NumberLiteral>(returnStatement.Expression);
        Assert.Equal(1, returnValue.Value);
        Assert.Empty(ifStatement.ElseIfBranches);
        Assert.Null(ifStatement.ElseBranch);
    }

    [Fact]
    public void Generates_IfElseStatement()
    {
        var luauTree = Utility.GetLuauAST("if x > 5 { return 1 } else { return 0 }");
        Assert.Single(luauTree.Statements);

        var ifStatement = Assert.IsType<IfStatement>(luauTree.Statements.First());
        var condition = Assert.IsType<BinaryOperator>(ifStatement.Condition);
        var left = Assert.IsType<Identifier>(condition.Left);
        var right = Assert.IsType<NumberLiteral>(condition.Right);
        Assert.Equal("x", left.Name);
        Assert.Equal(5, right.Value);
        Assert.Equal(">", condition.Operator);
        Assert.Single(ifStatement.ThenBranch.Statements);

        var thenReturn = Assert.IsType<Return>(ifStatement.ThenBranch.Statements.First());
        var thenValue = Assert.IsType<NumberLiteral>(thenReturn.Expression);
        Assert.Equal(1, thenValue.Value);
        Assert.NotNull(ifStatement.ElseBranch);
        Assert.Single(ifStatement.ElseBranch.Statements);

        var elseReturn = Assert.IsType<Return>(ifStatement.ElseBranch.Statements.First());
        var elseValue = Assert.IsType<NumberLiteral>(elseReturn.Expression);
        Assert.Equal(0, elseValue.Value);
        Assert.Empty(ifStatement.ElseIfBranches);
    }

    [Fact]
    public void Generates_IfElseIfStatement()
    {
        var luauTree = Utility.GetLuauAST("if x > 5 { return 1 } else if x < 0 { return -1 } else { return 0 }");
        Assert.Single(luauTree.Statements);

        var ifStatement = Assert.IsType<IfStatement>(luauTree.Statements.First());
        var mainCondition = Assert.IsType<BinaryOperator>(ifStatement.Condition);
        var mainLeft = Assert.IsType<Identifier>(mainCondition.Left);
        var mainRight = Assert.IsType<NumberLiteral>(mainCondition.Right);
        Assert.Equal("x", mainLeft.Name);
        Assert.Equal(5, mainRight.Value);
        Assert.Single(ifStatement.ThenBranch.Statements);
        Assert.Single(ifStatement.ElseIfBranches);

        var elseIf = ifStatement.ElseIfBranches.First();
        var elseIfCondition = Assert.IsType<BinaryOperator>(elseIf.Condition);
        var elseIfLeft = Assert.IsType<Identifier>(elseIfCondition.Left);
        var elseIfRight = Assert.IsType<NumberLiteral>(elseIfCondition.Right);
        Assert.Equal("x", elseIfLeft.Name);
        Assert.Equal(0, elseIfRight.Value);
        Assert.Equal("<", elseIfCondition.Operator);
        Assert.Single(elseIf.Branch.Statements);

        var elseIfReturn = Assert.IsType<Return>(elseIf.Branch.Statements.First());
        var elseIfUnary = Assert.IsType<UnaryOperator>(elseIfReturn.Expression);
        var unaryValue = Assert.IsType<NumberLiteral>(elseIfUnary.Operand);
        Assert.Equal("-", elseIfUnary.Operator);
        Assert.Equal(1, unaryValue.Value);
        Assert.NotNull(ifStatement.ElseBranch);
        Assert.Single(ifStatement.ElseBranch.Statements);

        var elseReturn = Assert.IsType<Return>(ifStatement.ElseBranch.Statements.First());
        var elseValue = Assert.IsType<NumberLiteral>(elseReturn.Expression);
        Assert.Equal(0, elseValue.Value);
    }

    [Fact]
    public void Generates_MultipleElseIfBranches()
    {
        var luauTree = Utility.GetLuauAST("if x == 1 { return 1 } else if x == 2 { return 2 } else if x == 3 { return 3 } else { return 0 }");
        Assert.Single(luauTree.Statements);

        var ifStatement = Assert.IsType<IfStatement>(luauTree.Statements.First());
        Assert.Equal(2, ifStatement.ElseIfBranches.Count);

        var firstElseIf = ifStatement.ElseIfBranches[0];
        var firstCondition = Assert.IsType<BinaryOperator>(firstElseIf.Condition);
        Assert.Equal("==", firstCondition.Operator);

        var firstValue = Assert.IsType<NumberLiteral>(firstCondition.Right);
        Assert.Equal(2, firstValue.Value);

        var secondElseIf = ifStatement.ElseIfBranches[1];
        var secondCondition = Assert.IsType<BinaryOperator>(secondElseIf.Condition);
        Assert.Equal("==", secondCondition.Operator);

        var secondValue = Assert.IsType<NumberLiteral>(secondCondition.Right);
        Assert.Equal(3, secondValue.Value);
        Assert.NotNull(ifStatement.ElseBranch);
    }

    [Fact]
    public void Generates_IfStatement_WithBlockBody()
    {
        var luauTree = Utility.GetLuauAST("if true { mut x = 1; mut y = 2; }");
        Assert.Single(luauTree.Statements);

        var ifStatement = Assert.IsType<IfStatement>(luauTree.Statements.First());
        Assert.Equal(2, ifStatement.ThenBranch.Statements.Count);
        Assert.IsType<LocalVariable>(ifStatement.ThenBranch.Statements[0]);
        Assert.IsType<LocalVariable>(ifStatement.ThenBranch.Statements[1]);
    }

    [Fact]
    public void Generates_NestedIfStatements()
    {
        var luauTree = Utility.GetLuauAST("if x > 0 { if y > 0 { return 1 } }");
        Assert.Single(luauTree.Statements);

        var outerIf = Assert.IsType<IfStatement>(luauTree.Statements.First());
        Assert.Single(outerIf.ThenBranch.Statements);

        var innerIf = Assert.IsType<IfStatement>(outerIf.ThenBranch.Statements.First());
        var innerCondition = Assert.IsType<BinaryOperator>(innerIf.Condition);
        var innerLeft = Assert.IsType<Identifier>(innerCondition.Left);
        Assert.Equal("y", innerLeft.Name);
        Assert.Single(innerIf.ThenBranch.Statements);
        Assert.IsType<Return>(innerIf.ThenBranch.Statements.First());
    }

    [Fact]
    public void Generates_EnumDeclaration_AsNumberTypeAlias()
    {
        var luauTree = Utility.GetLuauAST("enum Abc { A, B, C }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Abc", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var primitive = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
        Assert.Equal("number", primitive.Render());
    }

    [Fact]
    public void Generates_EnumDeclaration_WithExplicitNumberValues_AsNumberTypeAlias()
    {
        var luauTree = Utility.GetLuauAST("enum Status { Active = 1, Inactive = 0, Pending = 2 }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Status", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var primitive = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
        Assert.Equal("number", primitive.Render());
    }

    [Fact]
    public void Generates_EnumDeclaration_WithStringValues_AsUnionOfStringLiterals()
    {
        var luauTree = Utility.GetLuauAST("enum Colors : string { Red = \"FF0000\", Green = \"00FF00\", Blue = \"0000FF\" }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Colors", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var union = Assert.IsType<UnionType>(typeAlias.Type);
        Assert.Equal(3, union.Types.Count);

        var red = Assert.IsType<StringLiteralType>(union.Types[0]);
        var green = Assert.IsType<StringLiteralType>(union.Types[1]);
        var blue = Assert.IsType<StringLiteralType>(union.Types[2]);
        Assert.Equal("\"FF0000\"", red.Render());
        Assert.Equal("\"00FF00\"", green.Render());
        Assert.Equal("\"0000FF\"", blue.Render());
        Assert.Equal("\"FF0000\" | \"00FF00\" | \"0000FF\"", union.Render());
    }

    [Fact]
    public void Generates_EnumDeclaration_WithMixedValues_AsNumberTypeAlias()
    {
        var luauTree = Utility.GetLuauAST("enum Mixed { A, B = 69, C }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Mixed", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var primitive = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
        Assert.Equal("number", primitive.Render());
    }

    [Fact]
    public void Generates_EnumDeclaration_WithDuplicateStringValues_AsUnionWithDuplicatesRemoved()
    {
        var luauTree = Utility.GetLuauAST("enum Duplicates : string { A = \"same\", B = \"same\", C = \"different\" }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Duplicates", typeAlias.Name);

        var union = Assert.IsType<UnionType>(typeAlias.Type);
        Assert.Equal(2, union.Types.Count);

        var literalTypes = union.Types.Cast<StringLiteralType>().ToList();
        Assert.Contains(literalTypes, t => t.Render() == "\"same\"");
        Assert.Contains(literalTypes, t => t.Render() == "\"different\"");
        Assert.Equal("\"same\" | \"different\"", union.Render());
    }

    [Fact]
    public void Generates_EmptyEnum_AsNumberTypeAlias()
    {
        var luauTree = Utility.GetLuauAST("enum Empty { }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Empty", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var never = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal(PrimitiveTypeKind.Number, never.Kind);
    }

    [Fact]
    public void Generates_EnumDeclaration_WithSingleStringValue_AsStringLiteralType()
    {
        var luauTree = Utility.GetLuauAST("enum Single : string { Only = \"value\" }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Single", typeAlias.Name);

        var literalType = Assert.IsType<StringLiteralType>(typeAlias.Type);
        Assert.Equal("\"value\"", literalType.Render());
    }

    [Fact]
    public void Generates_EnumDeclaration_WithNumberBaseTypeExplicit_AsNumberTypeAlias()
    {
        var luauTree = Utility.GetLuauAST("enum Values : number { One = 1, Two = 2, Three = 3 }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Values", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var primitive = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Generates_EnumDeclaration_WithNumberBaseTypeImplicit_AsNumberTypeAlias()
    {
        var luauTree = Utility.GetLuauAST("enum Values : number { A, B, C }", true);
        Assert.Single(luauTree.Statements);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Values", typeAlias.Name);
        Assert.Empty(typeAlias.TypeParameters.Parameters);

        var primitive = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);
    }

    [Fact]
    public void Generates_EnumAccess_AsLiteralValue()
    {
        var luauTree = Utility.GetLuauAST("enum Abc { A, B, C }; let x = Abc.A", true);
        Assert.Equal(2, luauTree.Statements.Count);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements[0]);
        Assert.Equal("Abc", typeAlias.Name);
        var primitive = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal("number", primitive.Render());

        var x = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        Assert.Equal("x", x.Name);
        var literal = Assert.IsType<NumberLiteral>(x.Initializer);
        Assert.Equal(0, literal.Value);
    }

    [Fact]
    public void Generates_EnumInVariableTypeAnnotation()
    {
        var luauTree = Utility.GetLuauAST("enum Status { Active, Inactive }; let x: Status = Status.Active", true);
        Assert.Equal(2, luauTree.Statements.Count);

        var typeAlias = Assert.IsType<TypeAlias>(luauTree.Statements[0]);
        Assert.Equal("Status", typeAlias.Name);
        var primitive = Assert.IsType<PrimitiveType>(typeAlias.Type);
        Assert.Equal("number", primitive.Render());

        var x = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        Assert.Equal("x", x.Name);
        Assert.NotNull(x.DeclaredType);
        var typeName = Assert.IsType<TypeName>(x.DeclaredType);
        Assert.Equal("Status", typeName.Name);
        Assert.Empty(typeName.TypeArguments);

        var literal = Assert.IsType<NumberLiteral>(x.Initializer);
        Assert.Equal(0, literal.Value);
    }

    [Fact]
    public void Generates_TypeAliases_GenericWithDefault()
    {
        var luauTree = Utility.GetLuauAST("type Id<T = number> = T");
        Assert.Single(luauTree.Statements);

        var alias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Id", alias.Name);
        Assert.Single(alias.TypeParameters.Parameters);

        var parameter = alias.TypeParameters.Parameters.First();
        Assert.Equal("T", parameter.Name);

        var primitive = Assert.IsType<PrimitiveType>(parameter.DefaultType);
        Assert.Equal(PrimitiveTypeKind.Number, primitive.Kind);

        var typeName = Assert.IsType<TypeName>(alias.Type);
        Assert.Equal("T", typeName.Name);
    }

    [Fact]
    public void Generates_TypeAliases_Generic()
    {
        var luauTree = Utility.GetLuauAST("type Id<T> = T");
        Assert.Single(luauTree.Statements);

        var alias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("Id", alias.Name);
        Assert.Single(alias.TypeParameters.Parameters);

        var parameter = alias.TypeParameters.Parameters.First();
        Assert.Equal("T", parameter.Name);
        Assert.Null(parameter.DefaultType);

        var typeName = Assert.IsType<TypeName>(alias.Type);
        Assert.Equal("T", typeName.Name);
    }

    [Fact]
    public void Generates_TypeAliases()
    {
        var luauTree = Utility.GetLuauAST("type A = bool");
        Assert.Single(luauTree.Statements);

        var alias = Assert.IsType<TypeAlias>(luauTree.Statements.First());
        Assert.Equal("A", alias.Name);

        var primitive = Assert.IsType<PrimitiveType>(alias.Type);
        Assert.Equal(PrimitiveTypeKind.Boolean, primitive.Kind);
    }

    [Fact]
    public void Generates_QualifiedName_AsPropertyAccessChain()
    {
        var luauTree = Utility.GetLuauAST("a.b");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var propAccess = Assert.IsType<PropertyAccess>(variable.Initializer);
        var target = Assert.IsType<Identifier>(propAccess.Target);
        Assert.Equal("a", target.Name);
        Assert.Single(propAccess.Names);
        Assert.Equal("b", propAccess.Names[0]);
    }

    [Fact]
    public void Generates_QualifiedName_Chained()
    {
        var luauTree = Utility.GetLuauAST("a.b.c");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var outerAccess = Assert.IsType<PropertyAccess>(variable.Initializer);
        Assert.Equal(2, outerAccess.Names.Count);
        Assert.Equal("b", outerAccess.Names.First());
        Assert.Equal("c", outerAccess.Names.Last());
    }

    [Fact]
    public void Generates_PropertyAccess_OnRangeLiteral()
    {
        var luauTree = Utility.GetLuauAST("(1..10).minimum");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var propAccess = Assert.IsType<PropertyAccess>(variable.Initializer);
        Assert.Single(propAccess.Names);
        Assert.Equal("minimum", propAccess.Names[0]);

        var parenthesized = Assert.IsType<Parenthesized>(propAccess.Target);
        var rangeTable = Assert.IsType<Table>(parenthesized.Expression);
        Assert.Equal(2, rangeTable.Initializers.Count);
        var minInit = Assert.IsType<PropertyTableInitializer>(rangeTable.Initializers[0]);
        var maxInit = Assert.IsType<PropertyTableInitializer>(rangeTable.Initializers[1]);
        Assert.Equal("minimum", minInit.PropertyName);
        Assert.Equal("maximum", maxInit.PropertyName);
        Assert.IsType<NumberLiteral>(minInit.Value);
        Assert.IsType<NumberLiteral>(maxInit.Value);
    }

    [Fact]
    public void Generates_PropertyAccess_OnVariable()
    {
        var luauTree = Utility.GetLuauAST("let r = 1..10; r.minimum");
        Assert.Equal(2, luauTree.Statements.Count);

        var rVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        Assert.Equal("r", rVariable.Name);
        Assert.IsType<Table>(rVariable.Initializer);

        var accessVariable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var propAccess = Assert.IsType<PropertyAccess>(accessVariable.Initializer);
        Assert.Single(propAccess.Names);
        Assert.Equal("minimum", propAccess.Names[0]);

        var target = Assert.IsType<Identifier>(propAccess.Target);
        Assert.Equal("r", target.Name);
    }

    [Fact]
    public void Generates_ComputedAssignment()
    {
        var luauTree = Utility.GetLuauAST("mut x = 1; mut y = 2; let z = x = y = 69");
        Assert.Equal(5, luauTree.Statements.Count);

        {
            var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements[2]);
            var assignment = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
            var target = Assert.IsType<Identifier>(assignment.Left);
            var value = Assert.IsType<NumberLiteral>(assignment.Right);
            Assert.Equal("=", assignment.Operator);
            Assert.Equal("y", target.Name);
            Assert.Equal(69, value.Value);
        }

        {
            var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements[3]);
            var assignment = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
            var target = Assert.IsType<Identifier>(assignment.Left);
            var value = Assert.IsType<Identifier>(assignment.Right);
            Assert.Equal("=", assignment.Operator);
            Assert.Equal("x", target.Name);
            Assert.Equal("y", value.Name);
        }

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.Last());
        Assert.Null(variable.DeclaredType);

        var finalValue = Assert.IsType<Identifier>(variable.Initializer);
        Assert.Equal("x", finalValue.Name);
    }

    [Fact]
    public void Generates_BasicAssignment()
    {
        var luauTree = Utility.GetLuauAST("mut x = 1; x = 69");
        Assert.Equal(2, luauTree.Statements.Count);

        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.Last());
        var assignment = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
        var target = Assert.IsType<Identifier>(assignment.Left);
        var value = Assert.IsType<NumberLiteral>(assignment.Right);
        Assert.Equal("=", assignment.Operator);
        Assert.Equal("x", target.Name);
        Assert.Equal(69, value.Value);
    }

    [Fact]
    public void Generates_ExpressionBody_Functions()
    {
        var luauTree = Utility.GetLuauAST("fn abc -> 69");
        Assert.Single(luauTree.Statements);

        var fn = Assert.IsType<Function>(luauTree.Statements.First());
        Assert.Null(fn.ReturnType);
        Assert.Null(fn.TypeParameters);
        Assert.Empty(fn.Parameters);
        Assert.Single(fn.Body.Statements);
        Assert.Equal("abc", fn.Name);

        var returnStatement = Assert.IsType<Return>(fn.Body.Statements.First());
        var literal = Assert.IsType<NumberLiteral>(returnStatement.Expression);
        Assert.Equal(69, literal.Value);
    }

    [Theory]
    [InlineData("fn id<T: number>(value: T): T -> value", PrimitiveTypeKind.Number)]
    [InlineData("fn id<T>(value: T): T -> value")]
    [InlineData("fn id<T>(value: T): T { return value }")]
    public void Generates_Generic_Functions(string source, PrimitiveTypeKind? expectedConstraintKind = null)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);

        var fn = Assert.IsType<Function>(luauTree.Statements.First());
        if (expectedConstraintKind != null)
        {
            var intersection = Assert.IsType<IntersectionType>(fn.ReturnType);
            Assert.Equal(2, intersection.Types.Count);

            var returnType = Assert.IsType<TypeName>(intersection.Types.First());
            var constraintType = Assert.IsType<PrimitiveType>(intersection.Types.Last());
            Assert.Equal("T", returnType.Name);
            Assert.Equal(expectedConstraintKind, constraintType.Kind);
        }
        else
        {
            var returnType = Assert.IsType<TypeName>(fn.ReturnType);
            Assert.Equal("T", returnType.Name);
        }

        Assert.Equal("id", fn.Name);
        Assert.NotNull(fn.TypeParameters);
        Assert.Single(fn.TypeParameters.Parameters);

        var typeParameter = fn.TypeParameters.Parameters.First();
        Assert.Equal("T", typeParameter.Name);
        Assert.True(typeParameter.OfFunction);
        Assert.Null(typeParameter.DefaultType);
        Assert.Single(fn.Parameters);

        var parameter = fn.Parameters.First();
        Assert.Equal("value", parameter.Name);

        if (expectedConstraintKind != null)
        {
            var intersection = Assert.IsType<IntersectionType>(parameter.DeclaredType);
            Assert.Equal(2, intersection.Types.Count);

            var parameterType = Assert.IsType<TypeName>(intersection.Types.First());
            var constraintType = Assert.IsType<PrimitiveType>(intersection.Types.Last());
            Assert.Equal("T", parameterType.Name);
            Assert.Equal(expectedConstraintKind, constraintType.Kind);
        }
        else
        {
            var parameterType = Assert.IsType<TypeName>(parameter.DeclaredType);
            Assert.Equal("T", parameterType.Name);
        }

        Assert.Single(fn.Body.Statements);

        var returnStatement = Assert.IsType<Return>(fn.Body.Statements.First());
        var identifier = Assert.IsType<Identifier>(returnStatement.Expression);
        Assert.Equal("value", identifier.Name);
    }

    [Theory]
    [InlineData("fn abc -> 69")]
    [InlineData("fn abc { return 69 }")]
    public void Generates_Functions(string source)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);

        var fn = Assert.IsType<Function>(luauTree.Statements.First());
        Assert.Null(fn.ReturnType);
        Assert.Null(fn.TypeParameters);
        Assert.Empty(fn.Parameters);
        Assert.Single(fn.Body.Statements);
        Assert.Equal("abc", fn.Name);

        var returnStatement = Assert.IsType<Return>(fn.Body.Statements.First());
        var literal = Assert.IsType<NumberLiteral>(returnStatement.Expression);
        Assert.Equal(69, literal.Value);
    }

    [Fact]
    public void Generates_ConstVariables()
    {
        var luauTree = Utility.GetLuauAST("let x = 1;");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        Assert.Null(variable.DeclaredType);
        Assert.Equal("x", variable.Name);

        var literal = Assert.IsType<NumberLiteral>(variable.Initializer);
        Assert.Equal(1, literal.Value);
    }

    [Fact]
    public void Generates_LocalVariables()
    {
        var luauTree = Utility.GetLuauAST("mut x = 1;");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.Null(variable.DeclaredType);
        Assert.NotNull(variable.Initializer);
        Assert.Equal("x", variable.Name);

        var literal = Assert.IsType<NumberLiteral>(variable.Initializer);
        Assert.Equal(1, literal.Value);
    }

    [Fact]
    public void Generates_ElementAccess_Assignment_Short()
    {
        var luauTree = Utility.GetLuauAST("let x = abc[1] = 69");
        Assert.Equal(2, luauTree.Statements.Count);

        var bindingVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        var bindingValue = Assert.IsType<NumberLiteral>(bindingVariable.Initializer);
        Assert.Equal("x", bindingVariable.Name);
        Assert.Equal(69, bindingValue.Value);

        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements[1]);
        var assignment = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
        Assert.Equal("=", assignment.Operator);

        var elementAccess = Assert.IsType<ElementAccess>(assignment.Left);
        var assignmentValue = Assert.IsType<Identifier>(assignment.Right);
        var identifier = Assert.IsType<Identifier>(elementAccess.Target);
        var index = Assert.IsType<NumberLiteral>(elementAccess.Index);
        Assert.Equal("abc", identifier.Name);
        Assert.Equal(1, index.Value);
        Assert.Equal("x", assignmentValue.Name);
    }

    [Fact]
    public void Generates_ElementAccess_Assignment()
    {
        var luauTree = Utility.GetLuauAST("x[abc[1] = 69]");
        Assert.Equal(3, luauTree.Statements.Count);

        var bindingVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        var bindingValue = Assert.IsType<NumberLiteral>(bindingVariable.Initializer);
        Assert.Equal("_assigned", bindingVariable.Name);
        Assert.Equal(69, bindingValue.Value);

        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements[1]);
        var assignment = Assert.IsType<BinaryOperator>(expressionStatement.Expression);
        Assert.Equal("=", assignment.Operator);

        var elementAccess = Assert.IsType<ElementAccess>(assignment.Left);
        var assignmentValue = Assert.IsType<Identifier>(assignment.Right);
        var identifier = Assert.IsType<Identifier>(elementAccess.Target);
        var index = Assert.IsType<NumberLiteral>(elementAccess.Index);
        Assert.Equal("abc", identifier.Name);
        Assert.Equal(1, index.Value);
        Assert.Equal("_assigned", assignmentValue.Name);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements[2]);
        Assert.Equal("_", variable.Name);

        var value = Assert.IsType<ElementAccess>(variable.Initializer);
        var name = Assert.IsType<Identifier>(value.Target);
        var indexName = Assert.IsType<Identifier>(value.Index);
        Assert.Equal("x", name.Name);
        Assert.Equal("_assigned", indexName.Name);
    }

    [Fact]
    public void Generates_ElementAccess()
    {
        var luauTree = Utility.GetLuauAST("abc[1]");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var elementAccess = Assert.IsType<ElementAccess>(variable.Initializer);
        var identifier = Assert.IsType<Identifier>(elementAccess.Target);
        var index = Assert.IsType<NumberLiteral>(elementAccess.Index);
        Assert.Equal("abc", identifier.Name);
        Assert.Equal(1, index.Value);
    }

    [Fact]
    public void Generates_StringSlice_Character()
    {
        var luauTree = Utility.GetLuauAST("let s = 'abc'; s[1]", true);
        Assert.Equal(2, luauTree.Statements.Count);

        var stringVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        Assert.IsType<StringLiteral>(stringVariable.Initializer);

        var result = Assert.IsType<ExpressionStatement>(luauTree.Statements[1]);
        var call = Assert.IsType<Call>(result.Expression);
        var propertyAccess = Assert.IsType<PropertyAccess>(call.Callee);
        var target = Assert.IsType<Identifier>(propertyAccess.Target);
        Assert.Equal("string", target.Name);
        Assert.Single(propertyAccess.Names);
        Assert.Equal("sub", propertyAccess.Names[0]);
        Assert.Equal(3, call.Arguments.Count);
        Assert.IsType<Identifier>(call.Arguments[0]);

        var start = Assert.IsType<NumberLiteral>(call.Arguments[1]);
        var end = Assert.IsType<NumberLiteral>(call.Arguments[2]);
        Assert.Equal(1, start.Value);
        Assert.Equal(1, end.Value);
    }

    [Fact]
    public void Generates_RangeLiteral()
    {
        var luauTree = Utility.GetLuauAST("1..10");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Equal(2, table.Initializers.Count);

        var minInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        var maxInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[1]);
        Assert.Equal("minimum", minInit.PropertyName);
        Assert.Equal("maximum", maxInit.PropertyName);

        var minValue = Assert.IsType<NumberLiteral>(minInit.Value);
        var maxValue = Assert.IsType<NumberLiteral>(maxInit.Value);
        Assert.Equal(1, minValue.Value);
        Assert.Equal(10, maxValue.Value);
    }

    [Fact]
    public void Generates_ArraySlice_RangeLiteral()
    {
        var luauTree = Utility.GetLuauAST("let arr = [1,2,3]; arr[1..2]", true);
        Assert.Equal(3, luauTree.Statements.Count);

        var arrayVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        Assert.IsType<Table>(arrayVariable.Initializer);

        var lengthVariable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        Assert.Equal("_length", lengthVariable.Name);
        Assert.IsType<UnaryOperator>(lengthVariable.Initializer);

        var result = Assert.IsType<ExpressionStatement>(luauTree.Statements[2]);
        var call = Assert.IsType<Call>(result.Expression);
        var propertyAccess = Assert.IsType<PropertyAccess>(call.Callee);
        var target = Assert.IsType<Identifier>(propertyAccess.Target);
        Assert.Equal("table", target.Name);
        Assert.Single(propertyAccess.Names);
        Assert.Equal("move", propertyAccess.Names[0]);

        Assert.Equal(5, call.Arguments.Count);
        Assert.IsType<Identifier>(call.Arguments[0]);
        var start = Assert.IsType<Call>(call.Arguments[1]);
        var end = Assert.IsType<Call>(call.Arguments[2]);
        Assert.Equal(3, start.Arguments.Count);
        Assert.Equal(3, end.Arguments.Count);
        Assert.IsType<NumberLiteral>(start.Arguments.First());
        Assert.IsType<NumberLiteral>(end.Arguments.First());

        var startCall = Assert.IsType<PropertyAccess>(start.Callee);
        var startTarget = Assert.IsType<Identifier>(startCall.Target);
        Assert.Equal("math", startTarget.Name);
        Assert.Equal("clamp", startCall.Names[0]);
        Assert.Equal(3, start.Arguments.Count);
        Assert.IsType<NumberLiteral>(start.Arguments[0]);
        Assert.IsType<NumberLiteral>(start.Arguments[1]);
        Assert.IsType<Identifier>(start.Arguments[2]);

        Assert.IsType<NumberLiteral>(call.Arguments[3]);
        Assert.IsType<Table>(call.Arguments[4]);
    }

    [Fact]
    public void Generates_StringSlice_RangeLiteral()
    {
        var luauTree = Utility.GetLuauAST("let s = 'abc'; s[1..2]", true);
        Assert.Equal(3, luauTree.Statements.Count);

        var stringVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        Assert.IsType<StringLiteral>(stringVariable.Initializer);

        var lengthVariable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        Assert.Equal("_length", lengthVariable.Name);
        Assert.IsType<UnaryOperator>(lengthVariable.Initializer);

        var result = Assert.IsType<ExpressionStatement>(luauTree.Statements[2]);
        var call = Assert.IsType<Call>(result.Expression);
        var propertyAccess = Assert.IsType<PropertyAccess>(call.Callee);
        var target = Assert.IsType<Identifier>(propertyAccess.Target);
        Assert.Equal("string", target.Name);
        Assert.Single(propertyAccess.Names);
        Assert.Equal("sub", propertyAccess.Names[0]);

        Assert.Equal(3, call.Arguments.Count);
        Assert.IsType<Identifier>(call.Arguments[0]);
        var start = Assert.IsType<Call>(call.Arguments[1]);
        var end = Assert.IsType<Call>(call.Arguments[2]);
        Assert.Equal(3, start.Arguments.Count);
        Assert.Equal(3, end.Arguments.Count);
        Assert.IsType<NumberLiteral>(start.Arguments.First());
        Assert.IsType<NumberLiteral>(end.Arguments.First());

        var startCall = Assert.IsType<PropertyAccess>(start.Callee);
        var startTarget = Assert.IsType<Identifier>(startCall.Target);
        Assert.Equal("math", startTarget.Name);
        Assert.Equal("clamp", startCall.Names[0]);
        Assert.Equal(3, start.Arguments.Count);
        Assert.IsType<NumberLiteral>(start.Arguments[0]);
        Assert.IsType<NumberLiteral>(start.Arguments[1]);
        Assert.IsType<Identifier>(start.Arguments[2]);
    }

    [Fact]
    public void Generates_ArraySlice_RangeVariable()
    {
        var luauTree = Utility.GetLuauAST("let r = 1..5; let arr = [1,2,3,4,5]; arr[r]", true);
        Assert.Equal(4, luauTree.Statements.Count);

        var rangeVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        var arrayVariable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var lengthVariable = Assert.IsType<ConstVariable>(luauTree.Statements[2]);
        var result = Assert.IsType<ExpressionStatement>(luauTree.Statements[3]);
        Assert.IsType<Table>(rangeVariable.Initializer);
        Assert.IsType<Table>(arrayVariable.Initializer);

        var lengthOp = Assert.IsType<UnaryOperator>(lengthVariable.Initializer);
        Assert.Equal("#", lengthOp.Operator);

        var call = Assert.IsType<Call>(result.Expression);
        var propertyAccess = Assert.IsType<PropertyAccess>(call.Callee);
        var accessTarget = Assert.IsType<Identifier>(propertyAccess.Target);
        Assert.Equal("table", accessTarget.Name);
        Assert.Equal("move", propertyAccess.Names[0]);

        var start = Assert.IsType<Call>(call.Arguments[1]);
        var end = Assert.IsType<Call>(call.Arguments[2]);
        var startCallee = Assert.IsType<PropertyAccess>(start.Callee);
        var endCallee = Assert.IsType<PropertyAccess>(end.Callee);
        var startTarget = Assert.IsType<Identifier>(startCallee.Target);
        var endTarget = Assert.IsType<Identifier>(endCallee.Target);
        Assert.Equal("math", startTarget.Name);
        Assert.Equal("math", endTarget.Name);
        Assert.Equal("clamp", startCallee.Names[0]);
        Assert.Equal("clamp", endCallee.Names[0]);

        var minAccess = Assert.IsType<PropertyAccess>(start.Arguments[0]);
        var maxAccess = Assert.IsType<PropertyAccess>(end.Arguments[0]);
        var minTarget = Assert.IsType<Identifier>(minAccess.Target);
        var maxTarget = Assert.IsType<Identifier>(maxAccess.Target);
        Assert.Equal("r", minTarget.Name);
        Assert.Equal("r", maxTarget.Name);
        Assert.Equal("minimum", minAccess.Names[0]);
        Assert.Equal("maximum", maxAccess.Names[0]);
    }

    [Fact]
    public void Generates_StringSlice_RangeVariable()
    {
        var luauTree = Utility.GetLuauAST("let r = 1..5; let s = 'abcdef'; s[r]", true);
        Assert.Equal(4, luauTree.Statements.Count);

        var rangeVariable = Assert.IsType<ConstVariable>(luauTree.Statements[0]);
        var stringVariable = Assert.IsType<ConstVariable>(luauTree.Statements[1]);
        var lengthVariable = Assert.IsType<ConstVariable>(luauTree.Statements[2]);
        var result = Assert.IsType<ExpressionStatement>(luauTree.Statements[3]);
        Assert.IsType<Table>(rangeVariable.Initializer);
        Assert.IsType<StringLiteral>(stringVariable.Initializer);

        var lengthOp = Assert.IsType<UnaryOperator>(lengthVariable.Initializer);
        Assert.Equal("#", lengthOp.Operator);

        var call = Assert.IsType<Call>(result.Expression);
        var propertyAccess = Assert.IsType<PropertyAccess>(call.Callee);
        var accessTarget = Assert.IsType<Identifier>(propertyAccess.Target);
        Assert.Equal("string", accessTarget.Name);
        Assert.Equal("sub", propertyAccess.Names[0]);

        var start = Assert.IsType<Call>(call.Arguments[1]);
        var end = Assert.IsType<Call>(call.Arguments[2]);
        var startCallee = Assert.IsType<PropertyAccess>(start.Callee);
        var endCallee = Assert.IsType<PropertyAccess>(end.Callee);
        var startTarget = Assert.IsType<Identifier>(startCallee.Target);
        var endTarget = Assert.IsType<Identifier>(endCallee.Target);
        Assert.Equal("math", startTarget.Name);
        Assert.Equal("math", endTarget.Name);
        Assert.Equal("clamp", startCallee.Names[0]);
        Assert.Equal("clamp", endCallee.Names[0]);

        var minAccess = Assert.IsType<PropertyAccess>(start.Arguments[0]);
        var maxAccess = Assert.IsType<PropertyAccess>(end.Arguments[0]);
        var minTarget = Assert.IsType<Identifier>(minAccess.Target);
        var maxTarget = Assert.IsType<Identifier>(maxAccess.Target);
        Assert.Equal("r", minTarget.Name);
        Assert.Equal("r", maxTarget.Name);
        Assert.Equal("minimum", minAccess.Names[0]);
        Assert.Equal("maximum", maxAccess.Names[0]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Generates_ArrayLiterals(bool mutable)
    {
        var luauTree = Utility.GetLuauAST($"let _ = {(mutable ? "mut " : "")}[69, 420];");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Equal(2, table.Initializers.Count);
        Assert.All(table.Initializers, i => Assert.IsType<TableInitializer>(i));

        var firstElement = Assert.IsType<NumberLiteral>(table.Initializers.First().Value);
        var lastElement = Assert.IsType<NumberLiteral>(table.Initializers.Last().Value);
        Assert.Equal(69, firstElement.Value);
        Assert.Equal(420, lastElement.Value);
    }

    [Fact]
    public void Generates_Parenthesized()
    {
        var luauTree = Utility.GetLuauAST("(abc)");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var parenthesized = Assert.IsType<Parenthesized>(variable.Initializer);
        var identifier = Assert.IsType<Identifier>(parenthesized.Expression);
        Assert.Equal("abc", identifier.Name);
    }

    [Fact]
    public void Generates_TypeCasts()
    {
        var luauTree = Utility.GetLuauAST("abc as number");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var typeCast = Assert.IsType<TypeCast>(variable.Initializer);
        var identifier = Assert.IsType<Identifier>(typeCast.Expression);
        var primitive = Assert.IsType<PrimitiveType>(typeCast.Type);
        Assert.Equal("abc", identifier.Name);
        Assert.Equal("number", primitive.Render());
    }

    [Fact]
    public void Generates_Identifiers()
    {
        var luauTree = Utility.GetLuauAST("abc");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var identifier = Assert.IsType<Identifier>(variable.Initializer);
        Assert.Equal("abc", identifier.Name);
    }

    [Theory]
    [InlineData("a & b & c & d", true)]
    [InlineData("a << b << c << d", false)]
    public void Generates_ConcatenatedBitwiseArguments(string source, bool isConcatenated)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);

        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var call = Assert.IsType<Call>(expressionStatement.Expression);
        Assert.IsType<PropertyAccess>(call.Callee);

        Assert.Equal(isConcatenated ? 4 : 2, call.Arguments.Count);
    }

    [Theory]
    [InlineData("a & b", "band")]
    [InlineData("a | b", "bor")]
    [InlineData("a ~ b", "bxor")]
    [InlineData("a << b", "lshift")]
    [InlineData("a >> b", "arshift")]
    [InlineData("a >>> b", "rshift")]
    public void Generates_MappedBitwiseOperators(string source, string expectedMethod)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);

        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var call = Assert.IsType<Call>(expressionStatement.Expression);
        var access = Assert.IsType<PropertyAccess>(call.Callee);
        var bit32Identifier = Assert.IsType<Identifier>(access.Target);
        Assert.Equal("bit32", bit32Identifier.Name);
        Assert.Single(access.Names);

        var name = access.Names.First();
        Assert.Equal(expectedMethod, name);
    }

    [Fact]
    public void Generates_StringConcatenation()
    {
        var luauTree = Utility.GetLuauAST("'abc' + 'def'", typeCheck: true);
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var binary = Assert.IsType<BinaryOperator>(variable.Initializer);
        var left = Assert.IsType<StringLiteral>(binary.Left);
        var right = Assert.IsType<StringLiteral>(binary.Right);
        Assert.Equal("abc", left.Value);
        Assert.Equal("def", right.Value);
        Assert.Equal("..", binary.Operator);
    }

    [Fact]
    public void Generates_BinaryOperators()
    {
        var luauTree = Utility.GetLuauAST("1 + 2");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var binary = Assert.IsType<BinaryOperator>(variable.Initializer);
        var left = Assert.IsType<NumberLiteral>(binary.Left);
        var right = Assert.IsType<NumberLiteral>(binary.Right);
        Assert.Equal(1, left.Value);
        Assert.Equal(2, right.Value);
        Assert.Equal("+", binary.Operator);
    }

    [Fact]
    public void Generates_UnaryOperators()
    {
        var luauTree = Utility.GetLuauAST("!false");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var unary = Assert.IsType<UnaryOperator>(variable.Initializer);
        Assert.IsType<BooleanLiteral>(unary.Operand);
        Assert.Equal("not ", unary.Operator);
    }

    [Fact]
    public void Generates_NameOf()
    {
        var luauTree = Utility.GetLuauAST("let x = 1; nameof(x)");
        Assert.Equal(2, luauTree.Statements.Count);

        Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.Last());
        var literal = Assert.IsType<StringLiteral>(variable.Initializer);
        Assert.Equal("x", literal.Value);
    }

    [Theory]
    [InlineData("420", 420)]
    [InlineData("69.420", 69.42)]
    [InlineData(".5", 0.5)]
    public void Generates_NumberLiterals(string source, double expected)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var literal = Assert.IsType<NumberLiteral>(variable.Initializer);
        Assert.Equal(expected, literal.Value);
    }

    [Theory]
    [InlineData("'abc'", "abc")]
    [InlineData("\"def\"", "def")]
    public void Generates_StringLiterals(string source, string expected)
    {
        var luauTree = Utility.GetLuauAST(source);
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var literal = Assert.IsType<StringLiteral>(variable.Initializer);
        Assert.Equal(expected, literal.Value);
    }

    [Fact]
    public void Generates_BoolLiterals()
    {
        var luauTree = Utility.GetLuauAST("true");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var literal = Assert.IsType<BooleanLiteral>(variable.Initializer);
        Assert.True(literal.Value);
    }

    [Fact]
    public void Generates_NilLiterals()
    {
        var luauTree = Utility.GetLuauAST("none");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        Assert.IsType<NilLiteral>(variable.Initializer);
    }
}