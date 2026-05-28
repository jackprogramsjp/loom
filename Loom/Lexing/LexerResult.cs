using Loom.Diagnostics;
using Loom.Syntax;

namespace Loom.Lexing;

public class LexerResult(List<Token> tokens, DiagnosticBag diagnostics)
    : DiagnosedResult(diagnostics)
{
    public List<Token> Tokens { get; } = tokens;
}