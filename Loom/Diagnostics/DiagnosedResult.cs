using Loom.Syntax;

namespace Loom.Diagnostics;

public class DiagnosedResult(DiagnosticBag diagnostics)
{
    public DiagnosticBag Diagnostics { get; } = diagnostics;
}