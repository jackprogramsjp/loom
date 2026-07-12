using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;


public sealed class InterfaceSymbol(InterfaceDeclaration declaration, string name, bool isSealed)
    : Symbol(declaration, SymbolKind.Interface, name)
{
    public sealed record InterfaceProperty(string Name, bool IsMutable);
    
    public bool IsSealed { get; } = isSealed;
    public IReadOnlySet<InterfaceProperty> Properties { get; } = declaration.Body?.Members.OfType<PropertyDeclaration>().Select(p => new InterfaceProperty(p.Name.Text, p.MutKeyword != null)).ToHashSet() ?? [];
    public List<TraitSymbol> Implements { get; } = [];
    public List<Implement> Implementations { get; } = [];

    public override string ToString() => $"InterfaceSymbol({Name}, IsSealed: {IsSealed}, Implements: [{string.Join(", ", Implements.Select(s => s.Name))}])";
}