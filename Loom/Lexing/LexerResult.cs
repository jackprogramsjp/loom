using Loom.Diagnostics;
using Loom.Syntax;

namespace Loom.Lexing;

public class LexerResult(SourceFile file, List<Token> tokens, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public SourceFile File { get; } = file;
    public List<Token> Tokens { get; } = tokens;
}