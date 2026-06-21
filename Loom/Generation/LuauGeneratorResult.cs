using Loom.Diagnostics;
using Loom.Luau.AST;

namespace Loom.Generation;

public sealed class LuauGeneratorResult(LuauTree luauTree, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public LuauTree LuauTree { get; } = luauTree;
}