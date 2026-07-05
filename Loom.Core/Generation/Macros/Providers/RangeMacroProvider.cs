using System.Diagnostics.CodeAnalysis;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.TypeChecking;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Generation.Macros.Providers;

internal sealed class RangeMacroProvider : IMacroProvider
{
    public bool Supports(Type type) => type.Equals(Intrinsics.Range);

    public bool TryProperty(MacroContext context, string name, LuauExpression target, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        switch (name)
        {
            case "length":
            {
                var (minimum, maximum) = GetRangeBounds(context, target);
                expression = minimum is NumberLiteral minimumLiteral && maximum is NumberLiteral maximumLiteral
                    ? new NumberLiteral(1 + Math.Abs(maximumLiteral.Value - minimumLiteral.Value))
                    : new BinaryOperator(
                        new NumberLiteral(1),
                        "+",
                        LuauFactory.MathCall(
                            "abs",
                            [new BinaryOperator(maximum, "-", minimum)]
                        )
                    );

                return true;
            }
        }

        expression = null;
        return false;
    }

    public bool TryInvocation(MacroContext context, string name, Call call, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        var range = GetCallObject(call);
        switch (name)
        {
            case "clamp":
                var (minimum, maximum) = GetRangeBounds(context, range);
                var value = call.Arguments.Single();
                expression = value is NumberLiteral valueLiteral && minimum is NumberLiteral minimumLiteral && maximum is NumberLiteral maximumLiteral
                    ? new NumberLiteral(Math.Clamp(valueLiteral.Value, minimumLiteral.Value, maximumLiteral.Value))
                    : LuauFactory.MathCall(
                        "clamp",
                        [value, minimum, maximum]
                    );

                return true;
        }

        expression = null;
        return false;
    }

    public bool TryElementAccess(MacroContext context, ElementAccess access, Type targetType, [MaybeNullWhen(false)] out LuauExpression expression)
    {
        var one = new NumberLiteral(1);
        var length = context.State.PushToVariable("_length", new UnaryOperator("#", access.Target));
        var (minimumValue, maximumValue) = GetRangeBounds(context, access.Index);
        var minimum = LuauFactory.MathCall("clamp", [minimumValue, one, length]);
        var maximum = LuauFactory.MathCall("clamp", [maximumValue, one, length]);
        expression = targetType.IsAssignableTo(PrimitiveType.String)
            ? LuauFactory.StringCall("sub", [access.Target, minimum, maximum])
            : LuauFactory.TableCall("move", [access.Target, minimum, maximum, one, new Table([])]);

        return true;
    }

    private static (LuauExpression Minimum, LuauExpression Maximum) GetRangeBounds(MacroContext context, LuauExpression rangeExpression)
    {
        if (rangeExpression is Parenthesized parenthesized)
            return GetRangeBounds(context, parenthesized.Expression);

        LuauExpression minimum, maximum;
        if (rangeExpression is Table rangeTable)
        {
            var properties = rangeTable.Initializers.OfType<PropertyTableInitializer>().ToList();
            minimum = properties.First(p => p.PropertyName == "minimum").Value;
            maximum = properties.First(p => p.PropertyName == "maximum").Value;
        }
        else
        {
            var range = context.State.PushToVariable("_range", rangeExpression);
            minimum = new PropertyAccess(range, ["minimum"]);
            maximum = new PropertyAccess(range, ["maximum"]);
        }

        return (minimum, maximum);
    }

    private static LuauExpression GetCallObject(Call call) =>
        call.Callee switch
        {
            PropertyAccess propertyAccess => propertyAccess.Target,
            ElementAccess elementAccess => elementAccess.Target,
            var callee => callee
        };
}