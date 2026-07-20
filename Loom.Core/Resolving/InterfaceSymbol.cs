using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;

public sealed class InterfaceSymbol(InterfaceDeclaration declaration, string name, bool isSealed, List<InterfaceSymbol>? constraints)
    : Symbol(declaration, SymbolKind.Interface, name)
{
    public bool IsSealed { get; } = isSealed;
    public IReadOnlyList<InterfaceSymbol>? Constraints { get; } = constraints;
    public List<PropertySymbol> Properties { get; } = [];
    public List<TraitSymbol> Implements { get; } = [];
    public List<Implement> Implementations { get; } = [];

    public PropertySymbol? GetPropertyAtPath(List<string> path)
    {
        var firstName = path.FirstOrDefault();
        var property = Properties.FirstOrDefault(p => p.Name == firstName)
            ?? Constraints?.SelectMany(c => c.Properties).FirstOrDefault(p => p.Name == firstName);

        return property is { PointsTo: { } pointsTo }
            ? pointsTo.GetPropertyAtPath(path.Skip(1).ToList())
            : property;
    }

    public override string ToString() =>
        $"InterfaceSymbol({Name}, IsSealed: {IsSealed}, Implements: [{string.Join(", ", Implements.Select(s => s.Name))}], Constraints: [{string.Join(", ", Constraints?.Select(s => s.Name) ?? [])}])";
}