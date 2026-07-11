using Loom.Core.Diagnostics;
using Loom.Core.Text;

namespace Loom.Core.Lexing;

public sealed record LexerResult(SourceFile File, List<Token> Tokens, DiagnosticBag Diagnostics)
    : DiagnosedResult(Diagnostics);