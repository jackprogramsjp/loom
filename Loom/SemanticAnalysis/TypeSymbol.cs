using Loom.Parsing.AST;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.SemanticAnalysis;

public class TypeSymbol(Node declaringNode, Type type, string name)
    : Symbol(declaringNode, SymbolKind.Type, name)
{
    public Type Type { get; } = type;
    
    public override bool Equals(object? obj) => base.Equals(obj) && obj is TypeSymbol symbol && Type.Equals(symbol.Type);

    public override int GetHashCode() => base.GetHashCode();

    public override string ToString() => $"TypeSymbol({Name}, {Type})";
}