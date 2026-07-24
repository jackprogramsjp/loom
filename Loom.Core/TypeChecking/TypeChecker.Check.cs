using System.Diagnostics.CodeAnalysis;
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
        if (TryInstantiateGenericFunctionArgument(expression, actual, expected, out var instantiated))
            actual = instantiated;

        if (actual.IsAssignableTo(expected))
            return actual;

        _semanticModel.TypeSolver.AddConstraint(actual, expected, expression);
        return actual;
    }

    /// <summary>
    /// A generic-valued argument (e.g. passing `id` where `id&lt;T&gt;(n: T): T`) is otherwise
    /// compared structurally against its expected type with no attempt to specialize it first,
    /// so a type-parameter-count mismatch (the argument has its own free type parameters, the
    /// expected shape has none) fails immediately even when the expected shape fully determines
    /// what the argument's type parameters should be. Infer and substitute them here so `id`
    /// becomes e.g. `fn(number): number` before the normal assignability/unification check runs.
    /// </summary>
    private bool TryInstantiateGenericFunctionArgument(Node failNode, Type actual, Type expected, [NotNullWhen(true)] out Type? instantiated)
    {
        instantiated = null;
        if (actual is not Types.FunctionType { TypeParameters.Count: > 0 } genericFunction
            || expected is not Types.FunctionType expectedFunction
            || genericFunction.TypeParameters.Count == expectedFunction.TypeParameters.Count)
        {
            return false;
        }

        var substitution = TypeInferrer.InferFunctionTypeArguments(genericFunction, expectedFunction.ParameterTypes);
        foreach (var typeParameter in genericFunction.TypeParameters)
        {
            if (substitution.TryGetValue(typeParameter, out var substitutedType)
                && typeParameter.Constraint != null
                && !substitutedType.IsAssignableTo(typeParameter.Constraint))
            {
                return false;
            }
        }

        var substitutedParameterTypes = SubstituteTypeParameters(failNode, genericFunction.ParameterTypes, substitution);
        var substitutedReturnType = SubstituteTypeParameters(failNode, genericFunction.ReturnType, substitution);
        instantiated = new Types.FunctionType([], substitutedParameterTypes, substitutedReturnType);
        return true;
    }

    private Types.ArrayType CheckArrayLiteral(ArrayLiteral arrayLiteral, Types.ArrayType expected, FlowState state)
    {
        foreach (var element in arrayLiteral.Expressions)
            Check(element, expected.ElementType, state);

        return BindType(arrayLiteral, expected);
    }
}