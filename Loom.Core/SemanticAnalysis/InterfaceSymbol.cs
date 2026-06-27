using Loom.Parsing.AST;

namespace Loom.SemanticAnalysis;

public sealed class InterfaceSymbol(InterfaceDeclaration declaration, string name, bool isSealed)
    : Symbol(declaration, SymbolKind.Interface, name)
{
    public bool IsSealed { get; } = isSealed;
    public override string ToString() => $"InterfaceSymbol({Name}, IsSealed: {IsSealed})";
}