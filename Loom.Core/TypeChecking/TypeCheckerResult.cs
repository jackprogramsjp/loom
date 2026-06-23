using Loom.Diagnostics;
using Loom.SemanticAnalysis;

namespace Loom.TypeChecking;

public sealed class TypeCheckerResult(Types.Type returnType, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public Types.Type ReturnType { get; } = returnType;
}