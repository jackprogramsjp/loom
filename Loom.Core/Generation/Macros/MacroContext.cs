using Loom.Luau.AST;
using Loom.SemanticAnalysis;

namespace Loom.Generation.Macros;

internal record MacroContext(SemanticModel SemanticModel, LuauState State)
{
    public static LuauExpression GetCallObject(Call call) =>
        call.Callee switch
        {
            PropertyAccess propertyAccess => propertyAccess.Target,
            ElementAccess elementAccess => elementAccess.Target,
            var callee => callee
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
}