using Loom.Core.Diagnostics;
using Loom.Core.Resolving;
using Loom.Luau;
using Loom.Luau.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using ElementAccess = Loom.Luau.AST.ElementAccess;
using PropertyAccess = Loom.Luau.AST.PropertyAccess;
using UnaryOperator = Loom.Luau.AST.UnaryOperator;

namespace Loom.Core.Generation.Macros;

internal record MacroContext(SemanticModel SemanticModel, LuauState State, DiagnosticBag Diagnostics)
{
    public Parsing.AST.Node Node { get; set; } = null!;

    public static LuauExpression GetCallObject(Call call) =>
        call.Callee switch
        {
            ElementAccess elementAccess => LuauFactory.UnwrapParentheses(elementAccess.Target),
            PropertyAccess propertyAccess =>
                propertyAccess.Names.Count > 1
                    ? new PropertyAccess(LuauFactory.UnwrapParentheses(propertyAccess.Target), propertyAccess.Names.SkipLast(1).ToList())
                    : LuauFactory.UnwrapParentheses(propertyAccess.Target),

            var callee => LuauFactory.UnwrapParentheses(callee)
        };

    public static bool TryComputeConstantArithmetic(LuauExpression expression, out double computed)
    {
        computed = -1;
        switch (expression)
        {
            case NumberLiteral literal:
                computed = literal.Value;
                return true;

            case UnaryOperator { Operator: "-" } unary:
            {
                if (!TryComputeConstantArithmetic(unary.Operand, out var operand))
                    return false;

                computed = -operand;
                return true;
            }
            case BinaryOperator { Operator: "+" or "-" or "*" or "/" or "//" or "^" or "%" } binary:
            {
                if (!TryComputeConstantArithmetic(binary.Left, out var left))
                    return false;

                if (!TryComputeConstantArithmetic(binary.Right, out var right))
                    return false;

                computed = binary.Operator switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    "//" => Math.Floor(left / right),
                    "^" => Math.Pow(left, right),
                    "%" => left % right,
                    _ => -1
                };

                return true;
            }
        }

        return false;
    }

    public Call TypeArgumentAsStringCall(string name, string newName, Parsing.AST.TypeArguments? typeArguments, LuauExpression instance, bool isMethod = true)
    {
        var instanceName = GetStringFromOnlyTypeArgument(typeArguments, name);
        return new Call(new PropertyAccess(instance, [newName]), [instanceName], isMethod);
    }

    public StringLiteral GetStringFromOnlyTypeArgument(Parsing.AST.TypeArguments? typeArguments, string fnName)
    {
        var typeName = typeArguments!.ArgumentsList[0];
        var instanceType = SemanticModel.GetType(typeName);
        if (instanceType is TypeChecking.Types.TypeParameter)
            Diagnostics.Error(
                typeName,
                InternalCodes.AbstractTypeParameterInMacro,
                $"Cannot use type parameter '{typeName}' with '{fnName}::<T>()' macro."
            );

        return new StringLiteral(typeName.ToString());
    }
}