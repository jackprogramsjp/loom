using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Syntax;

namespace Loom.Testing;

internal static class Utility
{
    public static List<Token> GetTokens(string source) => Tokenize(source).Tokens;
    public static DiagnosticBag GetLexerDiagnostics(string source) => Tokenize(source).Diagnostics;

    private static LexerResult Tokenize(string source) => new Lexer(TestFile(source)).Tokenize();
    
    private static SourceFile TestFile(string source) => new("test", source);
}