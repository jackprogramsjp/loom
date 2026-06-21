using Loom.Parsing.AST;

namespace Loom.SemanticAnalysis;

internal sealed record ResolverScope
{
    public Dictionary<NodeId, Symbol> Declarations { get; } = [];
    public Dictionary<NodeId, Symbol> References { get; } = [];
    public Dictionary<string, Symbol> VariableLookup { get; } = [];
    public Dictionary<string, Symbol> TypeLookup { get; } = [];
}