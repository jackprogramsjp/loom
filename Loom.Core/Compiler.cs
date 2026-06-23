using Loom.Diagnostics;
using Loom.Generation;
using Loom.Lexing;
using Loom.Parsing;
using Loom.SemanticAnalysis;
using Loom.Text;
using Loom.TypeChecking;

namespace Loom;

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
            var typeChecker = new TypeChecker(semanticModel);
            var typeCheckerResult = trackDiagnostics(typeChecker.Check());
            var generator = new LuauGenerator(semanticModel);
            var generatorResult = trackDiagnostics(generator.Generate());
            var renderedLuau = generatorResult.LuauTree.Render();
            var diagnostics = DiagnosticBag.Concat(pipelineDiagnostics);
            var compiledFilePath = file.AbsolutePath
                .Replace(
                    Path.GetFileName(unit.LoomConfig.Files.SourceDirectory) + Path.DirectorySeparatorChar,
                    Path.GetFileName(unit.LoomConfig.Files.OutputDirectory) + Path.DirectorySeparatorChar
                )
                .Replace(FileManager.LoomExtension, ".luau");

            return new CompiledFile
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