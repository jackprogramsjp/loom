using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;

namespace Loom.Core.Parsing;

public sealed record ParserResult(Tree Tree, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);