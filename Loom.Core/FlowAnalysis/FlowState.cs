using Loom.Resolving;
using Loom.TypeChecking;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.FlowAnalysis;

public sealed class FlowState(
    HashSet<Symbol>? definitelyInitialized = null,
    HashSet<Symbol>? maybeInitialized = null,
    bool isUnreachable = false,
    Dictionary<FlowAddress, Type>? narrowedTypes = null
)
{
    public HashSet<Symbol> DefinitelyInitialized { get; } = [..definitelyInitialized ?? []];
    public HashSet<Symbol> MaybeInitialized { get; } = [..maybeInitialized ?? []];
    public bool IsUnreachable { get; init; } = isUnreachable;
    public Dictionary<FlowAddress, Type> NarrowedTypes { get; } = new(narrowedTypes ?? []);

    public FlowState(FlowState from)
        : this(from.DefinitelyInitialized, from.MaybeInitialized, from.IsUnreachable, from.NarrowedTypes)
    {
    }

    public FlowState Merge(FlowState other)
    {
        var result = new FlowState { IsUnreachable = IsUnreachable && other.IsUnreachable };
        result.DefinitelyInitialized.UnionWith(DefinitelyInitialized.Intersect(other.DefinitelyInitialized));
        result.MaybeInitialized.UnionWith(MaybeInitialized);
        result.MaybeInitialized.UnionWith(other.MaybeInitialized);

        foreach (var address in NarrowedTypes.Keys.Concat(other.NarrowedTypes.Keys).Distinct())
        {
            if (!NarrowedTypes.TryGetValue(address, out var left)) continue;
            if (!other.NarrowedTypes.TryGetValue(address, out var right)) continue;
            result.NarrowedTypes[address] = TypeSimplifier.Simplify(new TypeChecking.Types.UnionType([left, right]));
        }

        return result;
    }
}