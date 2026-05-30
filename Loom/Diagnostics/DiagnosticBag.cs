using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Diagnostics;

public class DiagnosticBag(HashSet<Diagnostic>? diagnostics = null)
{
    public HashSet<Diagnostic> Set { get; } = diagnostics ?? [];
    
    public static DiagnosticBag Concat(List<DiagnosticBag> bags) => new(bags.SelectMany(bag => bag.Set).ToHashSet());
    
    public void Info(LocationSpan span, string message) => Report(span, DiagnosticSeverity.Info, null, message);
    public void Info(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Info, code, message);
    public void Warn(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Warn, code, message);
    public void Error(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Error, code, message);
    public void NotImplemented(Node node) => NotImplemented(node.Span);
    public void NotImplemented(Token token) => NotImplemented(token.Span);
    public void NotImplemented(LocationSpan span) => Error(span, InternalCodes.NotImplemented, "This feature is not yet implemented.");
    public void CompilerError(LocationSpan span, string message) => Error(span, InternalCodes.CompilerError, message);

    public Diagnostic? Find(Func<Diagnostic, bool> predicate) => Set.FirstOrDefault(predicate);
    public DiagnosticBag NotInfo() => new(Set.Where(d => d.Severity != DiagnosticSeverity.Info).ToHashSet());
    public DiagnosticBag Errors() => new(Set.Where(d => d.Severity == DiagnosticSeverity.Error).ToHashSet());
    
    public override string ToString() => string.Join('\n', Set);
    
    private void Report(LocationSpan span, DiagnosticSeverity severity, string? code, string message) => Report(new Diagnostic(span, severity, code, message));
    private void Report(Diagnostic diagnostic) => Set.Add(diagnostic);
}