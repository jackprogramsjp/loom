using System.Diagnostics.CodeAnalysis;
using Loom.Luau.AST;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.Generation.Macros.Providers;

internal sealed class GlobalInvocationMacroProvider : IMacroProvider
{
    public bool Supports(Type _) => false;
    public bool Supports(Parsing.AST.Expression expression) => expression is Parsing.AST.Identifier;

    public bool TryInvocation(MacroContext context, string name, Call call, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        switch (name)
        {
            case "string":
                expression = new Call(new Identifier("tostring"), call.Arguments);
                return true;
            case "number":
                expression = new Call(new Identifier("tonumber"), call.Arguments);
                return true;
        }

        expression = null;
        return false;
    }
}