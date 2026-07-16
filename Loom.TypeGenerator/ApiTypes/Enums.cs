#pragma warning disable CS8618
namespace Loom.TypeGenerator.ApiTypes;

internal sealed class Enum
{
    public string Name { get; set; }
    public EnumItem[] Items { get; set; }
}

internal sealed class EnumItem
{
    public string[]? LegacyNames { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}