using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking;

namespace Loom.SemanticAnalysis;

public sealed class Resolver(ParserResult parserResult, CompilationUnit compilationUnit)
    : Visitor<bool>(true)
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<NodeId, Symbol> _allDeclarations = [];
    private readonly Dictionary<NodeId, Symbol> _allReferences = [];
    private readonly Stack<ResolverScope> _scopes = [];
    private readonly Stack<FlowState> _flowStates = [];
    private ResolverContext _context = ResolverContext.None;

    public SemanticModel Resolve()
    {
        var semanticModel = new SemanticModel(parserResult.Tree, _diagnostics, _allDeclarations, _allReferences);
        PushScope();
        PushFlowState(new FlowState([], []));
        DeclareIntrinsicSymbols(semanticModel);
        DeclareGlobalSymbols(semanticModel);
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
        PopScope();

        return result;
    }

    public override bool VisitAfter(After after)
    {
        Visit(after.Duration);
        var before = new FlowState(CurrentFlowState());
        var bodyState = PushFlowAndVisitBranch(after.Body, before);
        if (bodyState == null)
            return false;
        
        var definitely = new HashSet<Symbol>(before.DefinitelyInitialized);
        var maybe = new HashSet<Symbol>(before.MaybeInitialized.Concat(bodyState.DefinitelyInitialized.Intersect(before.DefinitelyInitialized)));
        maybe.UnionWith(bodyState.MaybeInitialized);
        
        PopFlowState();
        _flowStates.Push(new FlowState(definitely, maybe, before.IsUnreachable));
        return true;
    }

    public override bool VisitWhile(While @while)
    {
        Visit(@while.Condition);
        var beforeLoop = new FlowState(CurrentFlowState());
        var lastContext = _context;
        _context = ResolverContext.Loop;
        var bodyState = PushFlowAndVisitBranch(@while.Body, beforeLoop);
        _context = lastContext;

        if (bodyState == null)
            return false;

        var definitely = new HashSet<Symbol>(beforeLoop.DefinitelyInitialized.Concat(bodyState.DefinitelyInitialized.Intersect(beforeLoop.DefinitelyInitialized)));
        var maybe = new HashSet<Symbol>(beforeLoop.MaybeInitialized);
        maybe.UnionWith(bodyState.MaybeInitialized);

        PopFlowState();
        _flowStates.Push(new FlowState(definitely, maybe, beforeLoop.IsUnreachable));
        return true;
    }

    public override bool VisitIf(If @if)
    {
        Visit(@if.Condition);
        var beforeIf = new FlowState(CurrentFlowState());
        var thenState = PushFlowAndVisitBranch(@if.ThenBranch, beforeIf);
        if (thenState == null) return false;

        FlowState elseState;
        if (@if.ElseBranch != null)
        {
            var state = PushFlowAndVisitBranch(@if.ElseBranch.Branch, beforeIf);
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

    public override bool VisitContinue(Continue @continue)
    {
        if (_context == ResolverContext.Loop)
        {
            CurrentFlowState().IsUnreachable = true;
            return base.VisitContinue(@continue);
        }

        _diagnostics.Error(@continue, InternalCodes.ContinueOutsideLoop, "Continue statements can only be used inside of loops.");
        return false;
    }

    public override bool VisitBreak(Break @break)
    {
        if (_context == ResolverContext.Loop)
        {
            CurrentFlowState().IsUnreachable = true;
            return base.VisitBreak(@break);
        }

        _diagnostics.Error(@break, InternalCodes.BreakOutsideLoop, "Break statements can only be used inside of loops.");
        return false;
    }

    public override bool VisitReturn(Return @return)
    {
        if (_context == ResolverContext.Function)
        {
            CurrentFlowState().IsUnreachable = true;

            var after = @return.FirstAncestorOfType<After>();
            if (after == null || @return.FirstAncestorOfType<FunctionDeclaration>()?.FirstAncestorOfType<After>() == after)
                return base.VisitReturn(@return);

            _diagnostics.Error(@return, InternalCodes.ReturnInAfter, "Cannot return a value from an 'after' statement body.");
            return false;
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

        var lastContext = _context;
        _context = ResolverContext.Function;
        PushInheritedFlowState();
        base.VisitFunctionDeclaration(functionDeclaration);
        PopFlowState();
        _context = lastContext;

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
        if (!DeclareVariable(interfaceDeclaration, SymbolKind.Variable, out var valueSymbol))
            return false;

        var isSealed = interfaceDeclaration.SealedKeyword != null;
        if (!DeclareInterface(interfaceDeclaration, isSealed, out var symbol))
            return false;

        MarkDefinitelyInitialized(valueSymbol);
        if (!ResolveInterfaceBody(interfaceDeclaration.Body, valueSymbol.Name))
            return false;

        if (!ResolveInterfaceConstraints(interfaceDeclaration.ColonTypeListClause, symbol))
            return false;

        PushScope();
        base.VisitInterfaceDeclaration(interfaceDeclaration);
        PopScope();

        return true;
    }

    public override bool VisitDeclare(Declare declare)
    {
        var symbolKind = declare.Signature switch
        {
            InterfaceDeclaration => SymbolKind.Interface,
            DeclareFunctionSignature => SymbolKind.Function,
            _ => SymbolKind.Variable
        };

        if (Symbol.IsValueKind(symbolKind))
        {
            var isMutable = declare.Signature is DeclareVariableSignature { Keyword.Kind: SyntaxKind.MutKeyword };
            if (!DeclareVariable(declare.Signature, symbolKind, out var symbol, isMutable))
                return false;

            MarkDefinitelyInitialized(symbol);
        }
        else if (declare.Signature is InterfaceDeclaration interfaceDeclaration)
        {
            var isSealed = interfaceDeclaration.SealedKeyword != null;
            return DeclareInterface(interfaceDeclaration, isSealed, out _);
        }

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
        if (!DeclareVariable(enumDeclaration, SymbolKind.Variable, out var symbol) || !DeclareType(enumDeclaration, SymbolKind.EnumType))
            return false;

        MarkDefinitelyInitialized(symbol);
        return true;
    }

    public override bool VisitInterfaceInvocation(InterfaceInvocation interfaceInvocation)
    {
        var name = interfaceInvocation.Name.Token.Text;
        var symbol = LookupValueSymbol(name) ?? LookupTypeSymbol(name);
        switch (symbol)
        {
            case null:
                _diagnostics.Error(interfaceInvocation.Name, InternalCodes.CannotFindSymbol, $"Cannot find interface symbol '{name}'.");
                return false;
            case InterfaceSymbol:
                _diagnostics.Error(
                    interfaceInvocation,
                    InternalCodes.InvokeDeclaredInterface,
                    $"Cannot invoke interface '{name}' because it was declared as type."
                );

                return false;
        }
        
        return base.VisitInterfaceInvocation(interfaceInvocation);
    }

    public override bool VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Left is not Identifier identifier)
            return base.VisitAssignmentOperator(assignmentOperator);

        var name = identifier.Name.Text;
        var symbol = LookupValueSymbol(name);
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
        var symbol = LookupValueSymbol(name);
        if (symbol == null)
        {
            _diagnostics.Error(identifier, InternalCodes.CannotFindName, $"Cannot find name '{name}'.");
            return false;
        }

        if (symbol.IsValueSymbol && !IsDefinitelyInitialized(symbol))
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
        var symbol = LookupTypeSymbol(name);
        if (symbol == null)
        {
            _diagnostics.Error(typeName, InternalCodes.CannotFindName, $"Cannot find type '{name}'.");
            return false;
        }

        base.VisitTypeName(typeName);
        _allReferences[typeName.Id] = symbol;
        return true;
    }

    public override bool VisitTypeParameter(TypeParameter typeParameter) => DeclareType(typeParameter);

    private bool ResolveInterfaceBody(InterfaceBody? body, string name)
    {
        if (body == null)
            return true;

        var indexers = body.Members.OfType<IndexerDeclaration>().ToList();
        if (indexers.Count > 1)
        {
            foreach (var extraIndexer in indexers.Skip(1))
                _diagnostics.Error(extraIndexer, InternalCodes.DuplicateIndexer, $"Type '{name}' may only have one indexer.");

            return false;
        }

        var properties = body.Members.OfType<PropertyDeclaration>().ToList();
        var propertyNames = properties.Select(p => p.Name.Text);
        var duplicates = propertyNames.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count <= 0)
            return true;

        foreach (var duplicate in duplicates)
        {
            var property = properties.FindLast(p => p.Name.Text == duplicate)!;
            _diagnostics.Error(property.Span, InternalCodes.DuplicateName, $"Property '{duplicate}' already exists on type '{name}'");
        }

        return false;
    }

    private bool ResolveInterfaceConstraints(ColonTypeListClause? colonTypeListClause, InterfaceSymbol symbol)
    {
        if (colonTypeListClause == null)
            return true;

        foreach (var constraint in colonTypeListClause.Types)
        {
            if (constraint is not TypeName typeName)
                return ReportNonInterfaceConstraint(constraint);

            var constraintSymbol = LookupTypeSymbol(typeName.Name.Text);
            if (constraintSymbol is not InterfaceSymbol interfaceSymbol)
                return ReportNonInterfaceConstraint(constraint);

            if (!interfaceSymbol.IsSealed) continue;
            _diagnostics.Error(
                constraint,
                InternalCodes.InheritFromSealed,
                $"Cannot constrain interface '{symbol.Name}' with sealed interface '{interfaceSymbol.Name}'."
            );

            return false;
        }

        return true;
    }

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

    private bool DeclareInterface(InterfaceDeclaration interfaceDeclaration, bool isSealed, [MaybeNullWhen(false)] out InterfaceSymbol interfaceSymbol)
    {
        interfaceSymbol = null;
        var scope = CurrentScope();
        var name = interfaceDeclaration.Name.Text;
        if (scope.TypeLookup.ContainsKey(name))
        {
            _diagnostics.Error(interfaceDeclaration.Name, InternalCodes.DuplicateName, $"Interface '{name}' is already declared in this scope.");
            return false;
        }

        interfaceSymbol = new InterfaceSymbol(interfaceDeclaration, name, isSealed);
        DeclareSymbol(interfaceSymbol);

        return true;
    }

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

    private Symbol? LookupTypeSymbol(string name) =>
        LookupSymbol(name, SymbolKind.Type)
        ?? LookupSymbol(name, SymbolKind.EnumType)
        ?? LookupSymbol(name, SymbolKind.Interface);

    private Symbol? LookupValueSymbol(string name) =>
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

    private static Dictionary<string, Symbol> GetLookup(SymbolKind kind, ResolverScope scope) => Symbol.IsTypeKind(kind) ? scope.TypeLookup : scope.VariableLookup;

    private void MarkDefinitelyInitialized(Symbol symbol)
    {
        if (!symbol.IsValueSymbol)
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
    private FlowState? PushFlowAndVisitBranch(Node node, FlowState inherited)
    {
        PushFlowState(new FlowState(inherited));
        var ok = Visit(node);
        var state = PopFlowState();
        return ok ? state : null;
    }

    private bool ReportNonInterfaceConstraint(TypeExpression constraint)
    {
        _diagnostics.Error(
            constraint,
            InternalCodes.NonInterfaceConstraint,
            "Interfaces may only be constrained by other interfaces."
        );

        return false;
    }

    private void DeclareGlobalSymbols(SemanticModel semanticModel)
    {
        foreach (var (symbol, type) in compilationUnit.Globals)
        {
            DeclareSymbol(symbol);
            semanticModel.TypeSolver.SetType(symbol.Declaration, type);
        }
    }

    private void DeclareIntrinsicSymbols(SemanticModel semanticModel)
    {
        var intrinsicSymbols = Intrinsics.Register(semanticModel);
        foreach (var symbol in intrinsicSymbols)
        {
            DeclareSymbol(symbol);
            if (symbol.IsValueSymbol)
                MarkDefinitelyInitialized(symbol);
        }
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