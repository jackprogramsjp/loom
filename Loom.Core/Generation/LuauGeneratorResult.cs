using Loom.Core.Diagnostics;
using Loom.Luau.AST;

namespace Loom.Core.Generation;

public sealed record LuauGeneratorResult(LuauTree LuauTree, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);