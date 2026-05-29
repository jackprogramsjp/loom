using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;

namespace Loom.Testing;

internal static class Utility
{
    public static List<Token> GetTokens(string source) => Tokenize(source).Tokens;
    public static Tree GetAST(string source) => Parse(source).Tree;
    public static SemanticModel GetSemanticModel(string source) => new Resolver(GetAST(source)).Resolve();
    public static DiagnosticBag GetParserDiagnostics(string source) => Parse(source).Diagnostics;
    public static DiagnosticBag GetLexerDiagnostics(string source) => Tokenize(source).Diagnostics;

    public static void AssertDiagnostic(DiagnosticBag diagnostics, string code, string message)
    {
        var diagnostic = diagnostics.Find(d => d.Code == code);
        Assert.NotNull(diagnostic);
        Assert.Equal(message, diagnostic.Message);
    }

    private static LexerResult Tokenize(string source) => new Lexer(TestFile(source)).Tokenize();
    private static ParserResult Parse(string source) => new Parser(TestFile(source), GetTokens(source)).Parse();
    
    private static SourceFile TestFile(string source) => new("test", source);
}