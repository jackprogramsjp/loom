using Loom.Parsing.AST;
using Loom.TypeChecking.Types;
using ArrayType = Loom.TypeChecking.Types.ArrayType;
using FunctionType = Loom.TypeChecking.Types.FunctionType;
using IntersectionType = Loom.TypeChecking.Types.IntersectionType;
using OptionalType = Loom.TypeChecking.Types.OptionalType;
using Type = Loom.TypeChecking.Types.Type;
using TypeParameter = Loom.TypeChecking.Types.TypeParameter;
using UnionType = Loom.TypeChecking.Types.UnionType;

namespace Loom.TypeChecking;

internal sealed class TypeInferrer(TypeChecker checker)
{
    public TypeParameterSubstitution? InferInterfaceTypeArguments(InterfaceInvocation node, GenericType generic, InterfaceType underlying)
    {
        var objectType = underlying.ObjectType;
        var pairs = new List<(Type parameterType, Type argumentType)>();
        foreach (var initializer in node.Body.Initializers)
        {
            switch (initializer)
            {
                case InterfaceInvocationPropertyInitializer propInit:
                {
                    var prop = objectType.GetProperty(propInit.Name.Text);
                    if (prop == null) continue;
                    var argType = checker.Check(propInit.Expression);
                    pairs.Add((prop.ValueType, argType));
                    break;
                }
                case InterfaceInvocationIndexInitializer when objectType.Indexer == null: continue;
                case InterfaceInvocationIndexInitializer idxInit:
                {
                    var keyArg = checker.Check(idxInit.IndexExpression);
                    var valueArg = checker.Check(idxInit.Expression);
                    pairs.Add((objectType.Indexer.KeyType, keyArg));
                    pairs.Add((objectType.Indexer.ValueType, valueArg));
                    break;
                }
            }
        }

        var inferred = new TypeParameterSubstitution();
        var visited = new HashSet<(Type, Type)>();
        foreach (var (parameterType, argumentType) in pairs)
            TryInferTypes(parameterType, argumentType, inferred, visited);

        var substitution = new TypeParameterSubstitution();
        foreach (var typeParameter in generic.Parameters)
        {
            if (inferred.TryGetValue(typeParameter, out var inferredType))
            {
                substitution[typeParameter] = inferredType;
            }
            else if (typeParameter.DefaultType != null)
            {
                substitution[typeParameter] = typeParameter.DefaultType;
            }
            else
            {
                checker.ReportCannotInfer(node, typeParameter);
                return null;
            }
        }

        return substitution;
    }

    public TypeParameterSubstitution? InferFunctionTypeArguments(
        FunctionType functionType,
        List<Type> argumentTypes,
        Node errorNode)
    {
        var inferred = new TypeParameterSubstitution();
        var visited = new HashSet<(Type, Type)>();
        for (var i = 0; i < Math.Min(functionType.ParameterTypes.Count, argumentTypes.Count); i++)
            TryInferTypes(functionType.ParameterTypes[i], argumentTypes[i], inferred, visited);

        var substitution = new TypeParameterSubstitution();
        foreach (var typeParameter in functionType.TypeParameters)
        {
            if (inferred.TryGetValue(typeParameter, out var inferredType))
            {
                substitution[typeParameter] = inferredType;
            }
            else if (typeParameter.DefaultType != null)
            {
                substitution[typeParameter] = typeParameter.DefaultType;
            }
            else
            {
                checker.ReportCannotInfer(errorNode, typeParameter);
                return null;
            }
        }

        return substitution;
    }
    
    private static bool TryInferTypes(Type parameterType, Type argumentType, TypeParameterSubstitution inferredTypes, HashSet<(Type, Type)> visitedPairs)
    {
        parameterType = ExpandAliases(parameterType);
        argumentType = ExpandAliases(argumentType);

        if (!visitedPairs.Add((parameterType, argumentType)))
            return true;

        if (TryMatchGenericTypes(parameterType, argumentType, inferredTypes, visitedPairs, out var genericResult))
            return genericResult;

        return (parameterType, argumentType) switch
        {
            (TypeParameter typeParameter, _) => BindTypeParameter(typeParameter, argumentType, inferredTypes),
            (ArrayType parameterArray, ArrayType argumentArray) => TryInferTypes(
                parameterArray.ElementType,
                argumentArray.ElementType,
                inferredTypes,
                visitedPairs
            ),
            (OptionalType parameterOptional, OptionalType argumentOptional) => TryInferTypes(
                parameterOptional.NonNullableType,
                argumentOptional.NonNullableType,
                inferredTypes,
                visitedPairs
            ),
            (OptionalType parameterOptional, _) => TryInferTypes(parameterOptional.NonNullableType, argumentType, inferredTypes, visitedPairs),
            (ObjectType parameterObject, ObjectType argumentObject) => MatchObjectTypes(parameterObject, argumentObject, inferredTypes, visitedPairs),
            (InterfaceType parameterInterface, InterfaceType argumentInterface) => TryInferTypes(
                parameterInterface.ObjectType,
                argumentInterface.ObjectType,
                inferredTypes,
                visitedPairs
            ),
            (FunctionType parameterFunction, FunctionType argumentFunction) => MatchFunctionTypes(
                parameterFunction,
                argumentFunction,
                inferredTypes,
                visitedPairs
            ),
            (UnionType parameterUnion, UnionType argumentUnion) when parameterUnion.Types.Count == argumentUnion.Types.Count => MatchUnionTypes(
                parameterUnion,
                argumentUnion,
                inferredTypes,
                visitedPairs
            ),
            (UnionType parameterUnion, _) => TryInferFromUnion(parameterUnion, argumentType, inferredTypes),
            (IntersectionType parameterIntersection, IntersectionType argumentIntersection) when parameterIntersection.Types.Count
                == argumentIntersection.Types.Count => MatchIntersectionTypes(parameterIntersection, argumentIntersection, inferredTypes, visitedPairs),
            (IntersectionType parameterIntersection, _) => TryInferFromIntersection(parameterIntersection, argumentType, inferredTypes),
            _ => parameterType.Equals(argumentType) || argumentType.IsAssignableTo(parameterType)
        };
    }

    private static bool TryInferFromUnion(UnionType union, Type argumentType, TypeParameterSubstitution inferredTypes)
    {
        if (argumentType is UnionType)
            return false;

        var typeParams = union.Types.OfType<TypeParameter>().ToList();
        return typeParams.Count == 1 && BindTypeParameter(typeParams[0], argumentType, inferredTypes);
    }

    private static bool TryInferFromIntersection(IntersectionType union, Type argumentType, TypeParameterSubstitution inferredTypes)
    {
        if (argumentType is IntersectionType)
            return false;

        var typeParams = union.Types.OfType<TypeParameter>().ToList();
        return typeParams.Count == 1 && BindTypeParameter(typeParams[0], argumentType, inferredTypes);
    }

    private static bool BindTypeParameter(TypeParameter typeParameter, Type argumentType, TypeParameterSubstitution inferredTypes)
    {
        if (inferredTypes.TryGetValue(typeParameter, out var existingType))
            return existingType.Equals(argumentType);

        inferredTypes[typeParameter] = argumentType;
        return true;
    }

    private static bool MatchObjectTypes(
        ObjectType parameterObject,
        ObjectType argumentObject,
        TypeParameterSubstitution inferredTypes,
        HashSet<(Type, Type)> visitedPairs)
    {
        foreach (var parameterProperty in parameterObject.Properties)
        {
            var argumentProperty = argumentObject.GetProperty(parameterProperty.Name);
            if (argumentProperty == null)
                return false;

            if (!TryInferTypes(parameterProperty.ValueType, argumentProperty.ValueType, inferredTypes, visitedPairs))
                return false;
        }

        if (parameterObject.Indexer != null && argumentObject.Indexer != null)
        {
            return TryInferTypes(parameterObject.Indexer.KeyType, argumentObject.Indexer.KeyType, inferredTypes, visitedPairs)
                && TryInferTypes(parameterObject.Indexer.ValueType, argumentObject.Indexer.ValueType, inferredTypes, visitedPairs);
        }

        return parameterObject.Indexer == null;
    }

    private static bool MatchFunctionTypes(
        FunctionType parameterFunction,
        FunctionType argumentFunction,
        TypeParameterSubstitution inferredTypes,
        HashSet<(Type, Type)> visitedPairs)
    {
        if (parameterFunction.ParameterTypes.Count != argumentFunction.ParameterTypes.Count)
            return false;

        return !parameterFunction.ParameterTypes.Where((t, index) => !TryInferTypes(t, argumentFunction.ParameterTypes[index], inferredTypes, visitedPairs)).Any()
            && TryInferTypes(parameterFunction.ReturnType, argumentFunction.ReturnType, inferredTypes, visitedPairs);
    }

    private static bool MatchUnionTypes(
        UnionType parameterUnion,
        UnionType argumentUnion,
        TypeParameterSubstitution inferredTypes,
        HashSet<(Type, Type)> visitedPairs) =>
        !parameterUnion.Types.Where((t, index) => !TryInferTypes(t, argumentUnion.Types[index], inferredTypes, visitedPairs)).Any();

    private static bool MatchIntersectionTypes(
        IntersectionType parameterIntersection,
        IntersectionType argumentIntersection,
        TypeParameterSubstitution inferredTypes,
        HashSet<(Type, Type)> visitedPairs) =>
        !parameterIntersection.Types.Where((t, index) => !TryInferTypes(t, argumentIntersection.Types[index], inferredTypes, visitedPairs)).Any();

    private static bool TryMatchGenericTypes(
        Type parameterType,
        Type argumentType,
        TypeParameterSubstitution inferredTypes,
        HashSet<(Type, Type)> visitedPairs,
        out bool result)
    {
        result = false;
        var parameterGenericInfo = GetGenericTypeAndArguments(parameterType);
        var argumentGenericInfo = GetGenericTypeAndArguments(argumentType);
        if (parameterGenericInfo.Generic == null || argumentGenericInfo.Generic == null)
            return false;

        if (!parameterGenericInfo.Generic.Declaration.Equals(argumentGenericInfo.Generic.Declaration))
            return false;

        switch (parameterGenericInfo.Arguments.Count)
        {
            case > 0 when argumentGenericInfo.Arguments.Count > 0:
            {
                for (var index = 0; index < Math.Min(parameterGenericInfo.Arguments.Count, argumentGenericInfo.Arguments.Count); index++)
                {
                    if (!TryInferTypes(parameterGenericInfo.Arguments[index], argumentGenericInfo.Arguments[index], inferredTypes, visitedPairs))
                        return false;
                }

                break;
            }
            case > 0 when argumentGenericInfo.Arguments.Count == 0:
            {
                for (var index = 0; index < Math.Min(parameterGenericInfo.Arguments.Count, argumentGenericInfo.Generic.Parameters.Count); index++)
                {
                    if (!TryInferTypes(argumentGenericInfo.Generic.Parameters[index], parameterGenericInfo.Arguments[index], inferredTypes, visitedPairs))
                        return false;
                }

                break;
            }
            case 0 when argumentGenericInfo.Arguments.Count > 0:
            {
                for (var index = 0; index < Math.Min(parameterGenericInfo.Generic.Parameters.Count, argumentGenericInfo.Arguments.Count); index++)
                {
                    if (!TryInferTypes(parameterGenericInfo.Generic.Parameters[index], argumentGenericInfo.Arguments[index], inferredTypes, visitedPairs))
                        return false;
                }

                break;
            }
        }

        result = true;
        return true;
    }

    private static Type ExpandAliases(Type type) =>
        TypeSolver.Transform(
            type,
            candidateType => candidateType is InstantiatedType { GenericType.Declaration: TypeAlias } instantiated
                ? instantiated.Expand()
                : candidateType
        );

    private static (GenericType? Generic, List<Type> Arguments) GetGenericTypeAndArguments(Type type) =>
        type switch
        {
            InstantiatedType instantiated => (instantiated.GenericType, instantiated.Arguments),
            GenericType generic => (generic, []),
            _ => (null, [])
        };
}