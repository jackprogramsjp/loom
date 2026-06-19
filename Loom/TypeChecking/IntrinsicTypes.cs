using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public record IntrinsicType(string Name, Type Type)
{
    public string Name { get; } = Name;
    public Symbol Symbol { get; } = new(new NullStatement(null), SymbolKind.Type, Name, false, true);
    public Type Type { get; } = Type;
}

public static class IntrinsicTypes
{
    public static readonly IntrinsicType Range = new(
        "Range",
        new InterfaceType(
            "Range",
            [],
            new ObjectType(
                null,
                [new ObjectProperty(false, "minimum", PrimitiveType.Number), new ObjectProperty(false, "maximum", PrimitiveType.Number)]
            )
        )
    );

    private static readonly List<IntrinsicType> _types = [Range];

    public static void Register(SemanticModel model) => Resolver.DeclareGlobal([..GetSymbols(model)]);
    
    private static List<Symbol> GetSymbols(SemanticModel semanticModel) =>
        _types.ConvertAll(t =>
                {
                    semanticModel.TypeSolver.SetType(t.Symbol.Declaration, t.Type);
                    return t;
                }
            )
            .ConvertAll(t => t.Symbol);
}