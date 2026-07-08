using Loom.Diagnostics;
using Loom.Parsing.AST;

namespace Loom.Parsing;

public sealed record ParserResult(Tree Tree, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);