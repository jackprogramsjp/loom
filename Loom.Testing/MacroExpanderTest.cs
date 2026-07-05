using Loom.Luau.AST;

namespace Loom.Testing;

[Collection("Assembly")]
public class MacroExpanderTest
{
    [Fact]
    public void Generates_ResultStatic_Ok()
    {
        const string source = "Result.ok(69)";
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Equal(2, table.Initializers.Count);

        var kindInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        var valueInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[1]);
        Assert.Equal("kind", kindInit.PropertyName);
        Assert.Equal("value", valueInit.PropertyName);

        var kindValue = Assert.IsType<NumberLiteral>(kindInit.Value);
        var value = Assert.IsType<NumberLiteral>(valueInit.Value);
        Assert.Equal(0, kindValue.Value);
        Assert.Equal(69, value.Value);
    }
    
    [Fact]
    public void Generates_ResultStatic_Err()
    {
        const string source = "Result.err('stupid program')";
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var table = Assert.IsType<Table>(variable.Initializer);
        Assert.Equal(2, table.Initializers.Count);

        var kindInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[0]);
        var errorInit = Assert.IsType<PropertyTableInitializer>(table.Initializers[1]);
        Assert.Equal("kind", kindInit.PropertyName);
        Assert.Equal("error", errorInit.PropertyName);

        var kindValue = Assert.IsType<NumberLiteral>(kindInit.Value);
        var errorValue = Assert.IsType<StringLiteral>(errorInit.Value);
        Assert.Equal(1, kindValue.Value);
        Assert.Equal("stupid program", errorValue.Value);
    }

    [Theory]
    [InlineData("let a = [1, 2, 3]; a.length")]
    [InlineData("let a = [1, 2, 3]; a['length']")]
    [InlineData("let a = [1, 2, 3]; let _ = (a).length")]
    public void Generates_Array_Length(string source)
    {
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Equal(2, luauTree.Statements.Count);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.Last());
        var unaryOperator = Assert.IsType<UnaryOperator>(variable.Initializer);
        Assert.Equal("#", unaryOperator.Operator);
    }

    [Theory]
    [InlineData("(1..10).length")]
    [InlineData("(1..10)['length']")]
    public void Generates_Range_Length_Literal(string source)
    {
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Single(luauTree.Statements);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.First());
        var value = Assert.IsType<NumberLiteral>(variable.Initializer);
        Assert.Equal(10d, value.Value);
    }
    
    [Fact]
    public void Generates_Range_Clamp()
    {
        const string source = "let r = 1..10; r.clamp(69)";
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Equal(2, luauTree.Statements.Count);

        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.Last());
        var clampCall = Assert.IsType<Call>(expressionStatement.Expression);
        var value = Assert.IsType<NumberLiteral>(clampCall.Arguments[0]);
        var minimum = Assert.IsType<PropertyAccess>(clampCall.Arguments[1]);
        var maximum = Assert.IsType<PropertyAccess>(clampCall.Arguments[2]);
        var clamp = Assert.IsType<PropertyAccess>(clampCall.Callee);
        var mathIdentifier = Assert.IsType<Identifier>(clamp.Target);
        Assert.Equal(3, clampCall.Arguments.Count);
        Assert.Single(clamp.Names);
        Assert.Equal("math", mathIdentifier.Name);
        Assert.Equal("clamp", clamp.Names.First());
        Assert.Equal(69d, value.Value);
        Assert.Single(minimum.Names);
        Assert.Single(maximum.Names);

        var rangeIdentifier = Assert.IsType<Identifier>(minimum.Target);
        var rangeIdentifier2 = Assert.IsType<Identifier>(maximum.Target);
        Assert.Equal("r", rangeIdentifier.Name);
        Assert.Equal("r", rangeIdentifier2.Name);
        Assert.Equal("minimum", minimum.Names.First());
        Assert.Equal("maximum", maximum.Names.First());
    }

    [Fact]
    public void Generates_Range_Clamp_Literal()
    {
        const string source = "(1..10).clamp(69)";
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Single(luauTree.Statements);

        var expressionStatement = Assert.IsType<ExpressionStatement>(luauTree.Statements.First());
        var value = Assert.IsType<NumberLiteral>(expressionStatement.Expression);
        Assert.Equal(10d, value.Value);
    }

    [Fact]
    public void Generates_Range_Length()
    {
        const string source = "let r = 1..10; r.length";
        var luauTree = Utility.GetLuauAST(source, true);
        Utility.AssertNoErrors(Utility.GetGeneratorDiagnostics(source, true));
        Assert.Equal(2, luauTree.Statements.Count);

        var variable = Assert.IsType<ConstVariable>(luauTree.Statements.Last());
        var binaryOperator = Assert.IsType<BinaryOperator>(variable.Initializer);
        var one = Assert.IsType<NumberLiteral>(binaryOperator.Left);
        var absCall = Assert.IsType<Call>(binaryOperator.Right);
        Assert.Single(absCall.Arguments);
        Assert.Equal("+", binaryOperator.Operator);

        var subtractionBinary = Assert.IsType<BinaryOperator>(absCall.Arguments.First());
        var maximumAccess = Assert.IsType<PropertyAccess>(subtractionBinary.Left);
        var minimumAccess = Assert.IsType<PropertyAccess>(subtractionBinary.Right);
        var rangeIdentifier = Assert.IsType<Identifier>(maximumAccess.Target);
        var rangeIdentifier2 = Assert.IsType<Identifier>(minimumAccess.Target);
        Assert.Equal("-", subtractionBinary.Operator);
        Assert.Equal("r", rangeIdentifier.Name);
        Assert.Equal("r", rangeIdentifier2.Name);
        Assert.Single(maximumAccess.Names);
        Assert.Single(minimumAccess.Names);
        Assert.Equal("maximum", maximumAccess.Names.First());
        Assert.Equal("minimum", minimumAccess.Names.First());
        
        var abs = Assert.IsType<PropertyAccess>(absCall.Callee);
        var mathIdentifier = Assert.IsType<Identifier>(abs.Target);
        Assert.Single(abs.Names);
        Assert.Equal("math", mathIdentifier.Name);
        Assert.Equal("abs", abs.Names.First());
        Assert.Equal(1d, one.Value);
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
}