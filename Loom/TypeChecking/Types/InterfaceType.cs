namespace Loom.TypeChecking.Types;

public class InterfaceType(string name, List<TypeParameter> typeParameters, ObjectIndexer? indexer, List<ObjectProperty> properties)
    : ObjectType(indexer, properties)
{
    public string Name { get; } = name;
    public List<TypeParameter> TypeParameters { get; } = typeParameters;

    public override string ToString() => Name + (TypeParameters.Count > 0 ? $"<{string.Join(", ", TypeParameters.ConvertAll(p => p.ToString()))}>" : "");
}