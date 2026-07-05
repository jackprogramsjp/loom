using System.Diagnostics.CodeAnalysis;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Generation.Macros.Providers;

internal sealed class ArrayMacroProvider : IMacroProvider
{
    public bool Supports(Type type) => type is ArrayType;

    public bool TryProperty(MacroContext context, string name, LuauExpression target, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        switch (name)
        {
            case "length":
            {
                expression = new UnaryOperator("#", target);
                return true;
            }
        }

        expression = null;
        return false;
    }

    public bool TryInvocation(MacroContext context, string name, Call call, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        var array = MacroContext.GetCallObject(call);
        switch (name)
        {
            case "join":
            {
                expression = LuauFactory.TableCall("concat", [array, ..call.Arguments]);
                return true;
            }
        }

        expression = null;
        return false;
    }
}