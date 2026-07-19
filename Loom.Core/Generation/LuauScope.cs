using Loom.Luau.AST;

namespace Loom.Core.Generation;

internal sealed class LuauScope(LuauScope? parent = null)
{
    private readonly Dictionary<string, int> _nameCounts = parent?._nameCounts ?? [];

    public LuauScope? Parent { get; } = parent;
    public List<LuauStatement> PrereqStatements { get; } = [];
    public List<LuauStatement> PostreqStatements { get; } = [];

    public string AddIdentifier(string name)
    {
        if (TryGetId(name, out var temporaryId))
            return name + "_" + (_nameCounts[name] = temporaryId + 1);

        _nameCounts[name] = 0;
        return name;
    }

    private bool TryGetId(string name, out int temporaryId) => _nameCounts.TryGetValue(name, out temporaryId);
}