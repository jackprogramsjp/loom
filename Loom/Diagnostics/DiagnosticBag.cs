using Loom.Syntax;

namespace Loom.Diagnostics;

public class DiagnosticBag
{
    private readonly HashSet<Diagnostic> _diagnostics = [];
    private DiagnosticSeverity? _filterSeverity;

    public void Info(LocationSpan span, string message) => Report(span, DiagnosticSeverity.Info, null, message);
    public void Info(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Info, code, message);
    public void Warn(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Warn, code, message);
    public void Error(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Error, code, message);
    public void SetSeverityFilter(DiagnosticSeverity filter) => _filterSeverity = filter;
    public override string ToString() => string.Join('\n', Filtered());

    private HashSet<Diagnostic> Filtered() =>
        _filterSeverity == null
            ? _diagnostics
            : _diagnostics.Where(d => d.Severity == _filterSeverity).ToHashSet();

    private void Report(LocationSpan span, DiagnosticSeverity severity, string? code, string message) => Report(new Diagnostic(span, severity, code, message));
    private void Report(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);
}