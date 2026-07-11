using Loom.Config;
using Loom.Core.Resolving;
using Loom.Core.TypeChecking.Types;
using PrimitiveType = Loom.Core.TypeChecking.Types.PrimitiveType;

namespace Loom.Core.TypeChecking;

public static class Intrinsics
{
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

    public static HashSet<Symbol> Register(SemanticModel model)
    {
        if (_compilingIntrinsic) return [];
        _compilingIntrinsic = true;

        var sourceDirectory = Path.GetDirectoryName(
            Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))))
        );

        var loomConfig = new LoomConfig { NoEmit = true, Files = new FilesConfig { SourceDirectory = $"{sourceDirectory}/Loom.Core/TypeChecking/Intrinsic" } };
        var compilationUnit = new CompilationUnit(loomConfig);
        var compiledFiles = compilationUnit.SourceFiles.Select(f =>
            {
                f.IsIntrinsic = true;
                return compilationUnit.Compile(f);
            }
        );

        var intrinsicSymbols = new HashSet<Symbol>();
        foreach (var compiledFile in compiledFiles)
        {
            var symbols = compiledFile.Tree.Statements.SelectMany(statement => compiledFile.SemanticModel.GetDeclarationSymbols(statement));
            foreach (var symbol in symbols)
            {
                symbol.IsIntrinsic = true;
                symbol.IsGlobal = true;
                model.TypeSolver.SetType(symbol.Declaration, compiledFile.SemanticModel.GetType(symbol.Declaration));
                intrinsicSymbols.Add(symbol);
            }
        }

        _compilingIntrinsic = false;
        return intrinsicSymbols;
    }
}