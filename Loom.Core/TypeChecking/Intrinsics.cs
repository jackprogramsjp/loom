using Loom.Config;
using Loom.Core.Resolving;
using Loom.Core.Text;
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

    public static HashSet<(Symbol, Types.Type)> Register(SemanticModel model)
    {
        _cachedIntrinsics ??= CompileIntrinsics();
        
        foreach (var (symbol, type) in _cachedIntrinsics)
            model.TypeSolver.SetType(symbol.Declaration, type);

        return _cachedIntrinsics;
    }

    private static HashSet<(Symbol, Types.Type)> CompileIntrinsics()
    {
        if (_compilingIntrinsic) return [];
        _compilingIntrinsic = true;
        
        var sourceDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
        var loomConfig = new LoomConfig { NoEmit = true, Files = new FilesConfig { SourceDirectory = $"{sourceDirectory}/Loom.Core/TypeChecking/Intrinsic" } };
        var compilationUnit = new CompilationUnit(loomConfig);
        var compiledFiles = compilationUnit.SourceFiles.Select(file =>
                {
                    file.IsIntrinsic = true;
                    return compilationUnit.Compile(file);
                }
            )
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