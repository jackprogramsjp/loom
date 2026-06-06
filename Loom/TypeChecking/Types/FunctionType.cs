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

    public override string ToString() =>
        $"{(TypeParameters.Count != 0 ? $"<{string.Join(", ", TypeParameters)}>" : "")}({string.Join(", ", ParameterTypes)}) -> {ReturnType}";
}