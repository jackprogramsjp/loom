using System.Diagnostics.CodeAnalysis;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Text;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public sealed class TypeNarrower(SemanticModel semanticModel)
{
    public bool TryGetNarrowedType(Expression expression, TypedFlowState current, [MaybeNullWhen(false)] out Type narrowedType)
    {
        if (GetFlowAddress(expression) is { } address && current.NarrowedTypes.TryGetValue(address, out var narrowed))
        {
            narrowedType = narrowed;
            semanticModel.TypeSolver.SetType(expression, narrowed);
            return true;
        }

        narrowedType = null;
        return false;
    }

    public (TypedFlowState trueState, TypedFlowState falseState) ComputeBranchStates(Expression condition, TypedFlowState current) =>
        condition is BinaryOperator { Operator.Kind: SyntaxKind.EqualsEquals or SyntaxKind.BangEquals } binaryOperator
            ? NarrowByCondition(binaryOperator, current)
            : (new TypedFlowState(current), new TypedFlowState(current));

    private (TypedFlowState trueState, TypedFlowState falseState) NarrowByCondition(BinaryOperator binaryOperator, TypedFlowState current)
    {
        var trueState = new TypedFlowState(current);
        var falseState = new TypedFlowState(current);
        if (TryGetExpressionAndLiteral(binaryOperator.Left, binaryOperator.Right, out var expr, out var literal)
            || TryGetExpressionAndLiteral(binaryOperator.Right, binaryOperator.Left, out expr, out literal))
        {
            ApplyBinaryNarrowing(expr, literal, binaryOperator.Operator.Kind, trueState, falseState);
        }

        return (trueState, falseState);
    }

    private bool TryGetExpressionAndLiteral(
        Expression expr1,
        Expression expr2,
        [MaybeNullWhen(false)] out Expression expr,
        [MaybeNullWhen(false)] out Expression literal)
    {
        if (!IsCompileTimeLiteral(expr2))
        {
            expr = null;
            literal = null;
            return false;
        }

        expr = expr1;
        literal = expr2;
        return true;
    }

    private bool IsCompileTimeLiteral(Expression expr)
    {
        Console.WriteLine(expr is QualifiedName ? semanticModel.GetSymbol(expr) : null);
        return expr is Literal or NameOf || expr is QualifiedName name && semanticModel.GetDeclaringSymbol(name.Identifier)?.Declaration is EnumDeclaration;
    }

    private void ApplyBinaryNarrowing(
        Expression expression,
        Expression literal,
        SyntaxKind operatorKind,
        TypedFlowState trueState,
        TypedFlowState falseState)
    {
        var address = GetFlowAddress(expression);
        if (address == null) return;

        var baseType = semanticModel.GetType(expression);
        var literalType = semanticModel.GetType(literal);
        var isNone = literal is Literal { Value: null };
        var isEquals = operatorKind == SyntaxKind.EqualsEquals;

        if (isNone)
        {
            if (isEquals)
            {
                trueState.NarrowedTypes[address] = literalType;
                falseState.NarrowedTypes[address] = baseType.NonNullable();
            }
            else
            {
                trueState.NarrowedTypes[address] = baseType.NonNullable();
                falseState.NarrowedTypes[address] = literalType;
            }
        }
        else
        {
            if (isEquals)
            {
                trueState.NarrowedTypes[address] = literalType;
                falseState.NarrowedTypes[address] = RemoveType(baseType, literalType);
            }
            else
            {
                trueState.NarrowedTypes[address] = RemoveType(baseType, literalType);
                falseState.NarrowedTypes[address] = literalType;
            }
        }
    }

    private static Type RemoveType(Type source, Type toRemove)
    {
        if (source.Equals(toRemove))
            return PrimitiveType.Never;
        
        if (source is not Types.UnionType union)
            return source;

        var remaining = union.Types.Where(t => !toRemove.IsAssignableTo(t)).ToList();
        return remaining.Count switch
        {
            0 => PrimitiveType.Never,
            1 => remaining.First(),
            _ => new Types.UnionType(remaining)
        };
    }

    private TypedFlowAddress? GetFlowAddress(Expression expr) =>
        expr switch
        {
            Identifier identifier => GetIdentifierFlowAddress(identifier),
            QualifiedName qualifiedName => BuildFieldChain(qualifiedName.Identifier, qualifiedName.Names),
            PropertyAccess propertyAccess => BuildFieldChain(propertyAccess.Expression, propertyAccess.Names),
            ElementAccess elementAccess => GetElementAddress(elementAccess),
            _ => null
        };

    private TypedFlowAddress? BuildFieldChain(Expression baseExpr, List<DotName> dotNames)
    {
        var address = GetFlowAddress(baseExpr);
        return address == null
            ? null
            : dotNames.Aggregate(address, (current, name) => TypedFlowAddress.Field(current, name.Name.Text));
    }

    private TypedFlowAddress? GetElementAddress(ElementAccess elementAccess)
    {
        if (GetFlowAddress(elementAccess.Expression) is not { } baseAddress)
            return null;

        if (elementAccess.IndexExpression is Literal { Value: not null and not bool } literal)
            return TypedFlowAddress.Element(baseAddress, literal.Value);

        return null;
    }

    private TypedFlowAddress? GetIdentifierFlowAddress(Identifier identifier)
    {
        var symbol = semanticModel.GetSymbol(identifier);
        return symbol != null ? TypedFlowAddress.Variable(symbol) : null;
    }
}