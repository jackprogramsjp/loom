using Loom.Diagnostics;
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

    public static Type GetLastStatementType(string source)
    {
        var semanticModel = GetSemanticModel(source);
        var checker = new TypeChecker(semanticModel);
        checker.Check();

        Assert.True(semanticModel.Tree.Statements.Count > 0);
        return checker.TypeSolver.GetType(semanticModel.Tree.Statements.Last());
    }

    public static LuauTree GetLuauAST(string source) => new LuauGenerator(Parse(source).Tree).Generate().LuauTree;
    public static string Render(LuauNode node) => node.Render(new RenderState());
    
    public static DiagnosticBag GetTypeCheckerDiagnostics(string source) => new TypeChecker(GetSemanticModel(source)).Check().Diagnostics;
    public static SemanticModel GetSemanticModel(string source) => new Resolver(GetAST(source)).Resolve();
    public static DiagnosticBag GetParserDiagnostics(string source) => Parse(source).Diagnostics;
    public static DiagnosticBag GetLexerDiagnostics(string source) => Tokenize(source).Diagnostics;

    public static void AssertDiagnostic(DiagnosticBag diagnostics, string code, string message)
    {
        var diagnostic = diagnostics.Find(d => d.Code == code);
        Assert.NotNull(diagnostic);
        Assert.Equal(message, diagnostic.Message);
    }

    public static Token IdentifierToken(string name, LocationSpan? span = null) => Token(SyntaxKind.Identifier, name, span);
    
    private static LexerResult Tokenize(string source) => new Lexer(TestFile(source)).Tokenize();
    private static ParserResult Parse(string source) => new Parser(TestFile(source), GetTokens(source)).Parse();
    
    private static Token Token(SyntaxKind kind, string text, LocationSpan? span = null) => new(kind, span ?? Span, text);
    private static SourceFile TestFile(string source) => new("test", source);
}