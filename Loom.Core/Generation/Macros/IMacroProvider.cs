using System.Diagnostics.CodeAnalysis;
using Loom.Luau.AST;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Generation.Macros;

internal interface IMacroProvider
{
    public bool Supports(Type type);
    public bool Supports(Parsing.AST.Expression expression);

    public bool TryProperty(MacroContext context, string name, LuauExpression target, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        expression = null;
        return false;
    }

    public bool TryInvocation(MacroContext context, string name, Call call, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        expression = null;
        return false;
    }

    public bool TryElementAccess(MacroContext context, ElementAccess access, Type targetType, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        expression = null;
        return false;
    }
}