using Loom.Diagnostics;

namespace Loom.TypeChecking;

public class TypeCheckerResult(Types.Type returnType, DiagnosticBag diagnostics) : DiagnosedResult(diagnostics)
{
    public Types.Type ReturnType { get; } = returnType;
}