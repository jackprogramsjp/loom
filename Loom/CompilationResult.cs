using Loom.Diagnostics;

namespace Loom;

public sealed class CompilationResult(List<CompiledFile> files, DiagnosticBag diagnostics) : DiagnosedResult(diagnostics)
{
    internal static CompilationResult Empty { get; } = new([], new DiagnosticBag());
    
    public List<CompiledFile> Files { get; } = files;
}