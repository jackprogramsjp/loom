using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking;

namespace Loom;

public class CompilationUnit(List<SourceFile> files)
{
    private readonly List<DiagnosticBag> _pipelineDiagnostics = [];

    public static CompiledFile CompileFile(SourceFile file)
    {
        var compiler = new CompilationUnit([file]);
        var compilation = compiler.Compile();
        return compilation.Files.First();
    }

    public DiagnosticBag GetDiagnostics() => DiagnosticBag.Concat(_pipelineDiagnostics);
    
    public CompilationResult Compile()
    {
        var compiledFiles = files.ConvertAll(Compile);
        var diagnostics = DiagnosticBag.Concat(compiledFiles.ConvertAll(file => file.Diagnostics));
        return new CompilationResult(compiledFiles, diagnostics);
    }

    private CompiledFile Compile(SourceFile file)
    {
        _pipelineDiagnostics.Clear();
        var lexer = new Lexer(file);
        var lexerResult = TrackDiagnostics(lexer.Tokenize());
        var parser = new Parser(file, lexerResult.Tokens);
        var parserResult = TrackDiagnostics(parser.Parse());
        var resolver = new Resolver(parserResult.Tree);
        var semanticModel = TrackDiagnostics(resolver.Resolve());
        var typeChecker = new TypeChecker(semanticModel);
        var typeCheckerResult = TrackDiagnostics(typeChecker.Check());
        var diagnostics = GetDiagnostics();

        return new CompiledFile
        {
            Diagnostics = diagnostics,
            ReturnType = typeCheckerResult.ReturnType,
            SemanticModel = semanticModel,
            Tree = parserResult.Tree,
            Tokens = lexerResult.Tokens,
        };
    }

    private T TrackDiagnostics<T>(T result)
        where T : DiagnosedResult
    {
        _pipelineDiagnostics.Add(result.Diagnostics);
        return result;
    }
}