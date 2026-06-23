using Loom.Parsing.AST;

namespace Loom.SemanticAnalysis;

public sealed class InterfaceSymbol(InterfaceDeclaration declaration, string name, bool isSealed, bool isIntrinsic = false)
    : Symbol(declaration, SymbolKind.Interface, name, false, isIntrinsic)
{
    public bool IsSealed { get; } = isSealed;
    public override string ToString() => $"InterfaceSymbol({Name}, IsSealed: {IsSealed})";
}