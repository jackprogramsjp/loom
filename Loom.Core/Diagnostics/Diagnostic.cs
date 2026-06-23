using Loom.Text;
using Loom.Utility;

namespace Loom.Diagnostics;

public sealed record Diagnostic(LocationSpan Span, DiagnosticSeverity Severity, string? Code, string Message, string? Hint)
{
    public override string ToString()
    {
        var (severityColor, underlineColor, severityLabel) = Severity switch
        {
            DiagnosticSeverity.Error => (Colors.Red, Colors.Magenta, "error"),
            DiagnosticSeverity.Warn => (Colors.Yellow, Colors.Yellow, "warning"),
            DiagnosticSeverity.Info => (Colors.Blue, Colors.Cyan, "info"),
            _ => (Colors.White, Colors.Gray, "unknown")
        };

        var startLine = Span.Start.Line;
        var endLine = Span.End.Line;
        var startChar = Span.Start.Character;
        var endChar = Span.End.Character;
        var lineDigits = endLine.ToString().Length;
        var gutterIndent = new string(' ', lineDigits);
        var codePart = string.IsNullOrEmpty(Code) ? "" : $" {Colors.Dim}({Code}){Colors.Reset}{severityColor}";
        var header = $"{severityColor}{Colors.Bold}{severityLabel}{Colors.Reset}{severityColor}{codePart}:{Colors.Reset} {Colors.Gray}{Message}{Colors.Reset}";
        var location = $"{Colors.Dim}{gutterIndent} ╭─{Colors.Reset} {Colors.Orange}{Span}{Colors.Reset}";
        var sourceLines = Span.File.SourceText.Split(Environment.NewLine);
        var gutter = $"{Colors.Dim}{gutterIndent} │{Colors.Reset}";
        var lines = new List<string>([header, location, gutter]);
        if (startLine - 1 < sourceLines.Length && startLine - 1 > 0)
        {
            lines.Add($"{Colors.Dim}{startLine - 1} │ {sourceLines[startLine - 2]}{Colors.Reset}");
            lines.Add(gutter);
        }

        const char underlineChar = '\u2500';
        var hasHint = !string.IsNullOrEmpty(Hint);
        var pad = new string(' ', startChar);
        var noPad = false;
        for (var line = startLine; line <= endLine; line++)
        {
            var lineIndex = line - 1;
            var lineContent = sourceLines[lineIndex];
            var lineNumber = line.ToString().PadLeft(lineDigits);
            var lineSubtraction = hasHint ? Math.Min(endChar - startChar, 1) : 0;
            var linePrepend = hasHint ? line != endLine && startLine != endLine ? "─" : "┬" : "";
            if (line == startLine && line == endLine)
            {
                var underline = $"{pad}{underlineColor}{linePrepend}{new string(underlineChar, endChar - startChar - lineSubtraction)}{Colors.Reset}";
                lines.Add($"{Colors.Bold}{lineNumber} │{Colors.Reset} {lineContent}");
                lines.Add($"{gutter} {Colors.Bold}{underline}{Colors.Reset}");
            }
            else if (line == startLine)
            {
                var underline = $"{pad}{underlineColor}{linePrepend}{new string(underlineChar, lineContent.Length - startChar - lineSubtraction)}{Colors.Reset}";
                lines.Add($"{Colors.Bold}{lineNumber} │{Colors.Reset} {lineContent}");
                lines.Add($"{gutter} {Colors.Bold}{underline}{Colors.Reset}");
            }
            else if (line == endLine)
            {
                noPad = true;
                var underline = $"{underlineColor}{linePrepend}{new string(underlineChar, endChar - lineSubtraction)}{Colors.Reset}";
                lines.Add($"{Colors.Bold}{lineNumber} │{Colors.Reset} {lineContent}");
                lines.Add($"{gutter} {Colors.Bold}{underline}{Colors.Reset}");
            }
            else
            {
                noPad = true;
                var underline = $"{underlineColor}{linePrepend}{new string(underlineChar, lineContent.Length - lineSubtraction)}{Colors.Reset}";
                lines.Add($"{Colors.Bold}{lineNumber} │{Colors.Reset} {lineContent}");
                lines.Add($"{gutter} {Colors.Bold}{underline}{Colors.Reset}");
            }
        }

        if (hasHint)
        {
            lines.Add(
                $"{gutter}{(noPad ? "" : pad)} {underlineColor}╰─{Colors.Reset}  {severityColor}{Colors.Bold}Hint:{Colors.Reset} {Colors.Gray}{Hint}{Colors.Reset}"
            );
            lines.Add(gutter);
        }
        
        if (endLine < sourceLines.Length)
            lines.Add($"{gutter} {Colors.Dim}{sourceLines[endLine]}{Colors.Reset}");
        
        lines.Add(gutter);
        return string.Join(Environment.NewLine, lines);
    }
}