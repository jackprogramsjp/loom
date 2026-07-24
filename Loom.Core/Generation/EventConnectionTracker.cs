using System.Diagnostics.CodeAnalysis;
using Loom.Core.Resolving;
using Loom.Luau.AST;

namespace Loom.Core.Generation;

/// <summary>
/// Identifies the target of an event connection/disconnection: the event member itself, plus
/// (for interface-member events) which underlying instance it was accessed through, since the
/// same <see cref="PropertySymbol"/> is shared by every variable of that interface type.
/// </summary>
/// <param name="Instance">
/// <c>null</c> for global events (there is only ever one instance); the base variable's resolved
/// <see cref="Symbol"/> when the target is <c>identifier.eventName</c>; or a fresh <see cref="object"/>
/// when the base expression isn't a simple identifier (e.g. a call expression), which deliberately
/// makes the connection untrackable later since re-evaluating the base expression may yield a
/// different runtime object.
/// </param>
internal readonly record struct EventTarget(object? Instance, Symbol Event);

internal sealed class EventConnectionTracker
{
    private readonly Dictionary<(EventTarget Target, Symbol Function), LuauExpression> _connections = [];

    public void Track(EventTarget target, Symbol functionSymbol, LuauExpression connection) => _connections[(target, functionSymbol)] = connection;

    public bool TryGetConnection(EventTarget target, Symbol functionSymbol, [MaybeNullWhen(false)] out LuauExpression connection) =>
        _connections.TryGetValue((target, functionSymbol), out connection);
}
