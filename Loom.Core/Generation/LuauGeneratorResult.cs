using Loom.Diagnostics;
using Loom.Luau.AST;

namespace Loom.Generation;

public sealed record LuauGeneratorResult(LuauTree LuauTree, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);