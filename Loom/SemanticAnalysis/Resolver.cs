using Loom.Diagnostics;
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

public class Resolver(Tree ast) : Diagnosable, IVisitor<bool>
{
    private readonly Stack<ResolverScope> _scopes = new();
    private readonly Dictionary<NodeId, Symbol> _allDeclarations = new();
    private readonly Dictionary<NodeId, Symbol> _allReferences = new();
    private readonly Stack<ScopeNode> _scopeNodes = new();

    public SemanticModel Resolve()
    {
        var rootScope = new ScopeNode();
        _scopeNodes.Push(rootScope);
        
        VisitTree(ast);
        return new SemanticModel(ast, Diagnostics, _allDeclarations, _allReferences, rootScope);
    }

    public bool VisitTree(Tree tree)
    {
        PushScope();
        var result = VisitList(tree.Statements);
        PopScope();
        return result;
    }

    public bool VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var scope = CurrentScope();
        var name = variableDeclaration.Name.Text;
        if (scope.VariableLookup.ContainsKey(name))
        {
            Diagnostics.Error(variableDeclaration.Span, InternalCodes.DuplicateName, $"Variable '{name}' is already declared in this scope.");
            return false;
        }
        
        var symbol = new Symbol(variableDeclaration, SymbolKind.Variable, name);
        DeclareSymbol(symbol);
        
        if (variableDeclaration.ColonTypeClause != null)
        {
            Visit(variableDeclaration.ColonTypeClause);
        }
        if (variableDeclaration.EqualsValueClause != null)
        {
            Visit(variableDeclaration.EqualsValueClause);
            scope.InitializationState[name] = true;
        } else if (variableDeclaration.Keyword.Kind == SyntaxKind.LetKeyword)
        {
            Diagnostics.Error(variableDeclaration.Span, InternalCodes.MustHaveInitializer, "Immutable declarations must be initialized.");
            return false;
        }
        
        return true;
    }

    public bool VisitLiteral(Literal literal) => true;

    public bool VisitIdentifier(Identifier identifier)
    {
        var name = identifier.Name.Text;
        var symbol = LookupSymbol(name, SymbolKind.Variable);
        if (symbol == null)
        {
            Diagnostics.Error(identifier.Span, InternalCodes.CannotFindName, $"Cannot find name '{name}'.");
            return false;
        }
        
        if (symbol.Kind is SymbolKind.Variable or SymbolKind.Parameter && !IsSymbolInitialized(symbol))
        {
            Diagnostics.Error(identifier.Span, InternalCodes.UseOfUnassigned, $"Use of unassigned variable '{name}'.");
            return false;
        }
        
        _allReferences[identifier.Id] = symbol;
        return true;
    }
    
    public bool VisitBinaryOperator(BinaryOperator binaryOperator) => Visit(binaryOperator.Left) && Visit(binaryOperator.Right);
    
    public bool VisitTypeName(TypeName typeName)
    {
        var name = typeName.Name.Text;
        var symbol = LookupSymbol(name, SymbolKind.Type);
        if (symbol == null)
        {
            Diagnostics.Error(typeName.Span, InternalCodes.CannotFindName, $"Cannot find type '{name}'.");
            return false;
        }
        
        _allReferences[typeName.Id] = symbol;
        return true;
    }

    public bool VisitPrimitiveType(PrimitiveType primitiveType) => true;

    public bool Visit(Node node) => node.Accept(this);
    
    private bool VisitList(IEnumerable<Node> nodes) => nodes.Select(Visit).All(n => n);

    private void DeclareSymbol(Symbol symbol)
    {
        var scope = CurrentScope();
        var nodeId = symbol.DeclaringNode.Id;
        scope.Declarations.TryAdd(nodeId, symbol);
        scope.InitializationState.TryAdd(symbol.Name, false);
        _allDeclarations.TryAdd(nodeId, symbol);
        _scopeNodes.Peek().Symbols.Add(symbol);

        var lookup = symbol.Kind == SymbolKind.Type ? scope.TypeLookup : scope.VariableLookup;
        lookup.TryAdd(symbol.Name, symbol);
    }
    
    private Symbol? LookupSymbol(string name, SymbolKind kind)
    {
        foreach (var lookup in _scopes.Select(scope => kind == SymbolKind.Type 
                                                  ? scope.TypeLookup 
                                                  : scope.VariableLookup))
        {
            if (!lookup.TryGetValue(name, out var symbol)) continue;
            return symbol;
        }

        return null;
    }
    
    private bool IsSymbolInitialized(Symbol symbol)
    {
        return (
            from scope in _scopes
            where scope.Declarations.ContainsKey(symbol.DeclaringNode.Id)
            select scope.InitializationState.TryGetValue(symbol.Name, out var initialized)
                && initialized).FirstOrDefault();
    }
    
    private ResolverScope CurrentScope() => _scopes.Peek();
    private ResolverScope PopScope() => _scopes.Pop();
    private ResolverScope PushScope()
    {
        var scope = new ResolverScope();
        _scopes.Push(scope);
        return scope;
    }
}