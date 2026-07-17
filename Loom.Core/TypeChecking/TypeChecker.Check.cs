using Loom.Core.FlowAnalysis;
using Loom.Core.Parsing.AST;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.TypeChecking;

public sealed partial class TypeChecker
{
    private Type Check(Expression expression, Type expected) => Check(expression, expected, _flowState);

    private Type Check(Expression expression, Type expected, FlowState state)
    {
        if (expression is ArrayLiteral arrayLiteral && expected is Types.ArrayType arrayType)
            return CheckArrayLiteral(arrayLiteral, arrayType, state);

        // Once more specific rules, add more, but for now it'll just be like that.
        return CheckSubsumption(expression, expected, state);
    }

    private Type CheckSubsumption(Expression expression, Type expected, FlowState state)
    {
        var actual = Visit(expression, state);
        if (actual.IsAssignableTo(expected))
            return actual;

        _semanticModel.TypeSolver.AddConstraint(actual, expected, expression);
        return actual;
    }

    private Type CheckArrayLiteral(ArrayLiteral arrayLiteral, Types.ArrayType expected, FlowState state)
    {
        foreach (var element in arrayLiteral.Expressions)
            Check(element, expected.ElementType, state);

        return BindType(arrayLiteral, expected);
    }
}