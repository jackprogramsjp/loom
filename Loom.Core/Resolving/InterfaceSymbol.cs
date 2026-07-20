using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;


public sealed class InterfaceSymbol(InterfaceDeclaration declaration, string name, bool isSealed, IReadOnlySet<PropertySymbol> propertySymbols)
    : Symbol(declaration, SymbolKind.Interface, name)
{
    public bool IsSealed { get; } = isSealed;
    public IReadOnlySet<PropertySymbol> Properties { get; } = propertySymbols;
    public List<TraitSymbol> Implements { get; } = [];
    public List<Implement> Implementations { get; } = [];

    public override string ToString() => $"InterfaceSymbol({Name}, IsSealed: {IsSealed}, Implements: [{string.Join(", ", Implements.Select(s => s.Name))}])";
}