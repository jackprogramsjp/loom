using Loom.Luau.AST;

namespace Loom.Generation;

internal sealed class LuauScope(LuauScope? parent = null)
{
    private readonly Dictionary<string, int> _temporaryIds = [];

    public LuauScope? Parent { get; } = parent;
    public List<LuauStatement> PrereqStatements { get; } = [];
    public List<LuauStatement> PostreqStatements { get; } = [];

    public string AddIdentifier(string name)
    {
        if (TryGetId(name, out var temporaryId))
            return name + "_" + (_temporaryIds[name] = temporaryId + 1);

        _temporaryIds.Add(name, 0);
        return name;
    }

    private bool TryGetId(string name, out int temporaryId) =>
        _temporaryIds.TryGetValue(name, out temporaryId) || Parent != null && Parent.TryGetId(name, out temporaryId);
}