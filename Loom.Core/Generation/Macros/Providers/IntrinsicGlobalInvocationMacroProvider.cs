using System.Diagnostics.CodeAnalysis;
using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Luau;
using Loom.Luau.AST;
using Identifier = Loom.Luau.AST.Identifier;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.Generation.Macros.Providers;

internal sealed class IntrinsicGlobalInvocationMacroProvider : IMacroProvider
{
    public bool Supports(MacroContext _, Type __) => false;

    public bool Supports(MacroContext context, Expression expression) =>
        expression is Parsing.AST.Identifier && context.SemanticModel.GetSymbol(expression) is { IsIntrinsic: true };

    public bool IsInvocationOnlyMember(string memberName) => memberName is "string" or "number" or "log" or "new_instance" or "get_service";

    public bool TryInvocation(
        MacroContext context,
        string name,
        TypeArguments? typeArguments,
        Call call,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        switch (name)
        {
            case "get_service":
                var serviceName = GetStringFromOnlyTypeArgument(context, typeArguments, name);
                expression = LuauFactory.LibraryCall("game", ["GetService"], [serviceName], true);

                return true;
            case "new_instance":
                var instanceName = GetStringFromOnlyTypeArgument(context, typeArguments, name);
                expression = LuauFactory.LibraryCall("Instance", ["new"], [instanceName]);

                return true;
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

    private static StringLiteral GetStringFromOnlyTypeArgument(MacroContext context, TypeArguments? typeArguments, string fnName)
    {
        var typeName = typeArguments!.ArgumentsList[0];
        var instanceType = context.SemanticModel.GetType(typeName);
        if (instanceType is TypeChecking.Types.TypeParameter)
            context.Diagnostics.Error(
                typeName,
                InternalCodes.AbstractTypeParameterInMacro,
                $"Cannot use type parameter '{typeName}' with '{fnName}::<T>()' macro."
            );

        return new StringLiteral(typeName.ToString());
    }
}