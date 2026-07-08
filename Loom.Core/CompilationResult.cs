using Loom.Diagnostics;

namespace Loom;

public sealed record CompilationResult(List<CompiledFile> Files, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);