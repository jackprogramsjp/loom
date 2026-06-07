namespace Loom.TypeChecking.Types;

public sealed class FunctionType(List<TypeParameter> typeParameters, List<Type> parameterTypes, Type returnType) : Type
{
    public List<TypeParameter> TypeParameters { get; } = typeParameters;
    public List<Type> ParameterTypes { get; } = parameterTypes;
    public Type ReturnType { get; } = returnType;

    public override bool Equals(Type? other) =>
        other is FunctionType functionType
        && ListEquals(TypeParameters, functionType.TypeParameters)
        && ListEquals(ParameterTypes, functionType.ParameterTypes)
        && ReturnType.Equals(functionType.ReturnType);

    public override bool IsAssignableTo(Type other)
    {
        if (base.IsAssignableTo(other))
            return true;

        if (other is not FunctionType target || ParameterTypes.Count != target.ParameterTypes.Count || TypeParameters.Count != target.TypeParameters.Count)
            return false;

        if (TypeParameters.Where((t, i) => !t.Equals(target.TypeParameters[i])).Any())
            return false;

        return !ParameterTypes.Where((t, i) => !target.ParameterTypes[i].IsAssignableTo(t)).Any()
            && ReturnType.IsAssignableTo(target.ReturnType);
    }

    public override string ToString() =>
        $"{(TypeParameters.Count != 0 ? $"<{string.Join(", ", TypeParameters)}>" : "")}({string.Join(", ", ParameterTypes)}) -> {ReturnType}";
}