using Loom.Diagnostics;
using Loom.Luau;

namespace Loom;

public class LuauGeneratorResult(LuauTree luauTree, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public LuauTree LuauTree { get; } = luauTree;
}