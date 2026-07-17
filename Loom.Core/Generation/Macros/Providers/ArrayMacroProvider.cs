using System.Diagnostics.CodeAnalysis;
using Loom.Luau;
using Loom.Luau.AST;
using ArrayType = Loom.Core.TypeChecking.Types.ArrayType;
using Type = Loom.Core.TypeChecking.Types.Type;
using UnaryOperator = Loom.Luau.AST.UnaryOperator;

namespace Loom.Core.Generation.Macros.Providers;

internal sealed class ArrayMacroProvider : IMacroProvider
{
    public bool Supports(MacroContext _, Type type) => type is ArrayType;
    public bool Supports(MacroContext _, Parsing.AST.Expression __) => false;

    public bool IsInvocationOnlyMember(string memberName) => memberName is "join" or "push" or "pop" or "insert" or "remove" or "index_of" or "has";

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

    public bool TryInvocation(
        MacroContext context,
        string name,
        Parsing.AST.TypeArguments? typeArguments,
        Call call,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        var array = MacroContext.GetCallObject(call);
        switch (name)
        {
            case "join":
            {
                expression = LuauFactory.TableCall("concat", [array, ..call.Arguments]);
                return true;
            }
            case "push" or "insert":
            {
                expression = LuauFactory.TableCall("insert", [array, ..call.Arguments]);
                return true;
            }
            case "pop" or "remove":
            {
                expression = LuauFactory.TableCall("remove", [array, ..call.Arguments]);
                return true;
            }
            case "index_of":
            {
                expression = LuauFactory.TableCall("find", [array, ..call.Arguments]);
                return true;
            }
            case "has":
            {
                expression = new BinaryOperator(LuauFactory.TableCall("find", [array, ..call.Arguments]), "~=", new NilLiteral());
                return true;
            }
        }

        expression = null;
        return false;
    }
}