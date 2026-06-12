namespace Loom.SemanticAnalysis;

internal record FlowState(HashSet<Symbol> DefinitelyInitialized, HashSet<Symbol> MaybeInitialized, bool IsUnreachable = false)
{
    public bool IsUnreachable { get; set; } = IsUnreachable;

    public FlowState(FlowState from)
    {
        DefinitelyInitialized = new HashSet<Symbol>(from.DefinitelyInitialized);
        MaybeInitialized = new HashSet<Symbol>(from.MaybeInitialized);
        IsUnreachable = from.IsUnreachable;
    }
}