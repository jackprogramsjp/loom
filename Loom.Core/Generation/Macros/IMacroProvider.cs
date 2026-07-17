using System.Diagnostics.CodeAnalysis;
using Loom.Core.Diagnostics;
using Loom.Luau.AST;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.Generation.Macros;

internal interface IMacroProvider
{
    public bool Supports(MacroContext context, Type type);
    public bool Supports(MacroContext context, Parsing.AST.Expression expression);

    public bool IsInvocationOnlyMember(string memberName);

    public bool TryProperty(MacroContext context, string name, LuauExpression target, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        expression = null;
        return false;
    }

    public bool TryInvocation(MacroContext context, string name, Parsing.AST.TypeArguments? typeArguments, Call call, [MaybeNullWhen(false)] out LuauExpression expression)
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