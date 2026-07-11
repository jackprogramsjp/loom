using Loom.Core.Diagnostics;

namespace Loom.Core.TypeChecking;

public sealed record TypeCheckerResult(Types.Type ReturnType, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);