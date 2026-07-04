using Loom.SemanticAnalysis;

namespace Loom.TypeChecking;

public sealed record TypedFlowAddress(Symbol? Symbol, TypedFlowAddress? Parent, string? FieldName, object? ElementIndex)
{
    public static TypedFlowAddress Variable(Symbol symbol) => new(symbol, null, null, null);
    public static TypedFlowAddress Field(TypedFlowAddress @base, string name) => new(null, @base, name, null);
    public static TypedFlowAddress Element(TypedFlowAddress @base, object constantIndex) => new(null, @base, null, constantIndex);

    public bool Equals(TypedFlowAddress? other) =>
        other != null
        && FieldName == other.FieldName
        && (Symbol?.Equals(other.Symbol) ?? other.Symbol == null)
        && (Parent?.Equals(other.Parent) ?? other.Parent == null);

    public override int GetHashCode() => HashCode.Combine(Symbol, FieldName, Parent);
}