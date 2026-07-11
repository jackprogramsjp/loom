using Loom.Core.Parsing.AST;
using Loom.Core.Text;

namespace Loom.Core.Resolving;

public class Symbol(Node declaration, SymbolKind kind, string name, bool isMutable = false) : IEquatable<Symbol>
{
    
    public SourceFile File { get; } = declaration.File;
    public Node Declaration { get; } = declaration;
    public SymbolKind Kind { get; } = kind;
    public string Name { get; } = name;
    public bool IsMutable { get; } = isMutable;
    public bool IsIntrinsic { get; internal set; }
    public bool IsGlobal { get; internal set; }
    public bool IsTypeSymbol { get; } = IsTypeKind(kind);
    public bool IsValueSymbol { get; } = IsValueKind(kind);

    internal static bool IsTypeKind(SymbolKind kind) => kind is SymbolKind.Interface or SymbolKind.Type or SymbolKind.EnumType;
    internal static bool IsValueKind(SymbolKind kind) => kind is SymbolKind.Variable or SymbolKind.Function or SymbolKind.Parameter;
    
    public bool Equals(Symbol? symbol) => symbol != null && GetHashCode() == symbol.GetHashCode() && symbol.Declaration.Id == Declaration.Id;
    public override bool Equals(object? obj) => obj is Symbol symbol && Equals(symbol);
    public override int GetHashCode() => Kind.GetHashCode() + Name.GetHashCode();
    public override string ToString() => $"Symbol({Kind}, {Name})";
}