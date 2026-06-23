using Loom.Debug;
using Loom.Diagnostics;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Text;
using Loom.Utility;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom;

public sealed class CompiledFile
{
    public required string Path { get; init; }
    public required DiagnosticBag Diagnostics { get; init; }
    public required string RenderedLuau { get; init; }
    public required LuauTree LuauTree { get; init; }
    public required Type ReturnType { get; init; }
    public required SemanticModel SemanticModel { get; init; }
    public required Tree Tree { get; init; }
    public required IReadOnlyList<Token> Tokens { get; init; }

    public void WriteDebugInfo(bool tokens = true, bool ast = true, bool rebuilt = true, bool luau = true, bool showDiagnostics = true, bool debugDiagnostics = true)
    {
        var astDisplayer = new ASTDisplayer(Tree);
        if (tokens)
        {
            Console.WriteLine("Tokens:");
            foreach (var token in Tokens)
                Console.WriteLine(token.ToString());
        }

        if (ast)
        {
            Console.WriteLine();
            Console.WriteLine("AST:");
            astDisplayer.Display();
        }

        if (rebuilt)
        {
            Console.WriteLine();
            Console.WriteLine("Rebuilt program:");
            Console.WriteLine(Tree.ToString());
        }

        if (luau)
        {
            Console.WriteLine();
            Console.WriteLine("Compiled Luau program:");
            Console.WriteLine(RenderedLuau);
        }

        if (!showDiagnostics) return;
        var compilerDiagnostics = (debugDiagnostics ? Diagnostics : Diagnostics.WithoutInfo()).ToString();
        Console.WriteLine();
        Console.WriteLine("Diagnostics:");
        Console.WriteLine(string.IsNullOrEmpty(compilerDiagnostics) ? "(none)" : compilerDiagnostics);
    }
}