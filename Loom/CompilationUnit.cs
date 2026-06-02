using Loom.Diagnostics;
using Loom.Generation;
using Loom.Lexing;
using Loom.Parsing;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking;

namespace Loom;

public class CompilationUnit(List<SourceFile> files)
{
    public static CompiledFile Compile(SourceFile file)
    {
        var pipelineDiagnostics = new List<DiagnosticBag>();
        var lexer = new Lexer(file);
        var lexerResult = trackDiagnostics(lexer.Tokenize());
        var parser = new Parser(lexerResult);
        var parserResult = trackDiagnostics(parser.Parse());
        var resolver = new Resolver(parserResult);
        var semanticModel = trackDiagnostics(resolver.Resolve());
        var typeChecker = new TypeChecker(semanticModel);
        var typeCheckerResult = trackDiagnostics(typeChecker.Check());
        var generator = new LuauGenerator(semanticModel);
        var generatorResult = trackDiagnostics(generator.Generate());
        var renderedLuau = generatorResult.LuauTree.Render();
        var diagnostics = DiagnosticBag.Concat(pipelineDiagnostics);

        return new CompiledFile
        {
            Diagnostics = diagnostics,
            RenderedLuau = renderedLuau,
            LuauTree = generatorResult.LuauTree,
            ReturnType = typeCheckerResult.ReturnType,
            SemanticModel = semanticModel,
            Tree = parserResult.Tree,
            Tokens = lexerResult.Tokens
        };
        
        T trackDiagnostics<T>(T result)
            where T : DiagnosedResult
        {
            pipelineDiagnostics.Add(result.Diagnostics);
            return result;
        }
    }
    
    public CompilationResult Compile()
    {
        var compiledFiles = files.ConvertAll(Compile);
        var diagnostics = DiagnosticBag.Concat(compiledFiles.ConvertAll(file => file.Diagnostics));
        return new CompilationResult(compiledFiles, diagnostics);
    }
}