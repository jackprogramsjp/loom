global using SymbolLookup = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Loom.SemanticAnalysis.Symbol>>;

namespace Loom.SemanticAnalysis;

internal sealed record ResolverScope
{
    public SymbolLookup VariableLookup { get; } = [];
    public SymbolLookup TypeLookup { get; } = [];
}