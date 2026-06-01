using Loom.Diagnostics;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.SemanticAnalysis;

public class ResolverScope
{
    public Dictionary<NodeId, Symbol> Declarations { get; } = new();
    public Dictionary<NodeId, Symbol> References { get; } = new();
    public Dictionary<string, Symbol> VariableLookup { get; } = new();
    public Dictionary<string, Symbol> TypeLookup { get; } = new();
    public Dictionary<string, bool> InitializationState { get; } = new();
}

public class Resolver(ParserResult parserResult) : Visitor<bool>
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<NodeId, Symbol> _allDeclarations = new();
    private readonly Dictionary<NodeId, Symbol> _allReferences = new();
    private readonly Stack<ScopeNode> _scopeNodes = new();
    private readonly Stack<ResolverScope> _scopes = new();

    public SemanticModel Resolve()
    {
        var rootScope = new ScopeNode();
        _scopeNodes.Push(rootScope);

        PushScope();
        VisitTree(parserResult.Tree);
        PopScope();
        return new SemanticModel(
            parserResult.Tree,
            _diagnostics,
            _allDeclarations,
            _allReferences,
            rootScope
        );
    }

    public override bool Visit(Node node) => node.Accept(this);

    public override bool VisitTypeAlias(TypeAlias typeAlias)
    {
        var scope = CurrentScope();
        var name = typeAlias.Name.Text;
        if (scope.TypeLookup.ContainsKey(name))
        {
            _diagnostics.Error(typeAlias.Span, InternalCodes.DuplicateName, $"Type '{name}' is already declared in this scope.");
            return false;
        }

        var symbol = new Symbol(typeAlias, SymbolKind.Type, name);
        DeclareSymbol(symbol);

        PushScope();
        base.VisitTypeAlias(typeAlias);
        PopScope();
        return true;
    }

    public override bool VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var scope = CurrentScope();
        var name = variableDeclaration.Name.Text;
        if (scope.VariableLookup.ContainsKey(name))
        {
            _diagnostics.Error(variableDeclaration.Span, InternalCodes.DuplicateName, $"Variable '{name}' is already declared in this scope.");
            return false;
        }

        var symbol = new Symbol(variableDeclaration, SymbolKind.Variable, name);
        DeclareSymbol(symbol);

        base.VisitVariableDeclaration(variableDeclaration);
        if (variableDeclaration.EqualsValueClause != null)
        {
            scope.InitializationState[name] = true;
        }
        else if (variableDeclaration.Keyword.Kind == SyntaxKind.LetKeyword)
        {
            _diagnostics.Error(variableDeclaration.Span, InternalCodes.MustHaveInitializer, "Immutable declarations must be initialized.");
            return false;
        }

        return true;
    }

    public override bool VisitLiteral(Literal literal) => true;

    public override bool VisitIdentifier(Identifier identifier)
    {
        var name = identifier.Name.Text;
        var symbol = LookupSymbol(name, SymbolKind.Variable);
        if (symbol == null)
        {
            _diagnostics.Error(identifier.Span, InternalCodes.CannotFindName, $"Cannot find name '{name}'.");
            return false;
        }

        if (symbol.Kind is SymbolKind.Variable or SymbolKind.Parameter && !IsSymbolInitialized(symbol))
        {
            _diagnostics.Error(identifier.Span, InternalCodes.UseOfUnassigned, $"Use of unassigned variable '{name}'.");
            return false;
        }

        _allReferences[identifier.Id] = symbol;
        return true;
    }

    public override bool VisitTypeName(TypeName typeName)
    {
        var name = typeName.Name.Text;
        var symbol = LookupSymbol(name, SymbolKind.Type);
        if (symbol == null)
        {
            _diagnostics.Error(typeName.Span, InternalCodes.CannotFindName, $"Cannot find type '{name}'.");
            return false;
        }

        _allReferences[typeName.Id] = symbol;
        return true;
    }

    public override bool VisitPrimitiveType(PrimitiveType primitiveType) => true;

    public override bool VisitTypeParameter(TypeParameter typeParameter)
    {
        var scope = CurrentScope();
        var name = typeParameter.Name.Text;
        if (scope.TypeLookup.ContainsKey(name))
        {
            _diagnostics.Error(typeParameter.Span, InternalCodes.DuplicateName, $"Type '{name}' is already declared in this scope.");
            return false;
        }

        var symbol = new Symbol(typeParameter, SymbolKind.Type, name);
        DeclareSymbol(symbol);
        return typeParameter.EqualsTypeClause == null || Visit(typeParameter.EqualsTypeClause);
    }

    protected override bool CombineResults(IEnumerable<bool> results) => results.All(t => t);

    private void DeclareSymbol(Symbol symbol)
    {
        _diagnostics.Info(symbol.DeclaringNode.Span, $"Declared symbol: {symbol}");
        var scope = CurrentScope();
        var lookup = symbol.Kind == SymbolKind.Type ? scope.TypeLookup : scope.VariableLookup;
        var nodeId = symbol.DeclaringNode.Id;
        lookup.Add(symbol.Name, symbol);
        scope.Declarations.Add(nodeId, symbol);
        scope.InitializationState.Add(symbol.Name, false);
        _allDeclarations.Add(nodeId, symbol);
        _scopeNodes.Peek().Symbols.Add(symbol);
    }

    private Symbol? LookupSymbol(string name, SymbolKind kind)
    {
        var lookups = _scopes.Select(scope => kind == SymbolKind.Type ? scope.TypeLookup : scope.VariableLookup);
        foreach (var lookup in lookups)
        {
            if (!lookup.TryGetValue(name, out var symbol)) continue;
            return symbol;
        }

        return null;
    }

    private bool IsSymbolInitialized(Symbol symbol) =>
    (
        from scope in _scopes
        where scope.Declarations.ContainsKey(symbol.DeclaringNode.Id)
        select scope.InitializationState.TryGetValue(symbol.Name, out var initialized) && initialized
    ).FirstOrDefault();

    private ResolverScope CurrentScope() => _scopes.Peek();
    private void PopScope() => _scopes.Pop();

    private ResolverScope PushScope()
    {
        var scope = new ResolverScope();
        _scopes.Push(scope);
        return scope;
    }
}