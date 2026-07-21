using System.Diagnostics.CodeAnalysis;
using Loom.Core.Parsing.AST;
using Loom.Core.TypeChecking.Types;
using Loom.Luau;
using Loom.Luau.AST;
using PrimitiveType = Loom.Core.TypeChecking.Types.PrimitiveType;
using PropertyAccess = Loom.Luau.AST.PropertyAccess;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.Generation.Macros.Providers;

internal sealed class InstanceMacroProvider : IMacroProvider
{
    public bool Supports(MacroContext _, Type type) =>
        type is InterfaceType interfaceType && interfaceType.MatchOrMatchConstraint(i => i.Name is "Instance" or "Object");

    public bool Supports(MacroContext context, Parsing.AST.Expression expression) => false;

    public bool IsInvocationOnlyMember(string memberName) => memberName is "is_a";

    public bool TryInvocation(
        MacroContext context,
        string name,
        TypeArguments? typeArguments,
        Call call,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        var instance = MacroContext.GetCallObject(call);
        switch (name)
        {
            case "is_a":
                expression = context.TypeArgumentAsStringCall(name, "IsA", typeArguments, instance);
                return true;
            case "find_first_child_of_class":
                expression = context.TypeArgumentAsStringCall(name, "FindFirstChildOfClass", typeArguments, instance);
                return true;
            case "find_first_child_which_is_a":
                expression = context.TypeArgumentAsStringCall(name, "FindFirstChildWhichIsA", typeArguments, instance);
                return true;
            case "find_first_ancestor_of_class":
                expression = context.TypeArgumentAsStringCall(name, "FindFirstAncestorOfClass", typeArguments, instance);
                return true;
            case "find_first_ancestor_which_is_a":
                expression = context.TypeArgumentAsStringCall(name, "FindFirstAncestorWhichIsA", typeArguments, instance);
                return true;
        }

        expression = null;
        return false;
    }
}