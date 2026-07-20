using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;

public sealed class InterfaceSymbol(InterfaceDeclaration declaration, string name, bool isSealed)
    : Symbol(declaration, SymbolKind.Interface, name)
{
    public bool IsSealed { get; } = isSealed;
    public List<PropertySymbol> Properties { get; } = [];
    public List<TraitSymbol> Implements { get; } = [];
    public List<Implement> Implementations { get; } = [];

    public PropertySymbol? GetPropertyAtPath(IEnumerable<string> path)
    {
        var property = Properties.FirstOrDefault(p => p.Name == path.FirstOrDefault());
        return property is { PointsTo: { } pointsTo } 
            ? pointsTo.GetPropertyAtPath(path.Skip(1)) 
            : property;
    }

    public override string ToString() => $"InterfaceSymbol({Name}, IsSealed: {IsSealed}, Implements: [{string.Join(", ", Implements.Select(s => s.Name))}])";
}