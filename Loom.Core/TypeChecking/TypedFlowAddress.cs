using Loom.SemanticAnalysis;

namespace Loom.TypeChecking;

internal sealed class TypedFlowAddress : IEquatable<TypedFlowAddress>
{
    public static TypedFlowAddress Variable(Symbol symbol) => new(symbol, null, null, null);
    public static TypedFlowAddress Field(TypedFlowAddress @base, string name) => new(null, @base, name, null);
    public static TypedFlowAddress Element(TypedFlowAddress @base, object constantIndex) => new(null, @base, null, constantIndex);

    private TypedFlowAddress(Symbol? symbol, TypedFlowAddress? parent, string? fieldName, object? elementIndex)
    {
        Symbol = symbol;
        Parent = parent;
        FieldName = fieldName;
        ElementIndex = elementIndex;
    }

    public Symbol? Symbol { get; }
    public TypedFlowAddress? Parent { get; }
    public string? FieldName { get; }
    public object? ElementIndex { get; }

    public bool Equals(TypedFlowAddress? other) =>
        other != null
        && Equals(Symbol, other.Symbol)
        && FieldName == other.FieldName
        && (Parent?.Equals(other.Parent) ?? other.Parent is null);

    public override bool Equals(object? obj) => obj is TypedFlowAddress fa && Equals(fa);
    public override int GetHashCode() => HashCode.Combine(Symbol, FieldName, Parent);
}