using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking;

namespace Loom.SemanticAnalysis;

public class Resolver(ParserResult parserResult, CompilationUnit compilationUnit) : Visitor<bool>
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<NodeId, Symbol> _allDeclarations = [];
    private readonly Dictionary<NodeId, Symbol> _allReferences = [];
    private readonly Stack<ResolverScope> _scopes = [];
    private readonly Stack<FlowState> _flowStates = [];
    private bool _insideFunction;

    public SemanticModel Resolve()
    {
        var semanticModel = new SemanticModel(
            parserResult.Tree,
            _diagnostics,
            _allDeclarations,
            _allReferences
        );

        PushScope();
        PushFlowState(new FlowState([], []));
        
        var intrinsicSymbols = IntrinsicTypes.Register(semanticModel);
        foreach (var symbol in intrinsicSymbols)
        {
            DeclareSymbol(symbol);
        }

        foreach (var (symbol, type) in compilationUnit.Globals)
        {
            DeclareSymbol(symbol);
            semanticModel.TypeSolver.SetType(symbol.Declaration, type);
        }

        VisitTree(parserResult.Tree);
        PopFlowState();
        PopScope();

        return semanticModel;
    }

    protected override bool Visit(Node node) => node.Accept(this);

    public override bool VisitTree(Tree tree) => ResolveStatements(tree.Statements);

    public override bool VisitBlock(Block block)
    {
        PushScope();
        var parentState = CurrentFlowState();
        var blockState = PushInheritedFlowState();
        var result = ResolveStatements(block.Statements);
        MergeFlowState(blockState, target: parentState);
        PopFlowState();
        PushFlowState(parentState);
        PopScope();

        return result;
    }

    public override bool VisitIf(If @if)
    {
        Visit(@if.Condition);
        var beforeIf = new FlowState(CurrentFlowState());
        var thenState = PushAndVisitBranch(@if.ThenBranch, beforeIf);
        if (thenState == null) return false;

        FlowState elseState;
        if (@if.ElseBranch != null)
        {
            var state = PushAndVisitBranch(@if.ElseBranch.Branch, beforeIf);
            if (state == null) return false;
            elseState = state;
        }
        else
        {
            elseState = new FlowState(beforeIf);
        }

        var definitely = new HashSet<Symbol>(beforeIf.DefinitelyInitialized.Concat(thenState.DefinitelyInitialized.Intersect(elseState.DefinitelyInitialized)));
        var maybe = new HashSet<Symbol>(beforeIf.MaybeInitialized);
        maybe.UnionWith(thenState.MaybeInitialized);
        maybe.UnionWith(elseState.MaybeInitialized);

        PopFlowState();
        _flowStates.Push(new FlowState(definitely, maybe, beforeIf.IsUnreachable || thenState.IsUnreachable && elseState.IsUnreachable));
        return true;
    }

    public override bool VisitReturn(Return @return)
    {
        if (_insideFunction)
        {
            CurrentFlowState().IsUnreachable = true;
            return base.VisitReturn(@return);
        }

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

        if (!DeclareVariable(functionDeclaration, SymbolKind.Function, out var symbol))
            return false;

        MarkDefinitelyInitialized(symbol);
        PushScope();

        var wasInsideFunction = _insideFunction;
        _insideFunction = true;
        PushInheritedFlowState();
        base.VisitFunctionDeclaration(functionDeclaration);
        PopFlowState();
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
        if (!DeclareVariable(variableDeclaration, SymbolKind.Variable, out var symbol, isMutable))
            return false;

        base.VisitVariableDeclaration(variableDeclaration);
        if (variableDeclaration.EqualsValueClause != null)
        {
            MarkDefinitelyInitialized(symbol);
        }
        else if (!isMutable)
        {
            _diagnostics.Error(variableDeclaration, InternalCodes.MustHaveInitializer, "Immutable declarations must be initialized.");
            return false;
        }

        return true;
    }

    public override bool VisitInterfaceDeclaration(InterfaceDeclaration interfaceDeclaration)
    {
        if (!DeclareType(interfaceDeclaration))
            return false;

        if (interfaceDeclaration.Body != null)
        {
            var name = interfaceDeclaration.Name.Text;
            var members = interfaceDeclaration.Body.Members;
            var indexers = members.OfType<IndexerDeclaration>().ToList();
            if (indexers.Count > 1)
            {
                foreach (var extraIndexer in indexers.Skip(1))
                    _diagnostics.Error(extraIndexer, InternalCodes.DuplicateIndexer, $"Type '{name}' may only have one indexer.");

                return false;
            }

            var properties = members.OfType<PropertyDeclaration>().ToList();
            var propertyNames = properties.Select(p => p.Name.Text);
            var duplicates = propertyNames.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Count > 0)
            {
                foreach (var duplicate in duplicates)
                {
                    var property = properties.FindLast(p => p.Name.Text == duplicate)!;
                    _diagnostics.Error(property.Span, InternalCodes.DuplicateName, $"Property '{duplicate}' already exists on type '{name}'");
                }

                return false;
            }
        }

        PushScope();
        base.VisitInterfaceDeclaration(interfaceDeclaration);
        PopScope();

        return true;
    }

    public override bool VisitDeclare(Declare declare)
    {
        var symbolKind = declare.Signature switch
        {
            DeclareFunctionSignature => SymbolKind.Function,
            _ => SymbolKind.Variable
        };

        var isMutable = declare.Signature is DeclareVariableSignature { Keyword.Kind: SyntaxKind.MutKeyword };
        if (!DeclareVariable(declare.Signature, symbolKind, out var symbol, isMutable))
            return false;

        MarkDefinitelyInitialized(symbol);
        return base.VisitDeclare(declare);
    }

    public override bool VisitDeclareFunctionSignature(DeclareFunctionSignature declareFunctionSignature)
    {
        PushScope();
        base.VisitDeclareFunctionSignature(declareFunctionSignature);
        PopScope();

        return true;
    }

    public override bool VisitFunctionType(FunctionType functionType)
    {
        PushScope();
        base.VisitFunctionType(functionType);
        PopScope();

        return true;
    }

    public override bool VisitParameter(Parameter parameter)
    {
        var scope = CurrentScope();
        var name = parameter.Name.Text;
        var existingSymbol = scope.VariableLookup
            .Where(pair => pair.Key == name && pair.Value.Kind == SymbolKind.Parameter)
            .Select(pair => pair.Value)
            .FirstOrDefault();

        if (existingSymbol != null)
        {
            _diagnostics.Error(
                parameter,
                InternalCodes.DuplicateName,
                existingSymbol.Kind == SymbolKind.Parameter
                    ? $"Parameter '{name}' is already declared for this function."
                    : $"Variable '{name}' is already declared in this scope."
            );

            return false;
        }

        var symbol = new Symbol(parameter, SymbolKind.Parameter, name);
        DeclareSymbol(symbol);
        MarkDefinitelyInitialized(symbol);

        if (parameter.EqualsValueClause != null || parameter.ColonTypeClause != null)
            return base.VisitParameter(parameter);

        _diagnostics.Error(parameter, InternalCodes.MustHaveDefaultOrType, "Parameter must have a declared type or default value to infer from.");
        return false;
    }

    public override bool VisitEnumDeclaration(EnumDeclaration enumDeclaration)
    {
        if (!DeclareVariable(enumDeclaration, SymbolKind.Variable, out var symbol))
            return false;

        if (!DeclareType(enumDeclaration, SymbolKind.EnumType))
            return false;

        MarkDefinitelyInitialized(symbol);
        return true;
    }

    public override bool VisitLiteral(Literal literal) => true;

    public override bool VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Left is not Identifier identifier)
            return base.VisitAssignmentOperator(assignmentOperator);

        var name = identifier.Name.Text;
        var symbol = LookupValueId(name);
        if (symbol == null)
            return base.VisitAssignmentOperator(assignmentOperator);

        if (assignmentOperator.Operator.Kind == SyntaxKind.Equals)
            MarkDefinitelyInitialized(symbol);

        if (symbol is { IsMutable: true })
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

        if (symbol.Kind is SymbolKind.Variable or SymbolKind.Parameter or SymbolKind.Function && !IsDefinitelyInitialized(symbol))
        {
            if (IsMaybeInitialized(symbol))
                _diagnostics.Error(identifier, InternalCodes.UseOfMaybeUninitialized, $"Variable '{name}' might not be initialized on this path.");
            else
                _diagnostics.Error(identifier, InternalCodes.UseOfUninitialized, $"Use of uninitialized variable '{name}'.");

            return false;
        }

        if (symbol.Declaration is EnumDeclaration && identifier.Parent is not (QualifiedName or PropertyAccess or ElementAccess))
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

        base.VisitTypeName(typeName);
        _allReferences[typeName.Id] = symbol;
        return true;
    }

    public override bool VisitLiteralType(LiteralType literalType) => true;
    public override bool VisitPrimitiveType(PrimitiveType primitiveType) => true;
    public override bool VisitTypeParameter(TypeParameter typeParameter) => DeclareType(typeParameter);

    private bool ResolveStatements(List<Statement> statements) =>
        statements.All(statement =>
            {
                if (CurrentFlowState().IsUnreachable)
                    _diagnostics.Warn(statement, InternalCodes.UnreachableCode, "Unreachable code detected.");

                if (!parserResult.Tree.File.IsDeclaration || statement is Declare or InterfaceDeclaration or TypeAlias)
                    return Visit(statement);

                _diagnostics.Error(statement, InternalCodes.RuntimeInDeclarationFile, "Only type-level declarations are allowed in declaration files.");
                return false;
            }
        );

    private bool DeclareVariable(NamedDeclaration node, SymbolKind symbolKind, [MaybeNullWhen(false)] out Symbol symbol, bool isMutable = false)
    {
        symbol = null;
        var scope = CurrentScope();
        var name = node.Name.Text;
        if (scope.VariableLookup.ContainsKey(name))
        {
            _diagnostics.Error(node, InternalCodes.DuplicateName, $"Variable '{name}' is already declared in this scope.");
            return false;
        }

        symbol = new Symbol(node, symbolKind, name, isMutable);
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
        _diagnostics.Info(symbol.Declaration, $"Declared symbol: {symbol}");
        if (parserResult.Tree.File.IsDeclaration)
            symbol.IsGlobal = true;
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

    private void MarkDefinitelyInitialized(Symbol symbol)
    {
        if (symbol.Kind is SymbolKind.Type or SymbolKind.EnumType)
        {
            _diagnostics.CompilerError(symbol.Declaration, "Attempt to mark symbol as initialized - but symbol is not a value symbol");
            return;
        }

        var state = CurrentFlowState();
        state.DefinitelyInitialized.Add(symbol);
        state.MaybeInitialized.Add(symbol);
    }

    /// <summary>Merges <paramref name="source"/> into <paramref name="target"/>.</summary>
    private static void MergeFlowState(FlowState source, FlowState target)
    {
        foreach (var v in source.DefinitelyInitialized)
            target.DefinitelyInitialized.Add(v);

        foreach (var v in source.MaybeInitialized)
            target.MaybeInitialized.Add(v);
    }

    /// <summary>Pushes a copy of the current flow state and returns that copy.</summary>
    private FlowState PushInheritedFlowState()
    {
        var state = new FlowState(CurrentFlowState());
        PushFlowState(state);
        return state;
    }

    /// <summary>
    /// Visits a branch with a fresh inherited state copied from <paramref name="inherited"/>,
    /// then pops that state and returns its final content (or null if visit failed).
    /// </summary>
    private FlowState? PushAndVisitBranch(Node node, FlowState inherited)
    {
        PushFlowState(new FlowState(inherited));
        var ok = Visit(node);
        var state = PopFlowState();
        return ok ? state : null;
    }

    private void PushFlowState(FlowState state) => _flowStates.Push(state);
    private bool IsDefinitelyInitialized(Symbol symbol) => CurrentFlowState().DefinitelyInitialized.Contains(symbol);
    private bool IsMaybeInitialized(Symbol symbol) => CurrentFlowState().MaybeInitialized.Contains(symbol);
    private FlowState CurrentFlowState() => _flowStates.Peek();
    private FlowState PopFlowState() => _flowStates.Pop();
    private ResolverScope CurrentScope() => _scopes.Peek();
    private void PopScope() => _scopes.Pop();
    private void PushScope() => _scopes.Push(new ResolverScope());

    protected override bool CombineResults(IEnumerable<bool> results) => results.All(t => t);
}
