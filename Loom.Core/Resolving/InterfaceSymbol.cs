using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;

public sealed class InterfaceSymbol(InterfaceDeclaration declaration, string name, bool isSealed)
    : Symbol(declaration, SymbolKind.Interface, name)
{
    public bool IsSealed { get; } = isSealed;
    public override string ToString() => $"InterfaceSymbol({Name}, IsSealed: {IsSealed})";
}