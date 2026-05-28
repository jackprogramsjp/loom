namespace Loom.SemanticAnalysis;

public class ScopeNode
{
    public ScopeNode? Parent { get; init; }
    public List<ScopeNode> Children { get; } = [];
    public List<Symbol> Symbols { get; } = [];
}