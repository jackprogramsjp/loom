using Loom.Core.Text;

namespace Loom.Core.TypeChecking;

internal sealed record UnaryOperatorRule(SyntaxKind OperatorKind, Types.Type OperandType, Types.Type? ReturnType = null)
{
    public Types.Type ReturnType { get; } = ReturnType ?? OperandType;
}