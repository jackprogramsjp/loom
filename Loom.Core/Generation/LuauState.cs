using Loom.Luau.AST;

namespace Loom.Generation;

internal sealed class LuauState
{
    private LuauScope _scope = new();
    
    public Identifier PushToVariable(string name, LuauExpression expression, LuauType? type = null, bool isConst = true)
    {
        if (expression is Identifier identifier)
            return identifier;

        var id = _scope.AddIdentifier(name);
        Prereq(isConst ? new ConstVariable(id, type, expression) : new LocalVariable(id, type, expression));
        return new Identifier(id);
    }

    public (T, LuauScope) Capture<T>(Func<T> callback)
    {
        T value = default!;
        var scope = CaptureScope(() => value = callback());
        return (value, scope);
    }

    private LuauScope CaptureScope(Action callback)
    {
        var captured = new LuauScope(_scope);
        _scope = captured;
        callback();
        _scope = _scope.Parent!;

        return captured;
    }

    public void Prereq(params LuauStatement[] statements) => _scope.PrereqStatements.AddRange(statements);
    public void Postreq(params LuauStatement[] statements) => _scope.PostreqStatements.AddRange(statements);
}