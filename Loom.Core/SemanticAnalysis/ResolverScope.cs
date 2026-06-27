namespace Loom.SemanticAnalysis;

using SymbolLookup = Dictionary<string, List<Symbol>>;

internal sealed record ResolverScope
{
    public SymbolLookup VariableLookup { get; } = [];
    public SymbolLookup TypeLookup { get; } = [];
}