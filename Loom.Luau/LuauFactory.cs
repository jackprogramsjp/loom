using Loom.Luau.AST;

namespace Loom.Luau;

public static class LuauFactory
{
    public static Call Bit32Call(string name, List<LuauExpression> arguments) =>
        new(new PropertyAccess(new Identifier("bit32"), [name]), arguments);
}