using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Diagnostics;

public class DiagnosticBag(HashSet<Diagnostic>? diagnostics = null)
{
    public HashSet<DiagnosticSeverity> ImmediatelyReportSeverities { get; } = [];
    public HashSet<Diagnostic> Set { get; } = diagnostics ?? [];

    public static DiagnosticBag Concat(List<DiagnosticBag> bags) => new(bags.SelectMany(bag => bag.Set).ToHashSet());

    public void Info(LocationSpan span, string message) => Report(span, DiagnosticSeverity.Info, null, message, null);
    public void Info(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Info, code, message, null);
    public void Warn(LocationSpan span, string code, string message, string? hint = null) => Report(span, DiagnosticSeverity.Warn, code, message, hint);
    public void Error(Node node, string code, string message, string? hint = null) => Error(node.Span, code, message, hint);
    public void Error(LocationSpan span, string code, string message, string? hint = null) => Report(span, DiagnosticSeverity.Error, code, message, hint);
    public void NotImplemented(Node node, string? feature = null) => NotImplemented(node.Span, feature);
    public void NotImplemented(Token token, string? feature = null) => NotImplemented(token.Span, feature);

    public void NotImplemented(LocationSpan span, string? feature = null) =>
        Error(span, InternalCodes.NotImplemented, feature ?? "This feature is not yet implemented.");

    public void CompilerError(LocationSpan span, string message) => Error(span, InternalCodes.CompilerError, message);

    public Diagnostic? Find(Func<Diagnostic, bool> predicate) => Set.FirstOrDefault(predicate);
    public DiagnosticBag NotInfo() => new(Set.Where(d => d.Severity != DiagnosticSeverity.Info).ToHashSet());
    public DiagnosticBag Errors() => new(Set.Where(d => d.Severity == DiagnosticSeverity.Error).ToHashSet());

    public override string ToString() => string.Join('\n', Set);

    private void Report(LocationSpan span, DiagnosticSeverity severity, string? code, string message, string? hint) =>
        Report(new Diagnostic(span, severity, code, message, hint));

    private void Report(Diagnostic diagnostic)
    {
        Set.Add(diagnostic);
        if (diagnostic.Severity != DiagnosticSeverity.Error) return;

        var code = (diagnostic.Code ?? InternalCodes.Unknown).GetHashCode();
        Console.WriteLine(diagnostic.ToString());
        Environment.Exit(code);
    }
}