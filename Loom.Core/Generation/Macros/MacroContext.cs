using Loom.Luau.AST;
using Loom.Resolving;

namespace Loom.Generation.Macros;

internal record MacroContext(SemanticModel SemanticModel, LuauState State)
{
    public static LuauExpression GetCallObject(Call call) =>
        call.Callee switch
        {
            ElementAccess elementAccess => UnwrapParentheses(elementAccess.Target),
            PropertyAccess propertyAccess =>
                propertyAccess.Names.Count > 1
                    ? new PropertyAccess(UnwrapParentheses(propertyAccess.Target), propertyAccess.Names.SkipLast(1).ToList())
                    : UnwrapParentheses(propertyAccess.Target),
            
            var callee => UnwrapParentheses(callee)
        };

    private static LuauExpression UnwrapParentheses(LuauExpression expression)
    {
        while (true)
        {
            if (expression is not Parenthesized parenthesized)
                return expression;

            expression = parenthesized.Expression;
        }
    }

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
}