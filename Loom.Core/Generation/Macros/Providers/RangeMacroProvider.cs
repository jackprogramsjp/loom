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
                var (minimumExpression, maximumExpression) = GetRangeBounds(context, target);
                expression = MacroContext.TryComputeConstantArithmetic(minimumExpression, out var minimum)
                    && MacroContext.TryComputeConstantArithmetic(maximumExpression, out var maximum)
                        ? new NumberLiteral(1 + Math.Abs(maximum - minimum))
                        : new BinaryOperator(
                            new NumberLiteral(1),
                            "+",
                            LuauFactory.MathCall(
                                "abs",
                                [new BinaryOperator(maximumExpression, "-", minimumExpression)]
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
        var range = MacroContext.GetCallObject(call);
        switch (name)
        {
            case "clamp":
                var (minimumExpression, maximumExpression) = GetRangeBounds(context, range);
                var value = call.Arguments.Single();
                expression = MacroContext.TryComputeConstantArithmetic(value, out var valueConstant)
                    && MacroContext.TryComputeConstantArithmetic(minimumExpression, out var minimum)
                    && MacroContext.TryComputeConstantArithmetic(maximumExpression, out var maximum)
                        ? new NumberLiteral(Math.Clamp(valueConstant, minimum, maximum))
                        : LuauFactory.MathClampCall(value, minimumExpression, maximumExpression);

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
        var minimum = LuauFactory.MathClampCall(minimumValue, one, length);
        var maximum = LuauFactory.MathClampCall(maximumValue, one, length);
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
}