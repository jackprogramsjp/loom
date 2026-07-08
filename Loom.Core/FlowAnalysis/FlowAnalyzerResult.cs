using Loom.Diagnostics;

namespace Loom.FlowAnalysis;

public sealed record FlowAnalyzerResult(FlowState FlowState, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);