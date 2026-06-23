using Loom.Syntax;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

internal sealed record BinaryOperatorRule(SyntaxKind OperatorKind, Type LeftType, Type? RightType = null, Type? ReturnType = null)
{
    public Type RightType { get; } = RightType ?? LeftType;
    public Type ReturnType { get; } = ReturnType ?? RightType ?? LeftType;
}