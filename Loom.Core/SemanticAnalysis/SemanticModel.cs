global using SymbolTable = System.Collections.Generic.Dictionary<Loom.Parsing.AST.NodeId, System.Collections.Generic.List<Loom.SemanticAnalysis.Symbol>>;
using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.TypeChecking;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.SemanticAnalysis;

public sealed class SemanticModel(Tree tree, DiagnosticBag diagnostics, SymbolTable declarations, SymbolTable references)
    : DiagnosedResult(diagnostics)
{
    public Tree Tree { get; } = tree;
    public SymbolTable Declarations { get; } = declarations;
    public SymbolTable References { get; } = references;
    internal TypeSolver TypeSolver { get; } = new(new DiagnosticBag());

    public bool IsCompileTimeConstant(Expression expression) =>
        expression is Literal or NameOf
        || expression is QualifiedName name && GetDeclaringSymbol(name.Identifier) is { Declaration: EnumDeclaration }
        || expression is PropertyAccess access && GetDeclaringSymbol(access.Expression) is { Declaration: EnumDeclaration }
        || expression is ElementAccess elementAccess && GetDeclaringSymbol(elementAccess.Expression) is { Declaration: EnumDeclaration };

    public object? GetConstantValue(Expression expression) =>
        expression switch
        {
            QualifiedName qn when GetType(qn.Identifier) is TypeChecking.Types.ObjectType objectType
                && objectType.GetProperty(qn.Names.First().Name.Text) is { ValueType: TypeChecking.Types.LiteralType literalType } =>
                literalType.Value,
            _ when GetType(expression) is TypeChecking.Types.LiteralType literalType => literalType.Value,
            _ => null
        };

    public List<Symbol> GetDeclarationSymbols(Node node)
    {
        while (true)
        {
            if (node is not Declare declare)
                return Declarations.GetValueOrDefault(node.Id, []);

            node = declare.Signature;
        }
    }

    public Symbol? GetSymbol(Node node, SymbolKind? kind = null) => FindSymbol(node, kind, References);

    public Symbol? GetDeclarationSymbol(Node node, SymbolKind? kind = null)
    {
        while (true)
        {
            if (node is not Declare declare)
                return FindSymbol(node, kind, Declarations);

            node = declare.Signature;
        }
    }

    public Symbol? GetDeclaringSymbol(Node node, SymbolKind? kind = null)
    {
        var referenceSymbol = GetSymbol(node, kind);
        return referenceSymbol == null ? null : GetDeclarationSymbol(referenceSymbol.Declaration, kind);
    }

    public Type GetType(Node node) => TypeSolver.GetType(node);
    public Type? GetDeclarationType(Node node) => GetSymbol(node) is { } symbol ? TypeSolver.GetType(symbol.Declaration) : null;

    private static Symbol? FindSymbol(Node node, SymbolKind? kind, SymbolTable table)
    {
        var symbols = table.GetValueOrDefault(node.Id, []);
        return kind != null ? symbols.Find(s => s.Kind == kind) : symbols.FirstOrDefault();
    }
}