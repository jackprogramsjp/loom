namespace Loom.TypeChecking.Types;

public sealed class InstantiatedType(GenericType genericType, List<Type> arguments) : Type
{
    public GenericType GenericType { get; } = genericType;
    public List<Type> Arguments { get; } = arguments;
    private Type? _instantiatedBase;

    public override bool Equals(Type? other) =>
        other is InstantiatedType instantiated
        && GenericType.Equals(instantiated.GenericType)
        && Arguments.Count == instantiated.Arguments.Count
        && Arguments.All(t => instantiated.Arguments.Any(u => u.Equals(t)));

    public override bool IsAssignableTo(Type other) => Expand().IsAssignableTo(other);

    public override string ToString() => GenericType.Declaration.Name.Text + "<" + string.Join(", ", Arguments.ConvertAll(p => p.ToString())) + ">";

    public Type Expand()
    {
        if (_instantiatedBase != null)
            return _instantiatedBase;

        var substitution = new Dictionary<TypeParameter, Type>();
        for (var i = 0; i < GenericType.Parameters.Count; i++)
        {
            var parameter = GenericType.Parameters[i];
            substitution[parameter] = Arguments.ElementAtOrDefault(i) ?? parameter.DefaultType!;
        }

        var baseType = GenericType.UnderlyingType;
        _instantiatedBase = SubstituteTypeParameters(baseType, substitution);

        return _instantiatedBase;
    }

    private Type SubstituteTypeParameters(Type type, Dictionary<TypeParameter, Type> substitution) =>
        type is TypeParameter tp && substitution.TryGetValue(tp, out var substituted)
            ? substituted
            : type switch
            {
                FunctionType functionType => new FunctionType(
                    [],
                    functionType.ParameterTypes.ConvertAll(p => SubstituteTypeParameters(p, substitution)),
                    SubstituteTypeParameters(functionType.ReturnType, substitution)
                ),
                ArrayType arrayType => new ArrayType(SubstituteTypeParameters(arrayType.ElementType, substitution), arrayType.IsMutable),
                InterfaceType interfaceType => SubstituteInterfaceType(interfaceType, substitution),
                ObjectType objectType => SubstituteObjectType(objectType, substitution),
                TypeParameter tp2 when substitution.TryGetValue(tp2, out var s) => s,
                _ => TypeSolver.Transform(type, t => SubstituteTypeParameters(t, substitution))
            };

    private InterfaceType SubstituteInterfaceType(InterfaceType interfaceType, Dictionary<TypeParameter, Type> substitution)
    {
        var substitutedObject = SubstituteObjectType(interfaceType.ObjectType, substitution);
        var substitutedConstraints = interfaceType.Constraints
            .ConvertAll(c => SubstituteTypeParameters(c, substitution))
            .OfType<InterfaceType>()
            .ToList();

        return new InterfaceType(interfaceType.Name, substitutedConstraints, substitutedObject);
    }

    private ObjectType SubstituteObjectType(ObjectType objectType, Dictionary<TypeParameter, Type> substitution)
    {
        ObjectIndexer? substitutedIndexer = null;
        if (objectType.Indexer != null)
        {
            var substitutedKeyType = SubstituteTypeParameters(objectType.Indexer.KeyType, substitution);
            var substitutedValueType = SubstituteTypeParameters(objectType.Indexer.ValueType, substitution);
            substitutedIndexer = new ObjectIndexer(
                objectType.Indexer.IsMutable,
                substitutedKeyType,
                substitutedValueType
            );
        }

        var substitutedProperties = objectType.Properties
            .ConvertAll(p => new ObjectProperty(p.IsMutable, p.Name, SubstituteTypeParameters(p.ValueType, substitution)));

        return new ObjectType(substitutedIndexer, substitutedProperties);
    }
}