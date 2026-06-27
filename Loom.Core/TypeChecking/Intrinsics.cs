using Loom.Projects;
using Loom.SemanticAnalysis;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;

namespace Loom.TypeChecking;

public static class Intrinsics
{
    private static bool _compilingIntrinsic;

    public static readonly InterfaceType RangeType = new(
        "Range",
        [],
        new ObjectType(
            null,
            [new ObjectProperty(false, "minimum", PrimitiveType.Number), new ObjectProperty(false, "maximum", PrimitiveType.Number)]
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
        var compiledFiles = compilationUnit.SourceFiles.Select(compilationUnit.Compile);
        
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