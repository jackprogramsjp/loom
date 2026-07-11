using Loom.Core.Diagnostics;

namespace Loom.Core;

public sealed record CompilationResult(List<CompiledFile> Files, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);