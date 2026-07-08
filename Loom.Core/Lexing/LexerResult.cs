using Loom.Diagnostics;
using Loom.Text;

namespace Loom.Lexing;

public sealed record LexerResult(SourceFile File, List<Token> Tokens, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);