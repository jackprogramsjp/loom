global using SymbolLookup = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Loom.Resolving.Symbol>>;

namespace Loom.Resolving;

internal sealed record ResolverScope
{
    public SymbolLookup VariableLookup { get; } = [];
    public SymbolLookup TypeLookup { get; } = [];
}