using System.Diagnostics.CodeAnalysis;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Text;
using Loom.TypeChecking.Types;
using LiteralType = Loom.TypeChecking.Types.LiteralType;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;
using UnionType = Loom.TypeChecking.Types.UnionType;

namespace Loom.TypeChecking;

public sealed class TypeNarrower
{
    public sealed record BranchStates(TypedFlowState True, TypedFlowState False);

    private readonly Literal _trueLiteral = new(TokenFactory.Keyword(SyntaxKind.TrueLiteral), true);
    private readonly SemanticModel _semanticModel;

    public TypeNarrower(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
        _semanticModel.TypeSolver.SetType(_trueLiteral, new LiteralType(true));
    }

    public bool TryGetNarrowedType(Expression expression, TypedFlowState current, [MaybeNullWhen(false)] out Type narrowedType)
    {
        if (GetFlowAddress(expression) is { } address && current.NarrowedTypes.TryGetValue(address, out var narrowed))
        {
            narrowedType = narrowed;
            _semanticModel.TypeSolver.SetType(expression, narrowed);
            return true;
        }

        if (TryResolveViaNarrowedPrefix(expression, current) is { } resolved)
        {
            narrowedType = resolved;
            _semanticModel.TypeSolver.SetType(expression, resolved);
            return true;
        }

        narrowedType = null;
        return false;
    }

    public BranchStates ComputeBranchStates(Expression condition, TypedFlowState current) =>
        condition switch
        {
            BinaryOperator { Operator.Kind: SyntaxKind.EqualsEquals or SyntaxKind.BangEquals } binary => NarrowEquality(binary, current),
            BinaryOperator { Operator.Kind: SyntaxKind.AmpersandAmpersand or SyntaxKind.AmpersandAmpersandEquals } and => NarrowLogicalAnd(and, current),
            BinaryOperator { Operator.Kind: SyntaxKind.PipePipe or SyntaxKind.PipePipeEquals } or => NarrowLogicalOr(or, current),
            UnaryOperator { Operator.Kind: SyntaxKind.Bang } not => NarrowLogicalNot(not, current),
            Parenthesized p => ComputeBranchStates(p.Expression, current),
            _ => NarrowBooleanCondition(condition, current)
        };

    private BranchStates NarrowBooleanCondition(Expression expression, TypedFlowState current)
    {
        var type = GetBaseExpressionType(expression, current);
        if (type == null || !type.IsAssignableTo(PrimitiveType.Bool))
            return new BranchStates(new TypedFlowState(current), new TypedFlowState(current));

        var trueState = new TypedFlowState(current);
        var falseState = new TypedFlowState(current);
        ApplyBinaryNarrowing(
            expression,
            _trueLiteral,
            SyntaxKind.EqualsEquals,
            current,
            trueState,
            falseState
        );

        return new BranchStates(trueState, falseState);
    }

    private BranchStates NarrowEquality(BinaryOperator binaryOperator, TypedFlowState current)
    {
        var trueState = new TypedFlowState(current);
        var falseState = new TypedFlowState(current);

        if (TryGetExpressionAndLiteral(binaryOperator.Left, binaryOperator.Right, out var expression, out var literal)
            || TryGetExpressionAndLiteral(binaryOperator.Right, binaryOperator.Left, out expression, out literal))
        {
            ApplyBinaryNarrowing(
                expression,
                literal,
                binaryOperator.Operator.Kind,
                current,
                trueState,
                falseState
            );
        }

        return new BranchStates(trueState, falseState);
    }

    private BranchStates NarrowLogicalAnd(BinaryOperator andOp, TypedFlowState current)
    {
        var (leftTrue, leftFalse) = ComputeBranchStates(andOp.Left, current);
        var (rightTrue, _) = ComputeBranchStates(andOp.Right, leftTrue);
        var falseState = MergeStates(leftFalse, ApplyBranchState(andOp.Right, leftTrue, useTrue: false));
        return new BranchStates(rightTrue, falseState);
    }

    private BranchStates NarrowLogicalOr(BinaryOperator orOp, TypedFlowState current)
    {
        var (leftTrue, leftFalse) = ComputeBranchStates(orOp.Left, current);
        var (_, rightFalse) = ComputeBranchStates(orOp.Right, leftFalse);
        var trueState = MergeStates(leftTrue, ApplyBranchState(orOp.Right, leftFalse, useTrue: true));
        return new BranchStates(trueState, rightFalse);
    }

    private BranchStates NarrowLogicalNot(UnaryOperator notOp, TypedFlowState current)
    {
        var (trueState, falseState) = ComputeBranchStates(notOp.Operand, current);
        return new BranchStates(falseState, trueState);
    }

    private TypedFlowState ApplyBranchState(Expression expr, TypedFlowState state, bool useTrue = false)
    {
        var (trueState, falseState) = ComputeBranchStates(expr, state);
        return useTrue ? trueState : falseState;
    }

    private static TypedFlowState MergeStates(TypedFlowState a, TypedFlowState b)
    {
        var result = new TypedFlowState(a);
        foreach (var key in a.NarrowedTypes.Keys.Concat(b.NarrowedTypes.Keys).Distinct())
        {
            var aType = ResolveEffectiveType(key, a);
            var bType = ResolveEffectiveType(key, b);

            if (aType != null && bType != null)
                result.NarrowedTypes[key] = TypeSimplifier.Simplify(new UnionType([aType, bType]));
            else
                result.NarrowedTypes.Remove(key);
        }

        return result;
    }

    private static Type? ResolveEffectiveType(TypedFlowAddress address, TypedFlowState state)
    {
        if (state.NarrowedTypes.TryGetValue(address, out var direct))
            return direct;

        return address switch
        {
            { Parent: not null, FieldName: not null } => ResolveEffectiveType(address.Parent, state) is { } parentType
                ? GetMemberPropertyType(parentType, address.FieldName)
                : null,
            { Parent: not null, ElementIndex: not null } => ResolveEffectiveType(address.Parent, state) is { } parentType
                ? GetMemberElementType(parentType, new LiteralType(address.ElementIndex))
                : null,
            _ => null
        };
    }

    private bool TryGetExpressionAndLiteral(
        Expression expr1,
        Expression expr2,
        [MaybeNullWhen(false)] out Expression expression,
        [MaybeNullWhen(false)] out Expression literal)
    {
        if (!_semanticModel.IsCompileTimeConstant(expr2))
        {
            expression = null;
            literal = null;
            return false;
        }

        expression = expr1;
        literal = expr2;
        return true;
    }

    private void ApplyBinaryNarrowing(
        Expression expression,
        Expression literal,
        SyntaxKind operatorKind,
        TypedFlowState currentState,
        TypedFlowState trueState,
        TypedFlowState falseState)
    {
        var address = GetFlowAddress(expression);
        if (address == null) return;

        var baseType = _semanticModel.GetType(expression);
        var literalType = _semanticModel.GetType(literal);
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
                    currentState,
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
                    currentState,
                    trueState,
                    falseState
                );

                break;
            }
            case ElementAccess { IndexExpression: Literal { Value: not null and not bool } } elementAccess:
            {
                var indexLiteralType = _semanticModel.GetType(elementAccess.IndexExpression);
                NarrowBaseByElement(
                    elementAccess.Expression,
                    indexLiteralType,
                    literalType,
                    isEquals,
                    currentState,
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
        TypedFlowState currentState,
        TypedFlowState trueState,
        TypedFlowState falseState)
    {
        var baseAddress = GetFlowAddress(baseExpression);
        if (baseAddress == null) return;

        var baseType = GetBaseExpressionType(baseExpression, currentState);
        if (baseType == null) return;

        var unionAddress = baseAddress;
        var currentType = baseType;
        var pathIndex = 0;
        while (currentType is not UnionType && pathIndex < propertyPath.Count)
        {
            var name = propertyPath[pathIndex];
            var nextAddress = TypedFlowAddress.Field(unionAddress, name);
            currentType = currentState.NarrowedTypes.TryGetValue(nextAddress, out var narrowedStep)
                ? narrowedStep
                : GetMemberPropertyType(currentType, name);

            if (currentType == null) return;
            unionAddress = nextAddress;
            pathIndex++;
        }

        if (currentType is not UnionType union) return;

        var remainingPath = propertyPath.Skip(pathIndex).ToList();
        var constantValue = _semanticModel.GetConstantValue(literalExpression);
        var trueMembers = new List<Type>();
        var falseMembers = new List<Type>();
        foreach (var member in union.Types)
        {
            var propertyType = GetTypeAtPath(member, remainingPath);
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
            trueState.NarrowedTypes[unionAddress] = TypeSimplifier.Simplify(trueBaseType);
            falseState.NarrowedTypes[unionAddress] = TypeSimplifier.Simplify(falseBaseType);
        }
        else
        {
            trueState.NarrowedTypes[unionAddress] = TypeSimplifier.Simplify(falseBaseType);
            falseState.NarrowedTypes[unionAddress] = TypeSimplifier.Simplify(trueBaseType);
        }
    }

    private void NarrowBaseByElement(
        Expression baseExpression,
        Type indexType,
        Type literalType,
        bool isEquals,
        TypedFlowState currentState,
        TypedFlowState trueState,
        TypedFlowState falseState)
    {
        var baseAddress = GetFlowAddress(baseExpression);
        if (baseAddress == null) return;

        var baseType = GetBaseExpressionType(baseExpression, currentState);
        if (baseType is not UnionType union) return;

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

    private Type? GetBaseExpressionType(Expression expression, TypedFlowState currentState)
    {
        if (TryGetNarrowedType(expression, currentState, out var narrowed))
            return narrowed;

        switch (expression)
        {
            case Identifier:
                return _semanticModel.GetDeclarationType(expression) ?? _semanticModel.GetType(expression);

            case PropertyAccess property:
            {
                var parent = GetBaseExpressionType(property.Expression, currentState);
                return parent == null
                    ? null
                    : GetTypeAtPath(parent, property.Names.ConvertAll(n => n.Name.Text));
            }

            case QualifiedName qualified:
            {
                var parent = GetBaseExpressionType(qualified.Identifier, currentState);
                return parent == null
                    ? null
                    : GetTypeAtPath(parent, qualified.Names.ConvertAll(n => n.Name.Text));
            }

            case ElementAccess { IndexExpression: Literal { Value: not (null or bool) } } element:
            {
                var parent = GetBaseExpressionType(element.Expression, currentState);
                if (parent == null)
                    return null;

                var indexType = _semanticModel.GetType(element.IndexExpression);
                return GetMemberElementType(parent, indexType);
            }

            default:
                return _semanticModel.GetType(expression);
        }
    }

    private Type? TryResolveViaNarrowedPrefix(Expression expression, TypedFlowState current)
    {
        Expression baseExpression;
        List<string> path;
        switch (expression)
        {
            case QualifiedName qualifiedName:
                baseExpression = qualifiedName.Identifier;
                path = qualifiedName.Names.ConvertAll(n => n.Name.Text);
                break;
            case PropertyAccess propertyAccess:
                baseExpression = propertyAccess.Expression;
                path = propertyAccess.Names.ConvertAll(n => n.Name.Text);
                break;
            default:
                return null;
        }

        if (GetFlowAddress(baseExpression) is not { } address)
            return null;

        var narrowedBase = current.NarrowedTypes.GetValueOrDefault(address);
        var narrowedIndex = narrowedBase != null ? 0 : -1;
        for (var i = 0; i < path.Count; i++)
        {
            address = TypedFlowAddress.Field(address, path[i]);
            if (!current.NarrowedTypes.TryGetValue(address, out var narrowed)) continue;

            narrowedBase = narrowed;
            narrowedIndex = i + 1;
        }

        if (narrowedIndex < 0 || narrowedBase == null)
            return null;

        var remainingPath = path.Skip(narrowedIndex).ToList();
        return GetTypeAtPath(narrowedBase, remainingPath);
    }

    private static Type? GetTypeAtPath(Type type, List<string> path)
    {
        var final = type;
        foreach (var part in path)
        {
            final = GetMemberPropertyType(final, part);
            if (final == null)
                return null;
        }

        return final;
    }

    private static Type? GetMemberPropertyType(Type member, string propertyName)
    {
        if (member is InstantiatedType instantiated)
            member = instantiated.Expand();

        switch (member)
        {
            case UnionType union:
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

            case ObjectType objectType:
                var (bodyType, _) = objectType.GetTypeAtIndex(new LiteralType(propertyName));
                return bodyType?.ValueType;

            case InterfaceType interfaceType:
                var result = interfaceType.ObjectType.GetTypeAtIndex(new LiteralType(propertyName), interfaceType);
                return result.BodyType?.ValueType;
        }

        return null;
    }

    private static Type? GetMemberElementType(Type member, Type indexType)
    {
        if (member is InstantiatedType instantiated)
            member = instantiated.Expand();

        switch (member)
        {
            case UnionType union:
            {
                var members = union.Types
                    .Select(t => GetMemberElementType(t, indexType))
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
            
            case ObjectType objectType:
                var (bodyType, _) = objectType.GetTypeAtIndex(indexType);
                return bodyType?.ValueType;
            
            case InterfaceType interfaceType:
                var result = interfaceType.ObjectType.GetTypeAtIndex(indexType, interfaceType);
                return result.BodyType?.ValueType;
        }

        return null;
    }

    private static Type BuildUnionOrNever(List<Type> types) =>
        types.Count switch
        {
            0 => PrimitiveType.Never,
            1 => types.First(),
            _ => new UnionType(types)
        };

    private static Type RemoveType(Type source, Type toRemove)
    {
        if (source.Equals(toRemove))
            return PrimitiveType.Never;

        if (source.Equals(PrimitiveType.Bool) && toRemove is LiteralType { Value: bool value })
            return new LiteralType(!value);

        if (source is not UnionType union)
            return source;

        var remaining = union.Types.Where(t => !toRemove.IsAssignableTo(t)).ToList();
        return remaining.Count switch
        {
            0 => PrimitiveType.Never,
            1 => remaining.First(),
            _ => new UnionType(remaining)
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
            : dotNames.Select(name => name.Name.Text).Aggregate(address, TypedFlowAddress.Field);
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
        var symbol = _semanticModel.GetSymbol(identifier);
        return symbol != null ? TypedFlowAddress.Variable(symbol) : null;
    }
}