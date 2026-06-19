using System.Reflection;
using Loom.Parsing.AST;
using Loom.Projects;
using Loom.SemanticAnalysis;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public static class IntrinsicTypes
{
    private static bool _compilingIntrinsic;
    
    public static readonly ObjectType Range = new(
        null,
        [new ObjectProperty(false, "minimum", PrimitiveType.Number), new ObjectProperty(false, "maximum", PrimitiveType.Number)]
    );

    public static List<Symbol> Register(SemanticModel model)
    {
        if (_compilingIntrinsic) return [];
        _compilingIntrinsic = true;
        
        var sourceDirectory = Path.GetDirectoryName(
            Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))))
        );

        var loomConfig = new LoomConfig { NoEmit = true, Files = new FilesConfig { SourceDirectory = $"{sourceDirectory}/Loom/TypeChecking/Intrinsic" } };
        var compilationUnit = new CompilationUnit(loomConfig);
        var symbols = new List<Symbol>();
        foreach (var compiledFile in compilationUnit.SourceFiles.Select(compilationUnit.Compile))
        {
            foreach (var symbol in compiledFile.Tree.Statements.Select(statement => compiledFile.SemanticModel.GetDeclarationSymbol(statement)).OfType<Symbol>())
            {
                model.TypeSolver.SetType(symbol.Declaration, compiledFile.SemanticModel.GetType(symbol.Declaration));
                symbols.Add(symbol);
            }
        }

        _compilingIntrinsic = false;
        return symbols;
    }
}