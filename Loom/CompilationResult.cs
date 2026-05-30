using Loom.Diagnostics;

namespace Loom;

public class CompilationResult(List<CompiledFile> files, DiagnosticBag diagnostics) : DiagnosedResult(diagnostics)
{
    public List<CompiledFile> Files { get; } = files;
}