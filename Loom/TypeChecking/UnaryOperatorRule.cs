using Loom.Syntax;

namespace Loom.TypeChecking;

public class UnaryOperatorRule(SyntaxKind operatorKind, Types.Type operandType, Types.Type? returnType = null)
{
    public SyntaxKind OperatorKind { get; } = operatorKind;
    public Types.Type OperandType { get; } = operandType;
    public Types.Type ReturnType { get; } = returnType ?? operandType;
}