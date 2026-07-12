using Loom.Core.Parsing.AST;

namespace Loom.Core.Resolving;

public sealed class PropertyVariableSymbol(Implement implement, string name, InterfaceSymbol from, bool isMutable = false)
    : Symbol(implement, SymbolKind.PropertyVariable, name, isMutable)
{
    public InterfaceSymbol From { get; } = from;
    
    public override string ToString() => $"PropertyVariableSymbol({Name}, From: {From})";
}