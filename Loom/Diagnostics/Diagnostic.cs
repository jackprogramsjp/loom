using Loom.Syntax;
using Loom.Utility;

namespace Loom.Diagnostics;

public class Diagnostic(LocationSpan span, DiagnosticSeverity severity, string? code, string message)
{
    public LocationSpan Span { get; } = span;
    public DiagnosticSeverity Severity { get; } = severity;
    public string? Code { get; } = code;
    public string Message { get; } = message;

    public override string ToString()
    {
        var (severityColor, severityLabel) = Severity switch
        {
            DiagnosticSeverity.Error => (Colors.Red, "error"),
            DiagnosticSeverity.Warn => (Colors.Yellow, "warning"),
            DiagnosticSeverity.Info => (Colors.Cyan, "info"),
            _ => (Colors.White, "unknown")
        };

        var codePart = string.IsNullOrEmpty(Code) ? "" : $" {Colors.Dim}({Code}){Colors.Reset}{severityColor}";
        var header = $"{severityColor}{Colors.Bold}{severityLabel}{Colors.Reset}{severityColor}{codePart}:{Colors.Reset}{Colors.Dim} {Message}{Colors.Reset}";
        var location = $"{Colors.Dim}  ┌─{Colors.Reset} {Span.Start}";
        var sourceLines = Span.File.SourceText.Split(Environment.NewLine);
        var lineNumber = Span.Start.Line;
        var lineIndex = lineNumber - 1;
        var lineDigits = lineNumber.ToString().Length;
        var indent = new string(' ', lineDigits + 1);
        var gutter = $"{Colors.Dim}{indent}│{Colors.Reset}";
        var aboveLine = lineIndex > 0
            ? $"{Colors.Dim}{sourceLines[lineIndex - 1]}{Colors.Reset}"
            : "";

        var errorLine = sourceLines[lineIndex];
        var arrowLength = Math.Max(Span.End.Character - Span.Start.Character, 1);
        var padding = new string(' ', Span.Start.Character);
        var arrows = new string('^', arrowLength);
        var underline = $"{severityColor}{padding}{arrows}{Colors.Reset}";
        var belowLine = lineIndex + 1 < sourceLines.Length
            ? $"{Colors.Dim}{sourceLines[lineIndex + 1]}{Colors.Reset}"
            : "";

        List<string> lines =
        [
            header,
            location,
            $"{gutter}   {aboveLine}",
            $"{Colors.Bold}{lineNumber} │{Colors.Reset}   {errorLine}",
            $"{gutter}   {underline}",
            $"{gutter}   {belowLine}"
        ];

        return string.Join(Environment.NewLine, lines);
    }
}