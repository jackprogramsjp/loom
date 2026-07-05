using System.Diagnostics.CodeAnalysis;
using Loom.Luau;
using Loom.Luau.AST;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Generation.Macros.Providers;

internal sealed class NumberMacroProvider : IMacroProvider
{
    public bool Supports(Type type) => type.IsAssignableTo(PrimitiveType.Number);

    public bool TryElementAccess(MacroContext context, ElementAccess access, Type targetType, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        if (targetType.IsAssignableTo(PrimitiveType.String))
        {
            expression = LuauFactory.StringCall(
                "sub",
                [access.Target, access.Index, access.Index]
            );
            return true;
        }
        
        expression = null;
        return false;
    }
}