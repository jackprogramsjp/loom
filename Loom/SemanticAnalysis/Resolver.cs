using Loom.Diagnostics;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking;

namespace Loom.SemanticAnalysis;

public class Resolver(ParserResult parserResult) : Visitor<bool>
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<NodeId, Symbol> _allDeclarations = [];
    private readonly Dictionary<NodeId, Symbol> _allReferences = [];
    private readonly Stack<ScopeNode> _scopeNodes = [];
    private readonly Stack<ResolverScope> _scopes = [];
    private readonly Stack<InitializationState> _initializationStates = [];
    private bool _insideFunction;

    public SemanticModel Resolve()
    {
        var rootScope = new ScopeNode();
        _scopeNodes.Push(rootScope);

        var semanticModel = new SemanticModel(
            parserResult.Tree,
            _diagnostics,
            _allDeclarations,
            _allReferences,
            rootScope
        );

        PushScope();
        PushInitializationState();
        foreach (var symbol in IntrinsicTypes.GetSymbols(semanticModel))
            DeclareSymbol(symbol);
        
        VisitTree(parserResult.Tree);
        PopInitializationState();
        PopScope();
        
        return semanticModel;
    }

    protected override bool Visit(Node node) => node.Accept(this);

    public override bool VisitBlock(Block block)
    {
        PushScope();
        var savedState = InheritNewInitializationState();
        var result = block.Statements.All(Visit);
        var blockState = _initializationStates.Pop();
        foreach (var var in blockState.Definite)
            savedState.Definite.Add(var);
        foreach (var var in blockState.Maybe)
            savedState.Maybe.Add(var);
        
        _initializationStates.Push(savedState);
        PopScope();
        
        return result;
    }
    
    public override bool VisitIf(If @if)
    {
        var savedState = new InitializationState(InitializationState());
        Visit(@if.Condition);
    
        var thenState = new InitializationState(savedState);
        _initializationStates.Push(thenState);
        var thenResult = Visit(@if.ThenBranch);
        thenState = new InitializationState(_initializationStates.Pop());
        InitializationState elseState;
        
        if (@if.ElseBranch != null)
        {
            var elseBranchState = new InitializationState(savedState);
            _initializationStates.Push(elseBranchState);
            Visit(@if.ElseBranch.Branch);
            elseState = new InitializationState(_initializationStates.Pop());
        }
        else
        {
            elseState = new InitializationState(savedState);
        }
    
        var definite = new HashSet<string>(savedState.Definite.Concat(thenState.Definite.Where(var => elseState.Definite.Contains(var))));
        var maybe = new HashSet<string>(savedState.Maybe.Concat(thenState.Maybe).Concat(elseState.Maybe));
        _initializationStates.Push(new InitializationState(definite, maybe));
        
        return thenResult;
    }
    
    public override bool VisitReturn(Return @return)
    {
        if (_insideFunction)
            return base.VisitReturn(@return);

        _diagnostics.Error(@return, InternalCodes.ReturnOutsideFunction, "Return statements can only be used inside of functions.");
        return false;
    }

    public override bool VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var scope = CurrentScope();
        var name = functionDeclaration.Name.Text;
        if (scope.VariableLookup.ContainsKey(name))
        {
            _diagnostics.Error(functionDeclaration, InternalCodes.DuplicateName, $"Variable '{name}' is already declared in this scope.");
            return false;
        }

        var symbol = new Symbol(functionDeclaration, SymbolKind.Function, name);
        DeclareSymbol(symbol);
        SetDefinitelyInitialized(name);
        PushScope();
        if (functionDeclaration.TypeParameters != null)
            Visit(functionDeclaration.TypeParameters);

        if (functionDeclaration.Parameters != null)
            Visit(functionDeclaration.Parameters);

        if (functionDeclaration.ReturnType != null)
            Visit(functionDeclaration.ReturnType);

        var wasInsideFunction = _insideFunction;
        _insideFunction = true;
        Visit(functionDeclaration.Body);
        _insideFunction = wasInsideFunction;
        PopScope();
        
        return true;
    }

    public override bool VisitTypeAlias(TypeAlias typeAlias)
    {
        if (!DeclareType(typeAlias))
            return false;
        
        PushScope();
        base.VisitTypeAlias(typeAlias);
        PopScope();
        return true;
    }

    public override bool VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var isMutable = variableDeclaration.Keyword.Kind == SyntaxKind.MutKeyword;
        if (!DeclareVariable(variableDeclaration, SymbolKind.Variable, isMutable))
            return false;

        var name = variableDeclaration.Name.Text;
        base.VisitVariableDeclaration(variableDeclaration);
        if (variableDeclaration.EqualsValueClause != null)
        {
            SetDefinitelyInitialized(name);
        }
        else if (!isMutable)
        {
            _diagnostics.Error(variableDeclaration, InternalCodes.MustHaveInitializer, "Immutable declarations must be initialized.");
            return false;
        }

        return true;
    }

    public override bool VisitParameters(Parameters parameters) => parameters.ParameterList.All(Visit);

    public override bool VisitParameter(Parameter parameter)
    {
        var scope = CurrentScope();
        var name = parameter.Name.Text;
        if (scope.VariableLookup.ContainsKey(name))
        {
            _diagnostics.Error(parameter, InternalCodes.DuplicateName, $"Parameter '{name}' is already declared for this function.");
            return false;
        }
        
        var symbol = new Symbol(parameter, SymbolKind.Parameter, name);
        DeclareSymbol(symbol);
        SetDefinitelyInitialized(name);
        
        if (parameter.EqualsValueClause != null || parameter.ColonTypeClause != null)
            return base.VisitParameter(parameter);

        _diagnostics.Error(parameter, InternalCodes.MustHaveDefaultOrType, "Parameter must have a declared type or default value to infer from.");
        return false;
    }

    public override bool VisitEnumDeclaration(EnumDeclaration enumDeclaration)
    {
        if (!DeclareVariable(enumDeclaration, SymbolKind.Variable))
            return false;

        if (!DeclareType(enumDeclaration, SymbolKind.EnumType))
            return false;

        var name = enumDeclaration.Name.Text;
        SetDefinitelyInitialized(name);

        return true;
    }

    public override bool VisitLiteral(Literal literal) => true;

    public override bool VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Left is not Identifier identifier)
            return base.VisitAssignmentOperator(assignmentOperator);

        var name = identifier.Name.Text;
        if (assignmentOperator.Operator.Kind == SyntaxKind.Equals)
            SetDefinitelyInitialized(name);

        var symbol = LookupValueId(name);
        if (symbol is not { Mutable: false } || assignmentOperator.Operator.Kind != SyntaxKind.Equals)
            return base.VisitAssignmentOperator(assignmentOperator);

        _diagnostics.Error(
            assignmentOperator,
            InternalCodes.AssignToImmutable,
            $"Cannot assign to immutable variable '{name}'.",
            $"did you mean to declare '{name}' as mutable?"
        );

        return false;
    }

    public override bool VisitIdentifier(Identifier identifier)
    {
        var name = identifier.Name.Text;
        var symbol = LookupValueId(name);
        if (symbol == null)
        {
            _diagnostics.Error(identifier, InternalCodes.CannotFindName, $"Cannot find name '{name}'.");
            return false;
        }

        if (symbol.Kind is SymbolKind.Variable or SymbolKind.Parameter or SymbolKind.Function && !IsDefinitelyInitialized(name))
        {
            if (IsMaybeInitialized(name))
                _diagnostics.Error(identifier, InternalCodes.UseOfMaybeUninitialized, $"Variable '{name}' might not be initialized on this path.");
            else
                _diagnostics.Error(identifier, InternalCodes.UseOfUninitialized, $"Use of uninitialized variable '{name}'.");

            return false;
        }
        
        if (symbol.Declaration is EnumDeclaration && identifier.Parent is not QualifiedName or ElementAccess)
        {
            _diagnostics.Error(identifier, InternalCodes.DynamicEnumAccess, "Cannot use enums dynamically because they are compile-time constants.");
            return false;
        }

        _allReferences[identifier.Id] = symbol;
        return true;
    }

    public override bool VisitTypeName(TypeName typeName)
    {
        var name = typeName.Name.Text;
        var symbol = LookupSymbol(name, SymbolKind.Type) ?? LookupSymbol(name, SymbolKind.EnumType);
        if (symbol == null)
        {
            _diagnostics.Error(typeName, InternalCodes.CannotFindName, $"Cannot find type '{name}'.");
            return false;
        }

        _allReferences[typeName.Id] = symbol;
        return true;
    }

    public override bool VisitLiteralType(LiteralType literalType) => true;
    public override bool VisitPrimitiveType(PrimitiveType primitiveType) => true;

    public override bool VisitTypeParameter(TypeParameter typeParameter)
    {
        var scope = CurrentScope();
        var name = typeParameter.Name.Text;
        if (scope.TypeLookup.ContainsKey(name))
        {
            _diagnostics.Error(typeParameter, InternalCodes.DuplicateName, $"Type '{name}' is already declared in this scope.");
            return false;
        }

        var symbol = new Symbol(typeParameter, SymbolKind.Type, name);
        DeclareSymbol(symbol);
        return typeParameter.EqualsTypeClause == null || Visit(typeParameter.EqualsTypeClause);
    }
    
    private bool DeclareVariable(NamedDeclaration node, SymbolKind symbolKind, bool isMutable = false)
    {
        var scope = CurrentScope();
        var name = node.Name.Text;
        if (scope.VariableLookup.ContainsKey(name))
        {
            _diagnostics.Error(node, InternalCodes.DuplicateName, $"Variable '{name}' is already declared in this scope.");
            return false;
        }

        var symbol = new Symbol(node, symbolKind, name, isMutable);
        DeclareSymbol(symbol);
        return true;
    }
    
    private bool DeclareType(NamedDeclaration node, SymbolKind symbolKind = SymbolKind.Type)
    {
        var scope = CurrentScope();
        var name = node.Name.Text;
        if (scope.TypeLookup.ContainsKey(name))
        {
            _diagnostics.Error(node, InternalCodes.DuplicateName, $"Type '{name}' is already declared in this scope.");
            return false;
        }

        var symbol = new Symbol(node, symbolKind, name);
        DeclareSymbol(symbol);
        return true;
    }

    private void DeclareSymbol(Symbol symbol)
    {
        var scope = CurrentScope();
        var lookup = GetLookup(symbol.Kind, scope);
        var nodeId = symbol.Declaration.Id;
        lookup[symbol.Name] = symbol;
        scope.Declarations[nodeId] = symbol;
        _allDeclarations[nodeId] = symbol;
        _scopeNodes.Peek().Symbols.Add(symbol);
        _diagnostics.Info(symbol.Declaration, $"Declared symbol: {symbol}");
    }

    private Symbol? LookupValueId(string name) =>
        LookupSymbol(name, SymbolKind.Variable)
        ?? LookupSymbol(name, SymbolKind.Function)
        ?? LookupSymbol(name, SymbolKind.Parameter);

    private Symbol? LookupSymbol(string name, SymbolKind kind)
    {
        var lookups = _scopes.Select(scope => GetLookup(kind, scope));
        foreach (var lookup in lookups)
        {
            if (!lookup.TryGetValue(name, out var symbol)) continue;
            return symbol;
        }

        return null;
    }

    private static Dictionary<string, Symbol> GetLookup(SymbolKind kind, ResolverScope scope) =>
        kind is SymbolKind.Type or SymbolKind.EnumType ? scope.TypeLookup : scope.VariableLookup;
    
    private void SetDefinitelyInitialized(string name)
    {
        var state = _initializationStates.Peek();
        state.Definite.Add(name);
        state.Maybe.Add(name);
    }
    
    private InitializationState InheritNewInitializationState()
    {
        var state = new InitializationState(InitializationState());
        _initializationStates.Push(state);
        return state;
    }
    
    private void PushInitializationState() => _initializationStates.Push(new InitializationState([], []));
    private bool IsDefinitelyInitialized(string name) => InitializationState().Definite.Contains(name);
    private bool IsMaybeInitialized(string name) => InitializationState().Maybe.Contains(name);
    private InitializationState InitializationState() => _initializationStates.Peek();
    private void PopInitializationState() => _initializationStates.Pop();
    private ResolverScope CurrentScope() => _scopes.Peek();
    private void PopScope() => _scopes.Pop();
    private void PushScope() => _scopes.Push(new ResolverScope());

    protected override bool CombineResults(IEnumerable<bool> results) => results.All(t => t);
}