namespace Loom.Core.TypeChecking.Types;

public sealed class FunctionType(List<TypeParameter> typeParameters, List<Type> parameterTypes, Type returnType) : Type
{
    public List<TypeParameter> TypeParameters { get; } = typeParameters;
    public List<Type> ParameterTypes { get; } = parameterTypes;
    public List<Type> RequiredParameterTypes { get; } = GetRequiredParameterTypes(parameterTypes);
    public Type ReturnType { get; } = returnType;

    private static List<Type> GetRequiredParameterTypes(List<Type> parameterTypes)
    {
        var cutoffIndex = parameterTypes.Count;
        for (var i = parameterTypes.Count - 1; i >= 0; i--)
        {
            if (!IsNotOptional(parameterTypes[i])) continue;

            cutoffIndex = i + 1;
            break;
        }

        return parameterTypes.Take(cutoffIndex).ToList();
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(TypeParameters.Count);
        hash.Add(GetTypeListHash(TypeParameters));
        hash.Add(ParameterTypes.Count);
        hash.Add(GetTypeListHash(ParameterTypes));
        hash.Add(ReturnType);
        return hash.ToHashCode();
    }

    public override bool Equals(Type? other) =>
        other is FunctionType functionType
        && ListEquals(TypeParameters, functionType.TypeParameters)
        && ListEquals(RequiredParameterTypes, functionType.RequiredParameterTypes)
        && ReturnType.Equals(functionType.ReturnType);

    public override bool IsAssignableTo(Type other)
    {
        if (base.IsAssignableTo(other))
            return true;

        if (other is not FunctionType functionType
            || ParameterTypes.Count != functionType.ParameterTypes.Count
            || TypeParameters.Count != functionType.TypeParameters.Count)
            return false;

        if (TypeParameters
            .Where((t, i) => functionType.TypeParameters[i].Constraint is { } constraint
                && !(t.Constraint ?? PrimitiveType.Never).IsAssignableTo(constraint)
            )
            .Any())
            return false;

        return !ParameterTypes.Where((t, i) => !functionType.ParameterTypes[i].IsAssignableTo(t)).Any()
            && ReturnType.IsAssignableTo(functionType.ReturnType);
    }

    public override string ToString() =>
        $"fn{(TypeParameters.Count != 0 ? $"<{string.Join(", ", TypeParameters)}>" : "")}({string.Join(", ", ParameterTypes)}): {ReturnType}";
}