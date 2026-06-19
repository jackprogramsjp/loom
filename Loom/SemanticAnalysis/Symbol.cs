using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.SemanticAnalysis;

public class Symbol(Node declaration, SymbolKind kind, string name, bool isMutable = false, bool isIntrinsic = false) : IEquatable<Symbol>
{
    public SourceFile File { get; } = declaration.File;
    public Node Declaration { get; } = declaration;
    public SymbolKind Kind { get; } = kind;
    public string Name { get; } = name;
    public bool IsMutable { get; } = isMutable;
    public bool IsIntrinsic { get; } = isIntrinsic;
    public bool IsGlobal { get; internal set; }

    public bool Equals(Symbol? symbol) => symbol != null && GetHashCode() == symbol.GetHashCode() && symbol.Declaration.Id == Declaration.Id;
    public override bool Equals(object? obj) => obj is Symbol symbol && Equals(symbol);
    public override int GetHashCode() => Kind.GetHashCode() + Name.GetHashCode();
    public override string ToString() => $"Symbol({Kind}, {Name})";
}