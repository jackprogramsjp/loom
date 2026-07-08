using Loom.FlowAnalysis;
using Loom.Resolving;

namespace Loom.TypeChecking;

public record struct TypedFlowState()
{
    public Dictionary<FlowAddress, Types.Type> NarrowedTypes { get; } = [];

    public TypedFlowState(TypedFlowState other) : this()
    {
        foreach (var kv in other.NarrowedTypes)
            NarrowedTypes[kv.Key] = kv.Value;
    }
}