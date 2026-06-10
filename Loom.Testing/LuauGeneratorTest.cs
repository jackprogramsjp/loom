using Loom.Luau.AST;
using Loom.Parsing.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using ElementAccess = Loom.Luau.AST.ElementAccess;
using ExpressionStatement = Loom.Luau.AST.ExpressionStatement;
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
    public void Generates_Array_TableType()
    {
        var luauTree = Utility.GetLuauAST("mut x: number[];");
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<LocalVariable>(luauTree.Statements.First());
        Assert.NotNull(variable.DeclaredType);

        var table = Assert.IsType<TableType>(variable.DeclaredType);
        Assert.Null(table.KeyType);

        var inner = Assert.IsType<PrimitiveType>(table.ValueType);
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
        Assert.Single(fn.Statements);
        Assert.Equal("abc", fn.Name);

        var returnStatement = Assert.IsType<Return>(fn.Statements.First());
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

        Assert.Single(fn.Statements);

        var returnStatement = Assert.IsType<Return>(fn.Statements.First());
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
        Assert.Single(fn.Statements);
        Assert.Equal("abc", fn.Name);

        var returnStatement = Assert.IsType<Return>(fn.Statements.First());
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