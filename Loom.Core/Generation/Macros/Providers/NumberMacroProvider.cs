using System.Diagnostics.CodeAnalysis;
using Loom.Luau;
using Loom.Luau.AST;
using PrimitiveType = Loom.Core.TypeChecking.Types.PrimitiveType;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.Generation.Macros.Providers;

internal sealed class NumberMacroProvider : IMacroProvider
{
    public bool Supports(Type type) => type.IsAssignableTo(PrimitiveType.Number);
    public bool Supports(Parsing.AST.Expression _) => false;

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