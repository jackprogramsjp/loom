using Loom.Diagnostics;
using Loom.Syntax;

namespace Loom.Lexing;

public class LexerResult(SourceFile file, IReadOnlyList<Token> tokens, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public SourceFile File { get; } = file;
    public IReadOnlyList<Token> Tokens { get; } = tokens;
}