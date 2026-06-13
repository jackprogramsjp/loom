using Loom.Luau.AST;

namespace Loom.Luau;

public static class LuauFactory
{
    public static Call Bit32Call(string name, List<LuauExpression> arguments) => LibraryCall("bit32", name, arguments);
    public static Call MathCall(string name, List<LuauExpression> arguments) => LibraryCall("math", name, arguments);
    public static Call TableCall(string name, List<LuauExpression> arguments) => LibraryCall("table", name, arguments);
    public static Call StringCall(string name, List<LuauExpression> arguments) => LibraryCall("string", name, arguments);

    private static Call LibraryCall(string libraryName, string name, List<LuauExpression> arguments) =>
        new(new PropertyAccess(new Identifier(libraryName), [name]), arguments);

    public static LuauNode EmptyVariable() => new ConstVariable("_", null, new NilLiteral());
}