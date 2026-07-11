using Loom.Core.Diagnostics;

namespace Loom.Core.FlowAnalysis;

public sealed record FlowAnalyzerResult(FlowState FlowState, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);