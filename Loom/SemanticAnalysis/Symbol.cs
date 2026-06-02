using Loom.Parsing.AST;

namespace Loom.SemanticAnalysis;

public class Symbol(Node declaration, SymbolKind kind, string name, bool mutable = false)
{
    public Node Declaration { get; } = declaration;
    public SymbolKind Kind { get; } = kind;
    public string Name { get; } = name;
    public bool Mutable { get; } = mutable;

    public override bool Equals(object? obj) => obj is Symbol symbol && GetHashCode() == symbol.GetHashCode() && symbol.Declaration.Id == Declaration.Id;

    public override int GetHashCode() => Kind.GetHashCode() + Name.GetHashCode();

    public override string ToString() => $"Symbol({Kind}, {Name})";
}