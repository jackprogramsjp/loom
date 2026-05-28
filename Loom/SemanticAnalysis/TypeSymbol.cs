using Loom.Parsing.AST;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.SemanticAnalysis;

public class TypeSymbol(Node declaringNode, Type type, string name)
    : Symbol(declaringNode, SymbolKind.Type, name)
{
    public Type Type { get; } = type;

    public override string ToString() => $"TypeSymbol({Name}, {Type})";
}