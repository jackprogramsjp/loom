using Loom.Diagnostics;
using Loom.Generation;
using Loom.Lexing;
using Loom.Parsing;
using Loom.Projects;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking;

namespace Loom;

public class CompilationUnit(LoomConfig loomConfig)
{
    public CompilationResult Compile()
    {
        var sourceFiles = FileManager.LoadDirectory(loomConfig.Files.SourceDirectory);
        var compiledFiles = sourceFiles.ConvertAll(Compile);
        var diagnostics = DiagnosticBag.Concat(compiledFiles.ConvertAll(file => file.Diagnostics));
        if (!diagnostics.ContainsErrors())
        {
            compiledFiles.ForEach(FileManager.WriteCompiledFile);
        }

        return new CompilationResult(compiledFiles, diagnostics);
    }

    public CompiledFile Compile(SourceFile file)
    {
        var pipelineDiagnostics = new List<DiagnosticBag>();
        try
        {
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
                Path = file.AbsolutePath
                    .Replace(
                        Path.GetFileName(loomConfig.Files.SourceDirectory) + Path.DirectorySeparatorChar,
                        Path.GetFileName(loomConfig.Files.OutputDirectory) + Path.DirectorySeparatorChar
                    )
                    .Replace(FileManager.LoomExtension, ".luau"),
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