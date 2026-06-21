using Loom.Diagnostics;
using Loom.Parsing.AST;

namespace Loom.Parsing;

public sealed class ParserResult(Tree tree, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public Tree Tree { get; } = tree;
}