using System.Text;
using Loom.Debug;
using Loom.Diagnostics;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Text;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom;

public sealed class CompiledFile(SourceFile sourceFile)
{
    public SourceFile SourceFile { get; } = sourceFile;
    public required string Path { get; init; }
    public required DiagnosticBag Diagnostics { get; init; }
    public required string RenderedLuau { get; init; }
    public required LuauTree LuauTree { get; init; }
    public required Type ReturnType { get; init; }
    public required SemanticModel SemanticModel { get; init; }
    public required Tree Tree { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }

    public string GetDebugInfo(
        bool tokens = true,
        bool ast = true,
        bool rebuilt = true,
        bool luau = true,
        bool showDiagnostics = true,
        bool debugDiagnostics = true)
    {
        var sb = new StringBuilder();

        if (tokens)
        {
            appendHeader("Tokens");
            foreach (var token in Tokens)
                sb.AppendLine(token.ToString());
            sb.AppendLine();
        }

        if (ast)
        {
            appendHeader("AST");
            sb.AppendLine(ASTInspector.Inspect(Tree));
            sb.AppendLine();
        }

        if (rebuilt)
        {
            appendHeader("Rebuilt program");
            sb.AppendLine(Tree.ToString());
            sb.AppendLine();
        }

        if (luau)
        {
            appendHeader("Compiled Luau program");
            sb.AppendLine(RenderedLuau);
            sb.AppendLine();
        }

        if (showDiagnostics)
        {
            appendHeader("Diagnostics");
            var compilerDiagnostics = (debugDiagnostics ? Diagnostics : Diagnostics.WithoutInfo()).ToString();
            sb.AppendLine(string.IsNullOrEmpty(compilerDiagnostics) ? "(none)" : compilerDiagnostics);
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();

        void appendHeader(string title)
        {
            var line = new string('─', Math.Min(title.Length + 12, 60));
            sb.AppendLine(line);
            sb.AppendLine($"  {title}");
            sb.AppendLine(line);
        }
    }
}