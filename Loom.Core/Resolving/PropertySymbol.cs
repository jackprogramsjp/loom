using System.Diagnostics.CodeAnalysis;
using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;

public class PropertySymbol(PropertyDeclaration propertyDeclaration, InterfaceSymbol? pointsTo, List<AttributeSymbol> attributes)
    : Symbol(propertyDeclaration, SymbolKind.Property, propertyDeclaration.Name.Text, propertyDeclaration.MutKeyword != null)
{
    public InterfaceSymbol? PointsTo { get; } = pointsTo;
    public List<AttributeSymbol> Attributes { get; } = attributes;

    public bool HasIntrinsicAttribute(string name) => TryGetIntrinsicAttribute(name, out _);
    public bool TryGetIntrinsicAttribute(string name, [MaybeNullWhen(false)] out AttributeSymbol attribute)
    {
        attribute = Attributes.Find(a => a.IsIntrinsic && a.Name == name);
        return attribute != null;
    }
}