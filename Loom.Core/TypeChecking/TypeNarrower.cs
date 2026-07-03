using System.Diagnostics.CodeAnalysis;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Text;
using Loom.TypeChecking.Types;
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

    public (TypedFlowState trueState, TypedFlowState falseState) ComputeBranchStates(Expression condition, TypedFlowState current)
    {
        if (condition is BinaryOperator { Operator.Kind: SyntaxKind.EqualsEquals or SyntaxKind.BangEquals } binaryOperator)
            return NarrowByCondition(binaryOperator, current);

        return (new TypedFlowState(current), new TypedFlowState(current));
    }

    private (TypedFlowState trueState, TypedFlowState falseState) NarrowByCondition(BinaryOperator binaryOperator, TypedFlowState current)
    {
        var trueState = new TypedFlowState(current);
        var falseState = new TypedFlowState(current);

        if (TryGetExpressionAndLiteral(binaryOperator.Left, binaryOperator.Right, out var expression, out var literal)
            || TryGetExpressionAndLiteral(binaryOperator.Right, binaryOperator.Left, out expression, out literal))
        {
            ApplyBinaryNarrowing(expression, literal, binaryOperator.Operator.Kind, trueState, falseState);
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

    private bool IsCompileTimeLiteral(Expression expr) =>
        expr is Literal or NameOf || expr is QualifiedName name && semanticModel.GetDeclaringSymbol(name.Identifier)?.Declaration is EnumDeclaration;

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
        switch (expression)
        {
            case PropertyAccess propertyAccess:
            {
                var propertyNames = propertyAccess.Names.ConvertAll(n => n.Name.Text);
                NarrowBaseByProperty(
                    propertyAccess.Expression,
                    literal,
                    propertyNames,
                    literalType,
                    isEquals,
                    trueState,
                    falseState
                );

                break;
            }
            case QualifiedName qualifiedName:
            {
                var propertyNames = qualifiedName.Names.ConvertAll(n => n.Name.Text);
                NarrowBaseByProperty(
                    qualifiedName.Identifier,
                    literal,
                    propertyNames,
                    literalType,
                    isEquals,
                    trueState,
                    falseState
                );

                break;
            }
            case ElementAccess { IndexExpression: Literal { Value: not null and not bool } } elementAccess:
            {
                var indexLiteralType = semanticModel.GetType(elementAccess.IndexExpression);
                NarrowBaseByElement(
                    elementAccess.Expression,
                    indexLiteralType,
                    literalType,
                    isEquals,
                    trueState,
                    falseState
                );

                break;
            }
        }

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

    private void NarrowBaseByProperty(
        Expression baseExpression,
        Expression literalExpression,
        List<string> propertyPath,
        Type literalType,
        bool isEquals,
        TypedFlowState trueState,
        TypedFlowState falseState)
    {
        var baseAddress = GetFlowAddress(baseExpression);
        if (baseAddress == null) return;
        
        var baseType = semanticModel.GetDeclarationType(baseExpression);
        if (baseType is not Types.UnionType union) return;

        var constantValue = semanticModel.GetConstantValue(literalExpression);
        var propertyName = propertyPath.Last();
        var trueMembers = new List<Type>();
        var falseMembers = new List<Type>();

        foreach (var member in union.Types)
        {
            var propertyType = GetMemberPropertyType(member, propertyName);
            if (propertyType == null) continue;

            var matches = constantValue != null && propertyType is Types.LiteralType propertyLiteral
                    ? Equals(propertyLiteral.Value, constantValue)
                    : propertyType.IsAssignableTo(literalType) && literalType.IsAssignableTo(propertyType);

            if (matches)
                trueMembers.Add(member);
            else
                falseMembers.Add(member);
        }

        var trueBaseType = BuildUnionOrNever(trueMembers);
        var falseBaseType = BuildUnionOrNever(falseMembers);

        if (isEquals)
        {
            trueState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(trueBaseType);
            falseState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(falseBaseType);
        }
        else
        {
            trueState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(falseBaseType);
            falseState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(trueBaseType);
        }
    }

    private void NarrowBaseByElement(
        Expression baseExpression,
        Type indexType,
        Type literalType,
        bool isEquals,
        TypedFlowState trueState,
        TypedFlowState falseState)
    {
        var baseAddress = GetFlowAddress(baseExpression);
        if (baseAddress == null) return;

        var baseType = semanticModel.GetDeclarationType(baseExpression);
        if (baseType is not Types.UnionType union) return;

        var trueMembers = new List<Type>();
        var falseMembers = new List<Type>();
        foreach (var member in union.Types)
        {
            var elementType = GetMemberElementType(member, indexType);
            if (elementType == null) continue;

            if (elementType.IsAssignableTo(literalType) && literalType.IsAssignableTo(elementType))
                trueMembers.Add(member);
            else
                falseMembers.Add(member);
        }

        var trueBaseType = BuildUnionOrNever(trueMembers);
        var falseBaseType = BuildUnionOrNever(falseMembers);
        if (isEquals)
        {
            trueState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(trueBaseType);
            falseState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(falseBaseType);
        }
        else
        {
            trueState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(falseBaseType);
            falseState.NarrowedTypes[baseAddress] = TypeSimplifier.Simplify(trueBaseType);
        }
    }

    private static Type? GetMemberPropertyType(Type member, string propertyName)
    {
        if (member is InstantiatedType instantiated)
            member = instantiated.Expand();

        if (member is ObjectType objectType)
        {
            var (bodyType, _) = objectType.GetTypeAtIndex(new Types.LiteralType(propertyName));
            return bodyType?.ValueType;
        }

        if (member is not InterfaceType interfaceType)
            return null;

        var result = interfaceType.ObjectType.GetTypeAtIndex(new Types.LiteralType(propertyName), interfaceType);
        return result.BodyType?.ValueType;
    }

    private static Type? GetMemberElementType(Type member, Type indexType)
    {
        if (member is InstantiatedType instantiated)
            member = instantiated.Expand();

        if (member is ObjectType objectType)
        {
            var (bodyType, _) = objectType.GetTypeAtIndex(indexType);
            return bodyType?.ValueType;
        }

        if (member is not InterfaceType interfaceType)
            return null;

        var result = interfaceType.ObjectType.GetTypeAtIndex(indexType, interfaceType);
        return result.BodyType?.ValueType;
    }

    private static Type BuildUnionOrNever(List<Type> types) =>
        types.Count switch
        {
            0 => PrimitiveType.Never,
            1 => types.First(),
            _ => new Types.UnionType(types)
        };

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