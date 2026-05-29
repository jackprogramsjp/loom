using Loom.Parsing.AST;

namespace Loom.SemanticAnalysis;

public class Symbol(Node declaringNode, SymbolKind kind, string name)
{
    public Node DeclaringNode { get; } = declaringNode;
    public SymbolKind Kind { get; } = kind;
    public string Name { get; } = name;

    public override bool Equals(object? obj) => obj is Symbol symbol && GetHashCode() == symbol.GetHashCode() && symbol.DeclaringNode.Id == DeclaringNode.Id;

    public override int GetHashCode() => Kind.GetHashCode() + Name.GetHashCode();

    public override string ToString() => $"Symbol({Kind}, {Name})";
}