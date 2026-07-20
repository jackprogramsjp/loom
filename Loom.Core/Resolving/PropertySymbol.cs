using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;

public class PropertySymbol(PropertyDeclaration propertyDeclaration, InterfaceSymbol? pointsTo, List<AttributeSymbol> attributes)
    : Symbol(propertyDeclaration, SymbolKind.Property, propertyDeclaration.Name.Text, propertyDeclaration.MutKeyword != null)
{
    public InterfaceSymbol? PointsTo { get; } = pointsTo;
    public List<AttributeSymbol> Attributes { get; } = attributes;
}