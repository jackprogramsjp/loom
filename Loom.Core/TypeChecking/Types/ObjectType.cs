namespace Loom.Core.TypeChecking.Types;

public record ObjectBodyType(bool IsMutable, Type ValueType);

public record ObjectIndexer(bool IsMutable, Type KeyType, Type ValueType)
    : ObjectBodyType(IsMutable, ValueType);

public record ObjectProperty(bool IsMutable, string Name, Type ValueType)
    : ObjectBodyType(IsMutable, ValueType);

public class ObjectType(ObjectIndexer? indexer, List<ObjectProperty> properties) : Type
{
    public static readonly ObjectType Empty = new(null, []);

    public ObjectIndexer? Indexer { get; internal set; } = indexer;
    public List<ObjectProperty> Properties { get; } = properties;

    public Type KeyUnion()
    {
        var propertyKeyType = PropertyKeyUnion();
        if (Indexer == null)
            return propertyKeyType;

        var types = new List<Type> { Indexer.KeyType };
        if (propertyKeyType is UnionType propertyKeyUnion)
            types.AddRange(propertyKeyUnion.Types);
        else
            types.Add(propertyKeyType);

        return TypeSimplifier.Simplify(new UnionType(types));
    }

    public Type ValueUnion()
    {
        var propertyValueType = PropertyUnion();
        if (Indexer == null)
            return propertyValueType;

        var types = new List<Type> { Indexer.ValueType };
        if (propertyValueType is UnionType propertyUnion)
            types.AddRange(propertyUnion.Types);
        else
            types.Add(propertyValueType);

        return TypeSimplifier.Simplify(new UnionType(types));
    }

    private Type ValueUnionForKeyType(Type keyType)
    {
        var matches = (
                from property in Properties
                let literal = new LiteralType(property.Name)
                where literal.IsAssignableTo(keyType)
                select property.ValueType
            )
            .ToList();

        if (Indexer != null)
            matches.Add(Indexer.ValueType);

        return TypeSimplifier.Simplify(new UnionType(matches));
    }

    public Type PropertyKeyUnion() => TypeSimplifier.Simplify(new UnionType(Properties.ConvertAll(Type (p) => new LiteralType(p.Name))));
    public Type PropertyUnion() => TypeSimplifier.Simplify(new UnionType(Properties.ConvertAll(p => p.ValueType)));

    public ObjectProperty? GetProperty(string name) => Properties.Find(p => p.Name == name);

    public (ObjectBodyType? BodyType, string CannotFindReason) GetTypeAtIndex(Type indexType, Type? self = null)
    {
        if (indexType is TypeParameter parameter)
        {
            if (parameter.Constraint == null)
                return Indexer != null && indexType.IsAssignableTo(Indexer.KeyType)
                    ? (Indexer, "")
                    : (null, $" Type parameter '{parameter.Name}' is unconstrained.");

            indexType = parameter.Constraint;
        }
        
        if (indexType is LiteralType { Value: string name })
        {
            var property = GetProperty(name);
            if (property != null)
                return (property, "");

            if (Indexer == null)
                return (null, $" Property '{name}' does not exist on type '{self ?? this}'.");
        }
        
        if (Properties.Count > 0 && indexType.Equals(PropertyKeyUnion()))
        {
            return (new ObjectIndexer(
                IsMutable: Properties.Any(p => p.IsMutable),
                KeyType: indexType,
                ValueType: ValueUnionForKeyType(indexType)
            ), "");
        }

        if (Indexer != null && indexType.IsAssignableTo(Indexer.KeyType))
            return (Indexer, "");

        return Indexer == null
            ? (null, "")
            : (null, $" Index is not of type '{Indexer.KeyType}'.");
    }
    
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Properties.Count);
        foreach (var property in Properties.OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            hash.Add(property.Name);
            hash.Add(property.IsMutable);
        }
        // hash.Add(Indexer != null);
        return hash.ToHashCode();
    }

    public override bool Equals(Type? other) =>
        GuardedEquals(
            this,
            other,
            () =>
            {
                if (ReferenceEquals(this, other)) return true;
                if (other is not ObjectType objectType)
                    return false;

                if (Properties.Count != objectType.Properties.Count)
                    return false;

                var otherProps = objectType.Properties.ToDictionary(p => p.Name, p => p);
                foreach (var prop in Properties)
                {
                    if (!otherProps.TryGetValue(prop.Name, out var otherProp))
                        return false;

                    if (prop.IsMutable != otherProp.IsMutable)
                        return false;

                    if (!prop.ValueType.Equals(otherProp.ValueType))
                        return false;
                }

                if (Indexer == null)
                    return objectType.Indexer == null;

                if (objectType.Indexer == null)
                    return false;

                return Indexer.KeyType.Equals(objectType.Indexer.KeyType)
                    && Indexer.ValueType.Equals(objectType.Indexer.ValueType)
                    && Indexer.IsMutable == objectType.Indexer.IsMutable;
            }
        );

    public override bool IsAssignableTo(Type other)
    {
        if (base.IsAssignableTo(other))
            return true;

        if (other is not ObjectType objectType)
            return false;

        if (Properties.Count < objectType.Properties.Count)
            return false;

        var sourcePropertyMap = Properties.ToDictionary(p => p.Name, p => p);
        foreach (var targetProperty in objectType.Properties)
        {
            if (!sourcePropertyMap.TryGetValue(targetProperty.Name, out var sourceProperty))
                return false;

            if (sourceProperty.IsMutable && !targetProperty.IsMutable)
                return false;

            if (!sourceProperty.ValueType.IsAssignableTo(targetProperty.ValueType))
                return false;
        }

        if (objectType.Indexer == null)
            return true;

        if (Indexer == null)
            return false;

        if (Indexer.IsMutable || objectType.Indexer.IsMutable)
        {
            if (!Indexer.IsMutable && objectType.Indexer.IsMutable
                || !Indexer.KeyType.Equals(objectType.Indexer.KeyType)
                || !Indexer.ValueType.Equals(objectType.Indexer.ValueType))
            {
                return false;
            }
        }
        else
        {
            if (!Indexer.KeyType.IsAssignableTo(objectType.Indexer.KeyType)
                || !Indexer.ValueType.IsAssignableTo(objectType.Indexer.ValueType))
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        if (Indexer == null && Properties.Count == 0)
            return "object";

        var properties = string.Join(", ", Properties.ConvertAll(p => $"{(p.IsMutable ? "mut " : "")}{p.Name}: {p.ValueType}"));
        var indexer = Indexer != null
            ? $"{(Indexer.IsMutable ? "mut " : "")}[{Indexer.KeyType}]: {Indexer.ValueType}"
            : "";

        return $"{{ {indexer}{(Indexer != null && properties.Length > 0 ? ", " : "")}{properties} }}";
    }
}