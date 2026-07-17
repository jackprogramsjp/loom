using System.Diagnostics.CodeAnalysis;
using Loom.Core.Generation.Macros.Providers;
using Loom.Core.Parsing.AST;
using Loom.Core.TypeChecking;
using Loom.Core.TypeChecking.Types;
using LiteralType = Loom.Core.TypeChecking.Types.LiteralType;
using Type = Loom.Core.TypeChecking.Types.Type;
using UnionType = Loom.Core.TypeChecking.Types.UnionType;

namespace Loom.Core.Generation.Macros;

internal static class InvocationMacroReference
{
    private static readonly IReadOnlyCollection<IMacroProvider> _providers =
    [
        new NumberMacroProvider(),
        new RangeMacroProvider(),
        new ArrayMacroProvider(),
        new ResultStaticMacroProvider(),
        new IntrinsicGlobalInvocationMacroProvider()
    ];

    public static bool IsValidReferenceContext(Expression expression)
    {
        if (expression.FirstAncestorOfType<ArrayLiteral>() is not null)
            return false;

        for (var node = (Node?)expression; node is not null; node = node.Parent)
        {
            if (node.Parent is Arguments && node.Parent.Parent is Invocation)
                return true;
        }

        return false;
    }

    public static bool IsDirectInvocationCallee(Expression expression) => expression.Parent is Invocation invocation && invocation.Expression == expression;

    public static bool TryClassify(
        MacroContext context,
        Expression expression,
        [NotNullWhen(true)] out IMacroProvider? provider,
        [NotNullWhen(true)] out string? memberName)
    {
        provider = null;
        memberName = null;

        return expression switch
        {
            Identifier identifier => TryClassifyIdentifier(identifier, out provider, out memberName),
            QualifiedName qualified => TryClassifyNamedAccess(
                context,
                qualified.Identifier,
                qualified.Names,
                qualified.Names.Count - 1,
                out provider,
                out memberName
            ),
            PropertyAccess property => TryClassifyNamedAccess(
                context,
                property.Expression,
                property.Names,
                property.Names.Count - 1,
                out provider,
                out memberName
            ),
            ElementAccess element when context.SemanticModel.GetConstantValue(element.IndexExpression) is string name =>
                TryClassifyElementAccess(context, element, name, out provider, out memberName),
            _ => false
        };
    }

    private static bool TryClassifyIdentifier(Identifier identifier, out IMacroProvider? provider, out string? memberName)
    {
        provider = null;
        memberName = null;

        var name = identifier.Name.Text;
        if (name is not ("string" or "number"))
            return false;

        provider = _providers.OfType<IntrinsicGlobalInvocationMacroProvider>().First();
        memberName = name;
        return true;
    }

    private static bool TryClassifyElementAccess(
        MacroContext context,
        ElementAccess element,
        string name,
        out IMacroProvider? provider,
        out string? memberName)
    {
        provider = GetProvider(context, element.Expression);
        if (provider is null || !provider.IsInvocationOnlyMember(name))
        {
            provider = null;
            memberName = null;
            return false;
        }

        memberName = name;
        return true;
    }

    private static bool TryClassifyNamedAccess(
        MacroContext context,
        Expression rootExpression,
        List<DotName> names,
        int memberIndex,
        out IMacroProvider? provider,
        out string? memberName)
    {
        provider = null;
        memberName = null;

        if (names.Count == 0)
            return false;

        var currentType = context.SemanticModel.GetType(rootExpression);
        IMacroProvider? foundProvider = null;
        var foundIndex = -1;

        for (var i = 0; i < names.Count; i++)
        {
            if (GetProvider(context, currentType) is { } macroProvider)
            {
                foundProvider = macroProvider;
                foundIndex = i;
            }

            currentType = GetMemberPropertyType(currentType, names[i].Name.Text);
            if (currentType is null)
                return false;
        }

        if (foundProvider is null || foundIndex != memberIndex)
            return false;

        memberName = names[foundIndex].Name.Text;
        if (!foundProvider.IsInvocationOnlyMember(memberName))
            return false;

        provider = foundProvider;
        return true;
    }

    private static IMacroProvider? GetProvider(MacroContext context, Expression receiver) =>
        GetProvider(context, context.SemanticModel.GetType(receiver)) ?? _providers.FirstOrDefault(provider => provider.Supports(context, receiver));

    private static IMacroProvider? GetProvider(MacroContext context, Type? type) =>
        type is not null ? _providers.FirstOrDefault(provider => provider.Supports(context, type)) : null;

    private static Type? GetMemberPropertyType(Type type, string propertyName)
    {
        if (type is InstantiatedType instantiated)
            type = instantiated.Expand();

        return type switch
        {
            UnionType union => ResolveUnionAccess(propertyName, union),
            ObjectType objectType => objectType.GetTypeAtIndex(new LiteralType(propertyName)).BodyType?.ValueType,
            InterfaceType interfaceType => interfaceType.ObjectType.GetTypeAtIndex(new LiteralType(propertyName), interfaceType).BodyType?.ValueType,
            _ => null
        };
    }

    private static Type? ResolveUnionAccess(string propertyName, UnionType union)
    {
        var members = union.Types
            .Select(type => GetMemberPropertyType(type, propertyName))
            .Where(type => type is not null)
            .Cast<Type>()
            .ToList();

        return members.Count switch
        {
            0 => null,
            1 => members[0],
            _ => TypeSimplifier.Simplify(new UnionType(members))
        };
    }
}