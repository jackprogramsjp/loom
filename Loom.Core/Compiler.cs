using Loom.Core.Diagnostics;
using Loom.Core.FlowAnalysis;
using Loom.Core.Generation;
using Loom.Core.Lexing;
using Loom.Core.Parsing;
using Loom.Core.Resolving;
using Loom.Core.Text;
using Loom.Core.TypeChecking;

namespace Loom.Core;

public sealed class Compiler(CompilationUnit unit, SourceFile file)
{
    public CompiledFile Compile()
    {
        var pipelineDiagnostics = new List<DiagnosticBag>();
        try
        {
            var lexer = new Lexer(file);
            var lexerResult = trackDiagnostics(lexer.Tokenize());
            var parser = new Parser(lexerResult);
            var parserResult = trackDiagnostics(parser.Parse());
            var resolver = new Resolver(parserResult, unit);
            var semanticModel = trackDiagnostics(resolver.Resolve());
            var flowAnalyzer = new FlowAnalyzer(semanticModel);
            var flowAnalyzerResult = trackDiagnostics(flowAnalyzer.Analyze());
            var typeChecker = new TypeChecker(semanticModel, flowAnalyzer);
            var typeCheckerResult = trackDiagnostics(typeChecker.Check());
            var generator = new LuauGenerator(semanticModel);
            var generatorResult = trackDiagnostics(generator.Generate());
            var renderedLuau = generatorResult.LuauTree.Render();
            var diagnostics = DiagnosticBag.Concat(pipelineDiagnostics);
            var compiledFilePath = file.AbsolutePath
                .Replace(
                    Path.GetFileName(unit.Config.Files.SourceDirectory) + Path.DirectorySeparatorChar,
                    Path.GetFileName(unit.Config.Files.OutputDirectory) + Path.DirectorySeparatorChar
                )
                .Replace(FileManager.LoomExtension, ".luau");

            return new CompiledFile(file)
            {
                Path = compiledFilePath,
                Diagnostics = diagnostics,
                RenderedLuau = renderedLuau,
                LuauTree = generatorResult.LuauTree,
                ReturnType = typeCheckerResult.ReturnType,
                SemanticModel = semanticModel,
                Tree = parserResult.Tree,
                Tokens = lexerResult.Tokens
            };
        }
        catch (Exception e)
        {
            var diagnostics = DiagnosticBag.Concat(pipelineDiagnostics);
            DiagnosticBag.FailFast = true;
            diagnostics.CompilerError(file, $"The compiler threw an exception!\n{e.Message}\n{e.StackTrace}");
            return null!;
        }

        T trackDiagnostics<T>(T result)
            where T : DiagnosedResult
        {
            pipelineDiagnostics.Add(result.Diagnostics);
            return result;
        }
    }
}