using Loom.Parsing.AST;
using Loom.Syntax;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

internal static class UnaryOperatorBinder
{
    private static readonly HashSet<UnaryOperatorRule> _rules =
    [
        new(SyntaxKind.Bang, PrimitiveType.Bool), new(SyntaxKind.Tilde, PrimitiveType.Number), new(SyntaxKind.Minus, PrimitiveType.Number)
    ];

    public static UnaryOperatorRule? GetRule(UnaryOperator unaryOperator, Type operandType) =>
        _rules.FirstOrDefault(rule => rule.OperatorKind == unaryOperator.Operator.Kind
            && Type.IsNotNever(operandType)
            && operandType.IsAssignableTo(rule.OperandType)
        );

    public static UnaryOperatorRule? GetSuggestion(UnaryOperator unaryOperator, Type operandType)
    {
        var sameOp = _rules.FirstOrDefault(r => r.OperatorKind == unaryOperator.Operator.Kind
            && Type.IsNotNever(operandType)
            && !operandType.IsAssignableTo(r.OperandType)
        );

        var differentOp = _rules.FirstOrDefault(r => r.OperatorKind != unaryOperator.Operator.Kind
            && Type.IsNotNever(operandType)
            && operandType.IsAssignableTo(r.OperandType)
        );

        return differentOp ?? sameOp;
    }
}