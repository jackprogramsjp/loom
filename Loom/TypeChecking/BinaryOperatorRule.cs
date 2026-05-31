using Loom.Syntax;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public class BinaryOperatorRule(SyntaxKind operatorKind, Type leftType, Type? rightType = null, Type? returnType = null)
{
    public SyntaxKind OperatorKind { get; } = operatorKind;
    public Type LeftType { get; } = leftType;
    public Type RightType { get; } = rightType ?? leftType;
    public Type ReturnType { get; } = returnType ?? rightType ?? leftType;
}