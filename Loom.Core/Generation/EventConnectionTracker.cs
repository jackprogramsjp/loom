using System.Diagnostics.CodeAnalysis;
using Loom.Core.Resolving;
using Loom.Luau.AST;

namespace Loom.Core.Generation;

internal sealed class EventConnectionTracker
{
    private readonly Dictionary<(Symbol Event, Symbol Function), LuauExpression> _connections = [];

    public void Track(Symbol eventSymbol, Symbol functionSymbol, LuauExpression connection) => _connections[(eventSymbol, functionSymbol)] = connection;

    public bool TryGetConnection(Symbol eventSymbol, Symbol functionSymbol, [MaybeNullWhen(false)] out LuauExpression connection) =>
        _connections.TryGetValue((eventSymbol, functionSymbol), out connection);
}