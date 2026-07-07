using System.Diagnostics.CodeAnalysis;
using Loom.Generation.Macros.Providers;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using ElementAccess = Loom.Parsing.AST.ElementAccess;
using Identifier = Loom.Parsing.AST.Identifier;
using LiteralType = Loom.TypeChecking.Types.LiteralType;
using PropertyAccess = Loom.Parsing.AST.PropertyAccess;
using Type = Loom.TypeChecking.Types.Type;
using UnionType = Loom.TypeChecking.Types.UnionType;

namespace Loom.Generation.Macros;

internal sealed class MacroExpander(SemanticModel semanticModel, LuauState state)
{
    private readonly MacroContext _context = new(semanticModel, state);
    private static readonly IMacroProvider[] _providers =
    [
        new NumberMacroProvider(), new RangeMacroProvider(), new ArrayMacroProvider(), new ResultStaticMacroProvider(), new GlobalInvocationMacroProvider()
    ];

    public bool TryGetInvocationMacro(Invocation invocation, Call luauCall, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        expression = null;
        if (!TryDecomposeInvocationTarget(invocation.Expression, luauCall.Callee, out var provider, out var member))
            return false;

        return provider.TryInvocation(_context, member, luauCall, out expression);
    }

    public bool TryGetElementAccessMacro(ElementAccess access, Luau.AST.ElementAccess luauAccess, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        if (TryGetEnumConstant(access, out expression))
            return true;

        var targetType = semanticModel.GetType(access.Expression);
        if (GetProvider(access.IndexExpression) is { } provider && provider.TryElementAccess(_context, luauAccess, targetType, out expression))
            return true;

        return semanticModel.GetConstantValue(access.IndexExpression) is string name
            && TryGetNamedAccessMacro(access.Expression, name, luauAccess.Target, out expression);
    }

    public bool TryGetQualifiedNameMacro(QualifiedName name, Luau.AST.PropertyAccess luauAccess, [MaybeNullWhen(false)] out LuauExpression expression) =>
        TryRewriteNamedAccess(name, name.Identifier, name.Names, luauAccess, out expression);

    public bool TryGetPropertyAccessMacro(PropertyAccess access, Luau.AST.PropertyAccess luauAccess, [MaybeNullWhen(false)] out LuauExpression expression) =>
        TryRewriteNamedAccess(access, access.Expression, access.Names, luauAccess, out expression);

    private bool TryDecomposeInvocationTarget(
        Expression expression,
        LuauExpression target,
        [MaybeNullWhen(false)] out IMacroProvider provider,
        [MaybeNullWhen(false)] out string memberName)
    {
        switch (expression)
        {
            case Identifier identifier:
            {
                provider = GetProvider(expression);
                memberName = identifier.Name.Text;
                return provider != null;
            }

            case QualifiedName qualified:
            {
                if (!TryResolveMacroReceiver(
                        qualified.Identifier,
                        qualified.Names,
                        target,
                        out provider,
                        out _,
                        out var macroIndex
                    ))
                {
                    memberName = null;
                    return false;
                }

                memberName = qualified.Names[macroIndex].Name.Text;
                return true;
            }

            case PropertyAccess property:
            {
                if (!TryResolveMacroReceiver(
                        property.Expression,
                        property.Names,
                        target,
                        out provider,
                        out _,
                        out var macroIndex
                    ))
                {
                    memberName = null;
                    return false;
                }

                memberName = property.Names[macroIndex].Name.Text;
                return true;
            }

            case ElementAccess element
                when semanticModel.GetConstantValue(element.IndexExpression) is string name:
            {
                provider = GetProvider(element.Expression);
                memberName = name;
                return provider != null;
            }
        }

        provider = null;
        memberName = null;
        return false;
    }

    private bool TryRewriteNamedAccess(
        Expression access,
        Expression receiver,
        List<DotName> names,
        Luau.AST.PropertyAccess luauAccess,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        if (TryGetEnumConstant(access, out expression))
            return true;

        if (!TryResolveMacroReceiver(
                receiver,
                names,
                luauAccess.Target,
                out var provider,
                out var target,
                out var macroIndex
            ))
        {
            expression = null;
            return false;
        }

        if (!provider.TryProperty(_context, names[macroIndex].Name.Text, target, out expression))
            return false;

        if (macroIndex + 1 < names.Count)
            expression = new Luau.AST.PropertyAccess(expression, luauAccess.Names.Skip(macroIndex + 1).ToList());

        return true;
    }

    private bool TryResolveMacroReceiver(
        Expression rootExpression,
        List<DotName> names,
        LuauExpression rootTarget,
        [MaybeNullWhen(false)] out IMacroProvider provider,
        [MaybeNullWhen(false)] out LuauExpression target,
        out int macroIndex)
    {
        provider = null;
        target = null;
        macroIndex = -1;

        var currentType = semanticModel.GetType(rootExpression);
        var currentTarget = rootTarget;
        for (var i = 0; i < names.Count; i++)
        {
            if (GetProvider(currentType) is { } p)
            {
                provider = p;
                target = currentTarget;
                macroIndex = i;
            }

            currentType = GetMemberPropertyType(currentType, names[i].Name.Text);
            if (currentType == null)
                break;

            currentTarget = new Luau.AST.PropertyAccess(currentTarget, [names[i].Name.Text]);
        }

        return provider != null;
    }

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
            .Select(t => GetMemberPropertyType(t, propertyName))
            .Where(t => t != null)
            .Cast<Type>()
            .ToList();

        return members.Count switch
        {
            0 => null,
            1 => members[0],
            _ => TypeSimplifier.Simplify(new UnionType(members))
        };
    }

    private bool TryGetNamedAccessMacro(Expression objectExpression, string name, LuauExpression target, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        if (GetProvider(objectExpression) is { } provider)
            return provider.TryProperty(_context, name, target, out expression);

        expression = null;
        return false;
    }

    private bool TryGetEnumConstant(Expression expression, [MaybeNullWhen(false)] out LuauExpression constantValue)
    {
        constantValue = null;
        var value = semanticModel.GetConstantValue(expression);
        if (value is not (long or int or double or string))
            return false;

        constantValue = value is string s ? new StringLiteral(s) : new NumberLiteral(Convert.ToDouble(value));
        return true;
    }

    private IMacroProvider? GetProvider(Expression receiver) =>
        GetProvider(semanticModel.GetType(receiver)) ?? _providers.FirstOrDefault(provider => provider.Supports(receiver));

    private static IMacroProvider? GetProvider(Type type) => _providers.FirstOrDefault(provider => provider.Supports(type));
}