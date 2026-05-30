using Loom.Diagnostics;
using Loom.Luau;
using Loom.Luau.AST;

namespace Loom;

public class LuauGeneratorResult(LuauTree luauTree, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public LuauTree LuauTree { get; } = luauTree;
}