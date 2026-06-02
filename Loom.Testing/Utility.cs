using Loom.Diagnostics;
using Loom.Generation;
using Loom.Lexing;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking;
using ExpressionStatement = Loom.Luau.AST.ExpressionStatement;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Testing;

internal static class Utility
{
    public static readonly LocationSpan Span = LocationSpan.Empty(TestFile(""));
    
    public static List<Token> GetTokens(string source) => Tokenize(source).Tokens;
    public static Tree GetAST(string source) => Parse(source).Tree;
    public static Type GetLastStatementType(string source) => TypeCheck(source).ReturnType;
    public static LuauTree GetLuauAST(string source) => new LuauGenerator(GetSemanticModel(source)).Generate().LuauTree;
    
    public static DiagnosticBag GetLexerDiagnostics(string source) => Tokenize(source).Diagnostics;
    public static DiagnosticBag GetParserDiagnostics(string source) => Parse(source).Diagnostics;
    public static SemanticModel GetSemanticModel(string source) => new Resolver(Parse(source)).Resolve();
    public static DiagnosticBag GetTypeCheckerDiagnostics(string source) => TypeCheck(source).Diagnostics;

    public static void AssertDiagnostic(DiagnosticBag diagnostics, string code, string message)
    {
        var diagnostic = diagnostics.Find(d => d.Code == code);
        Assert.NotNull(diagnostic);
        Assert.Equal(message, diagnostic.Message);
    }

    public static Token IdentifierToken(string name, LocationSpan? span = null) => Token(SyntaxKind.Identifier, name, span);
    public static Token Token(SyntaxKind kind, string text, LocationSpan? span = null) => new(kind, span ?? Span, text);
    
    private static LexerResult Tokenize(string source) => new Lexer(TestFile(source)).Tokenize();
    private static ParserResult Parse(string source) => new Parser(Tokenize(source)).Parse();
    private static TypeCheckerResult TypeCheck(string source) => new TypeChecker(GetSemanticModel(source)).Check();
    
    private static SourceFile TestFile(string source) => new("test", source);
}