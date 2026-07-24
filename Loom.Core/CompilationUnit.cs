using Loom.Config;
using Loom.Core.Diagnostics;
using Loom.Core.Resolving;
using Loom.Core.Text;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core;

public sealed class CompilationUnit(LoomConfig config)
{
    public LoomConfig Config { get; } = config;
    public List<SourceFile> SourceFiles { get; } = FileManager.LoadDirectory(config.Files.SourceDirectory);
    public Dictionary<Symbol, Type> Globals { get; } = [];
    public RuntimeImport RuntimeImport { get; } = ResolveRuntimeImport(config);

    private static RuntimeImport ResolveRuntimeImport(LoomConfig config)
    {
        var resolver = RojoResolver.FromProjectDirectory(config.ProjectDirectory);
        if (resolver == null)
            return RuntimeImport.Default;

        var segments = resolver.ResolveRunTimePath();
        return segments == null
            ? new RuntimeImport(RuntimeImportStatus.NotFoundInRojo, Core.RuntimeImport.DefaultPath)
            : new RuntimeImport(RuntimeImportStatus.Resolved, RuntimeImport.PathPrefix + string.Join('/', segments));
    }
    
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
        if (!diagnostics.ContainsErrors() && !Config.NoEmit)
            compiledFiles.ForEach(FileManager.WriteCompiledFile);

        return new CompilationResult(compiledFiles, diagnostics);
    }

    public CompiledFile Compile(SourceFile file) => new Compiler(this, file).Compile();
}