using Loom.Diagnostics;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom;

public class CompiledFile
{
    public required DiagnosticBag Diagnostics { get; init; }
    public required string RenderedLuau { get; init; }
    public required LuauTree LuauTree { get; init; }
    public required Type ReturnType { get; init; }
    public required SemanticModel SemanticModel { get; init; }
    public required Tree Tree { get; init; }
    public required List<Token> Tokens { get; init; }
}