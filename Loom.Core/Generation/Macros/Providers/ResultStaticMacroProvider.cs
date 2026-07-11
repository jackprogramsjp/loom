using System.Diagnostics.CodeAnalysis;
using Loom.Core.TypeChecking.Types;
using Loom.Luau.AST;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.Generation.Macros.Providers;

internal sealed class ResultStaticMacroProvider : IMacroProvider
{
    public bool Supports(Type type) => type is InterfaceType { Name: "ResultStatic" };
    public bool Supports(Parsing.AST.Expression _) => false;

    public bool TryInvocation(MacroContext context, string name, Call call, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        switch (name)
        {
            case "ok":
                expression = CreateResultConstructor(call, isOk: true);
                return true;
            case "err":
                expression = CreateResultConstructor(call, isOk: false);
                return true;
        }

        expression = null;
        return false;
    }

    private static Table CreateResultConstructor(Call call, bool isOk) =>
        new([new PropertyTableInitializer("ok", new BooleanLiteral(isOk)), new PropertyTableInitializer(isOk ? "value" : "error", call.Arguments.Single())]);
}