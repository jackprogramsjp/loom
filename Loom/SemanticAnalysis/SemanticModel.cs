using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.TypeChecking;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.SemanticAnalysis;

public class SemanticModel(Tree tree, DiagnosticBag diagnostics, Dictionary<NodeId, Symbol> declarations, Dictionary<NodeId, Symbol> references)
    : DiagnosedResult(diagnostics)
{
    public Tree Tree { get; } = tree;
    internal TypeSolver TypeSolver { get; } = new(new DiagnosticBag());

    public Symbol? GetSymbol(Node node) => references.GetValueOrDefault(node.Id);
    public Symbol? GetDeclarationSymbol(Node node) => declarations.GetValueOrDefault(node.Id);

    public Symbol? GetDeclaringSymbol(Node node)
    {
        var referenceSymbol = GetSymbol(node);
        return referenceSymbol == null ? null : GetDeclarationSymbol(referenceSymbol.Declaration);
    }

    public Type GetType(Node node) => TypeSolver.GetType(node);
    public Type? GetDeclaredType(Node node) => GetSymbol(node) is { } symbol ? TypeSolver.GetType(symbol.Declaration) : null;
}