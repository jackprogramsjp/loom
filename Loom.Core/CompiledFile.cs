using Loom.Diagnostics;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Text;
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
}