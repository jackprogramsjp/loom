using Loom.Diagnostics;
using Loom.Parsing.AST;

namespace Loom.Parsing;

public class ParserResult(Tree tree, DiagnosticBag diagnostics) : DiagnosedResult(diagnostics)
{
    public Tree Tree { get; } = tree;
}