global using TypeParameterSubstitution = System.Collections.Generic.Dictionary<Loom.TypeChecking.Types.TypeParameter, Loom.TypeChecking.Types.Type>;
using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public sealed partial class TypeChecker
{
    private TypeParameterSubstitution? ResolveExplicitInterfaceTypeArguments(InterfaceInvocation node, GenericType generic)
    {
        var arguments = node.TypeArguments!.ArgumentsList.ConvertAll(Visit);
        if (!CheckGenericArity(node.TypeArguments, generic.Parameters, arguments, $"Interface '{generic}'"))
            return null;

        var resolved = FillGenericArguments(generic.Parameters, arguments);
        var substitution = new TypeParameterSubstitution();
        for (var i = 0; i < generic.Parameters.Count; i++)
            substitution[generic.Parameters[i]] = resolved[i];

        return substitution;
    }

    private static List<Type> FillGenericArguments(List<Types.TypeParameter> parameters, List<Type> given) =>
        parameters.Select((t, i) => i < given.Count ? given[i] : t.DefaultType ?? PrimitiveType.Unknown).ToList();

    private TypeParameterSubstitution? ResolveTypeArguments(
        Invocation invocation,
        Types.FunctionType functionType,
        List<Type> argumentTypes,
        Type? expectedReturnType)
    {
        var substitution = new TypeParameterSubstitution();
        if (invocation.TypeArguments != null)
        {
            var explicitArguments = invocation.TypeArguments.ArgumentsList.ConvertAll(Visit);
            if (!CheckGenericArity(invocation, functionType.TypeParameters, explicitArguments, "Function"))
                return null;

            for (var i = 0; i < explicitArguments.Count; i++)
                substitution[functionType.TypeParameters[i]] = explicitArguments[i];
        }
        else
        {
            var inferred = TypeInferrer.InferFunctionTypeArguments(functionType, argumentTypes, expectedReturnType);
            foreach (var (tp, type) in inferred)
                substitution[tp] = type;
        }

        foreach (var tp in functionType.TypeParameters)
            if (substitution.TryGetValue(tp, out var substitutedType) && tp.Constraint != null)
                if (!CheckTypeParameterConstraints(invocation, substitutedType, tp))
                    return null;

        return substitution;
    }

    private Type InstantiateGenericType(Node node, TypeArguments? typeArguments, GenericType genericType)
    {
        var arguments = typeArguments?.ArgumentsList.ConvertAll(Visit) ?? [];
        if (!CheckGenericArity(typeArguments ?? node, genericType.Parameters, arguments, $"Type '{genericType}'"))
            return BindType(node, PrimitiveType.Never);

        var fullArguments = FillGenericArguments(genericType.Parameters, arguments);
        for (var i = 0; i < genericType.Parameters.Count; i++)
        {
            var parameter = genericType.Parameters[i];
            var argument = fullArguments[i];
            if (parameter.Constraint == null) continue;
            CheckTypeParameterConstraints(node, argument, parameter);
        }

        var instantiated = new InstantiatedType(genericType, fullArguments);
        return BindType(node, instantiated);
    }

    private bool CheckTypeParameterConstraints(Node node, Type type, Types.TypeParameter parameter)
    {
        if (parameter.Constraint == null) return true;
        if (type.IsAssignableTo(parameter.Constraint)) return true;

        _diagnostics.Error(
            node,
            InternalCodes.ConstraintViolation,
            $"Type '{type}' does not satisfy constraint '{parameter.Constraint}' for type parameter '{parameter.Name}'."
        );

        return false;
    }

    private bool CheckGenericArity(Node node, List<Types.TypeParameter> parameters, List<Type> arguments, string genericKind)
    {
        var minimum = parameters.Count(p => p.DefaultType == null);
        var maximum = parameters.Count;
        var arityDisplay = minimum == maximum ? minimum.ToString() : $"{minimum}-{maximum}";
        if (arguments.Count >= minimum && arguments.Count <= maximum)
            return true;

        _diagnostics.Error(
            node,
            InternalCodes.GenericArity,
            $"{genericKind} expects {arityDisplay} type argument{(minimum != maximum || maximum != 1 ? "s" : "")}, but {arguments.Count} were provided."
        );

        return false;
    }

    private static ObjectType SubstituteObjectType(ObjectType objectType, TypeParameterSubstitution substitution)
    {
        var newProperties = objectType.Properties.ConvertAll(property => new ObjectProperty(
                property.IsMutable,
                property.Name,
                SubstituteTypeParameters(property.ValueType, substitution)
            )
        );

        ObjectIndexer? newIndexer = null;
        if (objectType.Indexer != null)
        {
            newIndexer = new ObjectIndexer(
                objectType.Indexer.IsMutable,
                SubstituteTypeParameters(objectType.Indexer.KeyType, substitution),
                SubstituteTypeParameters(objectType.Indexer.ValueType, substitution)
            );
        }

        return new ObjectType(newIndexer, newProperties);
    }

    private static List<Type> SubstituteTypeParameters(List<Type> types, TypeParameterSubstitution substitution) =>
        types.ConvertAll(t => SubstituteTypeParameters(t, substitution));

    private static Type SubstituteTypeParameters(Type type, TypeParameterSubstitution substitution) =>
        type is Types.TypeParameter tp && substitution.TryGetValue(tp, out var substituted)
            ? substituted
            : TypeSolver.Transform(type, t => t is Types.TypeParameter tp2 && substitution.TryGetValue(tp2, out var s) ? s : SubstituteTypeParameters(t, substitution));
}