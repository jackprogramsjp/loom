namespace Loom.TypeChecking.Types;

public record ObjectIndexer(bool IsMutable, Type KeyType, Type ValueType);
public record ObjectProperty(bool IsMutable, string Name, Type Type);

public class ObjectType(ObjectIndexer? indexer, List<ObjectProperty> properties) : Type
{
    public ObjectIndexer? Indexer { get; } = indexer;
    public List<ObjectProperty> Properties { get; } = properties;

    public override bool Equals(Type? other)
    {
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

            if (!prop.Type.Equals(otherProp.Type))
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

    public override bool IsAssignableTo(Type other)
    {
        if (other is not ObjectType objectType)
            return false;
        
        var propertyMap = objectType.Properties.ToDictionary(p => p.Name, p => p);
        foreach (var sourceProperty in Properties)
        {
            if (!propertyMap.TryGetValue(sourceProperty.Name, out var property)) continue;
            if (sourceProperty.IsMutable && !property.IsMutable)
                return false;

            if (!sourceProperty.Type.IsAssignableTo(property.Type))
                return false;
        }

        if (objectType.Indexer == null)
            return true;

        if (Indexer == null)
            return false;
            
        if (Indexer.IsMutable || objectType.Indexer.IsMutable)
        {
            if (!Indexer.IsMutable && objectType.Indexer.IsMutable || !Indexer.KeyType.Equals(objectType.Indexer.KeyType) || !Indexer.ValueType.Equals(objectType.Indexer.ValueType))
                return false;
        }
        else
        {
            if (!Indexer.KeyType.IsAssignableTo(objectType.Indexer.KeyType) || !Indexer.ValueType.IsAssignableTo(objectType.Indexer.ValueType))
                return false;
        }
        
        return true;
    }

    public override string ToString()
    {
        if (Indexer == null && Properties.Count == 0)
            return "{}";
        
        var properties = string.Join(", ", Properties.ConvertAll(p => $"{(p.IsMutable ? "mut " : "")}{p.Name}: {p.Type}"));
        var indexer = Indexer != null
            ? $"{(Indexer.IsMutable ? "mut " : "")}[{Indexer.KeyType}]: {Indexer.ValueType}"
            : "";

        return $"{{ {indexer}{(properties.Length > 0 ? ", " : "")}{properties} }}";
    }
}