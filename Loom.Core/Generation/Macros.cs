using System.Diagnostics.CodeAnalysis;
using Loom.Debug;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using ElementAccess = Loom.Parsing.AST.ElementAccess;
using FunctionType = Loom.TypeChecking.Types.FunctionType;
using PropertyAccess = Loom.Parsing.AST.PropertyAccess;

namespace Loom.Generation;

public sealed class Macros(SemanticModel semanticModel)
{
    public bool TryGetInvocationMacro(Invocation invocation, Call luauCall, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        expression = null;
        if (invocation.Expression is QualifiedName or PropertyAccess or ElementAccess)
        {
            var objectExpression = invocation.Expression.Children.First();
            if (semanticModel.GetType(objectExpression) is InterfaceType { Name: "ResultStatic" })
            {
                var calleeType = semanticModel.GetType(invocation.Expression);
                if (calleeType is not FunctionType functionType)
                    return false;

                expression = CreateResultConstructor(luauCall, functionType.TypeParameters.Single().Name == "T" ? "Ok" : "Error");
                return true;
            }
        }

        return false;
    }

    private static Table CreateResultConstructor(Call call, string kind) =>
        new(
            [
                new PropertyTableInitializer("kind", new NumberLiteral(kind == "Ok" ? 0 : 1)),
                new PropertyTableInitializer(kind == "Ok" ? "value" : "error", call.Arguments.Single()),
            ]
        );
}