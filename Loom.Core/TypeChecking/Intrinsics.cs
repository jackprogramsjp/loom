using Loom.Config;
using Loom.Core.Resolving;
using Loom.Core.TypeChecking.Types;
using PrimitiveType = Loom.Core.TypeChecking.Types.PrimitiveType;

namespace Loom.Core.TypeChecking;

public static class Intrinsics
{
    private static HashSet<(Symbol, Types.Type)>? _cachedIntrinsics;
    private static bool _compilingIntrinsic;

    public static readonly InterfaceType Range = new(
        "Range",
        [],
        new ObjectType(
            null,
            [
                new ObjectProperty(false, "minimum", PrimitiveType.Number),
                new ObjectProperty(false, "maximum", PrimitiveType.Number),
                new ObjectProperty(false, "length", PrimitiveType.Number),
                new ObjectProperty(false, "clamp", new FunctionType([], [PrimitiveType.Number], PrimitiveType.Number))
            ]
        )
    );

    public static HashSet<(Symbol, Types.Type)> Register(SemanticModel model, CompilationUnit injectInto)
    {
        _cachedIntrinsics ??= CompileIntrinsics(injectInto);

        foreach (var (symbol, type) in _cachedIntrinsics)
            model.TypeSolver.SetType(symbol.Declaration, type);

        return _cachedIntrinsics;
    }

    private static HashSet<(Symbol, Types.Type)> CompileIntrinsics(CompilationUnit injectInto)
    {
        if (_compilingIntrinsic) return [];
        _compilingIntrinsic = true;

        var sourceDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
        var loomConfig = new LoomConfig
        {
            ProjectType = ProjectType.Library, NoEmit = true, Files = new FilesConfig { SourceDirectory = $"{sourceDirectory}/Loom.Core/TypeChecking/Intrinsic" }
        };

        var compilationUnit = new CompilationUnit(loomConfig);
        var compiledFiles = compilationUnit.SourceFiles
            .Where(file =>
            {
                file.IsIntrinsic = true;
                
                if (injectInto.Config.ProjectType != ProjectType.Plugin && file.Name == "PluginSecurity.loom")
                    return false;
                
                return injectInto.Config.ProjectType != ProjectType.Plugin || file.Name != "None.loom";
            })
            .Select(compilationUnit.Compile)
            .ToArray();

        var intrinsicSymbols = new HashSet<(Symbol, Types.Type)>();
        foreach (var compiledFile in compiledFiles)
        {
            var symbols = compiledFile.Tree.Statements.SelectMany(statement => compiledFile.SemanticModel.GetDeclarationSymbols(statement));
            foreach (var symbol in symbols)
            {
                symbol.IsIntrinsic = true;
                symbol.IsGlobal = true;
                intrinsicSymbols.Add((symbol, compiledFile.SemanticModel.GetType(symbol.Declaration)));
            }
        }

        _compilingIntrinsic = false;
        return intrinsicSymbols;
    }
}