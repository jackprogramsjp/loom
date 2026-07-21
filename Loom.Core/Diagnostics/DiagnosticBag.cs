using Loom.Core.Parsing.AST;
using Loom.Core.Text;

namespace Loom.Core.Diagnostics;

public sealed class DiagnosticBag(HashSet<Diagnostic>? diagnostics = null)
{
    public static bool FailFast { get; set; } = true;
    public static DiagnosticBag Concat(List<DiagnosticBag> bags) => new(bags.SelectMany(bag => bag.Set).ToHashSet());
    
    public HashSet<Diagnostic> Set { get; } = diagnostics ?? [];
    
    public void Debug(Node node, string message) => Debug(node.Span, message);
    public void Debug(LocationSpan span, string message) => Report(span, DiagnosticSeverity.Debug, null, message, null);
    public void Debug(Node node, string code, string message) => Debug(node.Span, code, message);
    public void Debug(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Debug, code, message, null);
    public void Info(Node node, string message) => Info(node.Span, message);
    public void Info(LocationSpan span, string message) => Report(span, DiagnosticSeverity.Info, null, message, null);
    public void Info(Node node, string code, string message) => Info(node.Span, code, message);
    public void Info(LocationSpan span, string code, string message) => Report(span, DiagnosticSeverity.Info, code, message, null);
    public void Warn(Node node, string code, string message, string? hint = null) => Warn(node.Span, code, message, hint);
    public void Warn(Token token, string code, string message, string? hint = null) => Warn(token.GetLocation(), code, message, hint);
    public void Warn(LocationSpan span, string code, string message, string? hint = null) => Report(span, DiagnosticSeverity.Warn, code, message, hint);
    public void Error(Node node, string code, string message, string? hint = null) => Error(node.Span, code, message, hint);
    public void Error(Token token, string code, string message, string? hint = null) => Error(token.GetLocation(), code, message, hint);
    public void Error(LocationSpan span, string code, string message, string? hint = null) => Report(span, DiagnosticSeverity.Error, code, message, hint);
    public void NotImplemented(Node node, string? feature = null, string? hint = null) => NotImplemented(node.Span, feature, hint);
    public void NotImplemented(Token token, string? feature = null, string? hint = null) => NotImplemented(token.GetLocation(), feature, hint);

    public void NotImplemented(LocationSpan span, string? feature = null, string? hint = null) =>
        Error(span, InternalCodes.NotImplemented, feature ?? "This feature is not yet implemented.", hint);

    public object? CompilerError(Node node, string message) => CompilerError(node.Span, message);
    public object? CompilerError(SourceFile file, string message) => CompilerError(LocationSpan.Empty(file), message);

    public object? CompilerError(LocationSpan span, string message)
    {
        Error(span, InternalCodes.CompilerError, message, "this is a compiler bug! please report an issue.");
        return null;
    }

    public Diagnostic? Find(Func<Diagnostic, bool> predicate) => Set.FirstOrDefault(predicate);
    public DiagnosticBag WithoutInfo() => new(Set.Where(d => d.Severity > DiagnosticSeverity.Info).ToHashSet());
    public DiagnosticBag Errors() => new(Set.Where(d => d.Severity == DiagnosticSeverity.Error).ToHashSet());
    public bool ContainsErrors() => Errors().Set.Count > 0;

    public override string ToString() => string.Join('\n', Set);

    internal void Report(LocationSpan span, DiagnosticSeverity severity, string? code, string message, string? hint) =>
        Report(new Diagnostic(span, severity, code, message, hint));

    private void Report(Diagnostic diagnostic)
    {
        Set.Add(diagnostic);
        if (!FailFast || diagnostic.Severity < DiagnosticSeverity.Error) return;

        var code = (diagnostic.Code ?? InternalCodes.Unknown).GetHashCode();
        Console.WriteLine(diagnostic.ToString());
        Environment.Exit(code);
    }
}