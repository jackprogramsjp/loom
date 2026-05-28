using Loom.SemanticAnalysis;

namespace Loom.TypeChecking.Types;

public class TypeName(TypeSymbol symbol) : Type
{
    public TypeSymbol Symbol { get; } = symbol;
    public Type Underlying { get; } = symbol.Type;

    public override bool IsAssignableTo(Type other) => Underlying.IsAssignableTo(other);

    public override string ToString() => Symbol.Name;
}