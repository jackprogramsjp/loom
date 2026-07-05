using System.Diagnostics.CodeAnalysis;
using Loom.Generation.Macros.Providers;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using ElementAccess = Loom.Parsing.AST.ElementAccess;
using PropertyAccess = Loom.Parsing.AST.PropertyAccess;

namespace Loom.Generation.Macros;

internal sealed class MacroExpander(SemanticModel semanticModel, LuauState state)
{
    private readonly MacroContext _context = new(semanticModel, state);
    private static readonly IMacroProvider[] _providers =
    [
        new NumberMacroProvider(), new RangeMacroProvider(), new ArrayMacroProvider(), new ResultStaticMacroProvider()
    ];

    public bool TryGetInvocationMacro(
        Invocation invocation,
        Call luauCall,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        expression = null;
        return TryDecomposeMemberAccess(invocation.Expression, out var receiver, out var memberName)
            && TryGetInvocationMacro(receiver, memberName, luauCall, out expression);
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

    private bool TryRewriteNamedAccess(
        Expression access,
        Expression receiver,
        List<DotName> names,
        Luau.AST.PropertyAccess luauAccess,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        if (TryGetEnumConstant(access, out expression))
            return true;

        if (!TryGetNamedAccessMacro(receiver, names[0].Name.Text, luauAccess.Target, out expression))
            return false;

        if (names.Count > 1)
            expression = new Luau.AST.PropertyAccess(expression, luauAccess.Names.Skip(1).ToList());

        return true;
    }

    private bool TryGetNamedAccessMacro(
        Expression objectExpression,
        string name,
        LuauExpression target,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        if (GetProvider(objectExpression) is { } provider)
            return provider.TryProperty(_context, name, target, out expression);

        expression = null;
        return false;
    }

    private bool TryGetInvocationMacro(
        Expression objectExpression,
        string name,
        Call call,
        [MaybeNullWhen(false)] out LuauExpression expression)
    {
        if (GetProvider(objectExpression) is { } provider)
            return provider.TryInvocation(_context, name, call, out expression);

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

    private bool TryDecomposeMemberAccess(Expression expression, [MaybeNullWhen(false)] out Expression receiver, [MaybeNullWhen(false)] out string memberName)
    {
        switch (expression)
        {
            case QualifiedName qualified:
                receiver = qualified.Identifier;
                memberName = qualified.Names.Last().Name.Text;
                return true;

            case PropertyAccess property:
                receiver = property.Expression;
                memberName = property.Names.Last().Name.Text;
                return true;

            case ElementAccess elementAccess
                when semanticModel.GetConstantValue(elementAccess.IndexExpression) is string name:
                receiver = elementAccess.Expression;
                memberName = name;
                return true;

            default:
                receiver = null;
                memberName = null;
                return false;
        }
    }

    private IMacroProvider? GetProvider(Expression receiver)
    {
        var type = semanticModel.GetType(receiver);
        return _providers.FirstOrDefault(provider => provider.Supports(type));
    }
}