namespace Loom.TypeChecking;

internal record struct TypedFlowState()
{
    public Dictionary<TypedFlowAddress, Types.Type> NarrowedTypes { get; } = [];

    public TypedFlowState(TypedFlowState other) : this()
    {
        foreach (var kv in other.NarrowedTypes)
            NarrowedTypes[kv.Key] = kv.Value;
    }
}