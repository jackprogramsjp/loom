using Loom.SemanticAnalysis;

namespace Loom.TypeChecking.Types;

public class TypeName(TypeSymbol symbol) : Type
{
    public TypeSymbol Symbol { get; } = symbol;
    public Type Underlying { get; } = symbol.Type;

    public override bool Equals(Type? other) => other is TypeName typeName && Symbol.Equals(typeName.Symbol) && Underlying.Equals(typeName.Underlying);

    public override bool IsAssignableTo(Type other) => Underlying.IsAssignableTo(other);

    public override string ToString() => Symbol.Name;
}