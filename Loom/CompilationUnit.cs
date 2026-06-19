using Loom.Diagnostics;
using Loom.Generation;
using Loom.Lexing;
using Loom.Parsing;
using Loom.Projects;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom;

public class CompilationUnit(LoomConfig loomConfig)
{
    public List<SourceFile> SourceFiles { get; } = FileManager.LoadDirectory(loomConfig.Files.SourceDirectory);
    public Dictionary<Symbol, Type> Globals { get; } = [];
    
    public CompilationResult Compile()
    {
        Globals.Clear();
        var compiledDeclarationFiles = SourceFiles.FindAll(file => file.IsDeclaration).ConvertAll(Compile);
        foreach (var compiledFile in compiledDeclarationFiles)
        {
            foreach (var symbol in compiledFile.Tree.Statements.Select(statement => compiledFile.SemanticModel.GetDeclarationSymbol(statement)).OfType<Symbol>())
            {
                var type = compiledFile.SemanticModel.GetType(symbol.Declaration);
                Globals.Add(symbol, type);
            }
        }

        var compiledConcreteFiles = SourceFiles.FindAll(file => !file.IsDeclaration).ConvertAll(Compile);
        var compiledFiles = compiledDeclarationFiles.Concat(compiledConcreteFiles).ToList();
        var diagnostics = DiagnosticBag.Concat(compiledFiles.ConvertAll(file => file.Diagnostics));
        if (!diagnostics.ContainsErrors() && !loomConfig.NoEmit)
            compiledFiles.ForEach(FileManager.WriteCompiledFile);

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
            var resolver = new Resolver(parserResult, this);
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