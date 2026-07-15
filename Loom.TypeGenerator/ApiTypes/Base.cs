#pragma warning disable CS8618
namespace Loom.TypeGenerator.ApiTypes;

internal sealed class Serialization
{
    public bool CanLoad { get; init; }
    public bool CanSave { get; init; }
}

internal sealed class ValueType
{
    public string Category { get; init; }
    public string Name { get; init; }
}

internal sealed class Security
{
    public string Read { get; init; }
    public string Write { get; init; }
}