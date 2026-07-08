using Loom.Diagnostics;

namespace Loom.TypeChecking;

public sealed record TypeCheckerResult(Types.Type ReturnType, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);