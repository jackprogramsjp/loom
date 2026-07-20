using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;

public class PropertySymbol(PropertyDeclaration propertyDeclaration, InterfaceSymbol parentInterface)
    : Symbol(propertyDeclaration, SymbolKind.Property, propertyDeclaration.Name.Text, propertyDeclaration.MutKeyword != null)
{
    public InterfaceSymbol ParentInterface { get; } = parentInterface;
}