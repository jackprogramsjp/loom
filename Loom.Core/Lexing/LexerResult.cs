using Loom.Diagnostics;
using Loom.Text;

namespace Loom.Lexing;

public sealed class LexerResult(SourceFile file, List<Token> tokens, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public SourceFile File { get; } = file;
    public List<Token> Tokens { get; } = tokens;
}