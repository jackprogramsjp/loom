using Loom.Diagnostics;
using Loom.FlowAnalysis;
using Loom.Generation;
using Loom.Lexing;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.Projects;
using Loom.Resolving;
using Loom.Text;
using Loom.TypeChecking;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Testing;

internal static class Utility
{
    public static readonly LocationSpan Span = LocationSpan.Empty(TestFile(""));

    public static IReadOnlyList<Token> GetTokens(string source, bool withTrivia = false) => Tokenize(source, withTrivia).Tokens;
    public static Tree GetAST(string source) => Parse(source).Tree;
    public static Type GetLastStatementType(string source) => TypeCheck(source).ReturnType;
    public static LuauTree GetLuauAST(string source, bool typeCheck = false) => Generate(source, typeCheck).LuauTree;

    public static DiagnosticBag GetGeneratorDiagnostics(string source, bool typeCheck = false) => Generate(source, typeCheck).Diagnostics;
    private static LuauGeneratorResult Generate(string source, bool typeCheck = false)
    {
        var result = FlowAnalyze(source);
        if (typeCheck)
        {
            var typeChecker = new TypeChecker(result.SemanticModel, result.Analyzer);
            typeChecker.Check();
        }

        return new LuauGenerator(result.SemanticModel).Generate();
    }

    public static DiagnosticBag GetLexerDiagnostics(string source) => Tokenize(source).Diagnostics;
    public static ParserResult Parse(string source) => new Parser(Tokenize(source)).Parse();
    public static DiagnosticBag GetParserDiagnostics(string source) => Parse(source).Diagnostics;

    public static SemanticModel GetSemanticModel(string source, bool isDeclaration = false)
    {
        var parserResult = Parse(source);
        if (isDeclaration)
            parserResult.Tree.File.IsDeclaration = true;
        
        var compilationUnit = new CompilationUnit(new LoomConfig());
        return new Resolver(parserResult, compilationUnit).Resolve();
    }
    
    public static (FlowAnalyzerResult AnalyzerResult, SemanticModel SemanticModel, FlowAnalyzer Analyzer) FlowAnalyze(string source)
    {
        var semanticModel = GetSemanticModel(source);
        var flowAnalyzer = new FlowAnalyzer(semanticModel);
        var result = flowAnalyzer.Analyze();
        return (result, semanticModel, flowAnalyzer);
    }

    private static TypeChecker GetTypeChecker(string source)
    {
        var (_, semanticModel, flowAnalyzer) = FlowAnalyze(source);
        return new TypeChecker(semanticModel, flowAnalyzer);
    }
    
    public static TypeCheckerResult TypeCheck(string source) => GetTypeChecker(source).Check();
    public static DiagnosticBag GetTypeCheckerDiagnostics(string source) => TypeCheck(source).Diagnostics;

    public static Token IdentifierToken(string name, LocationSpan? span = null) => Token(SyntaxKind.Identifier, name, span);
    public static Token Token(SyntaxKind kind, string text, LocationSpan? span = null) => new(kind, span ?? Span, text);

    public static T AssertNoErrors<T>(T result)
        where T : DiagnosedResult
    {
        AssertNoErrors(result.Diagnostics);
        return result;
    }

    public static void AssertNoErrors(DiagnosticBag diagnostics) => Assert.Empty(diagnostics.Set.Where(d => d.Severity == DiagnosticSeverity.Error));

    public static void AssertDiagnostic(DiagnosticBag diagnostics, string code, string message, string? hint = null)
    {
        var diagnostic = diagnostics.Find(d => d.Code == code);
        Assert.NotNull(diagnostic);
        Assert.Equal(message, diagnostic.Message);
        
        if (hint == null) return;
        Assert.Equal(hint, diagnostic.Hint);
    }
    
    public static IEnumerable<object[]> GetSnapshotFiles(string folderName, string targetExtension) =>
        Directory.EnumerateFiles(AssemblyFixture.Snapshots + '/' + folderName, $"*{FileManager.LoomExtension}")
            .Select(path => new object[] { path, path.Replace(FileManager.LoomExtension, targetExtension) });

    public static SourceFile TestFile(string source) => new("test", source);
    
    private static LexerResult Tokenize(string source, bool withTrivia = false) => new Lexer(TestFile(source)).Tokenize(withTrivia);
}