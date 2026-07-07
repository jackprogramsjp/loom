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

        if (other is not FunctionType functionType
            || ParameterTypes.Count != functionType.ParameterTypes.Count
            || TypeParameters.Count != functionType.TypeParameters.Count)
        {
            return false;
        }

        if (TypeParameters
            .Where((t, i) => functionType.TypeParameters[i].Constraint is { } constraint
                && !(t.Constraint ?? PrimitiveType.Never).IsAssignableTo(constraint)
            )
            .Any())
        {
            return false;
        }

        return !ParameterTypes.Where((t, i) => !functionType.ParameterTypes[i].IsAssignableTo(t)).Any()
            && ReturnType.IsAssignableTo(functionType.ReturnType);
    }

    public override string ToString() =>
        $"fn{(TypeParameters.Count != 0 ? $"<{string.Join(", ", TypeParameters)}>" : "")}({string.Join(", ", ParameterTypes)}): {ReturnType}";
}