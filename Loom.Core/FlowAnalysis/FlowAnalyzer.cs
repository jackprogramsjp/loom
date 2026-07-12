using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Core.Text;

namespace Loom.Core.FlowAnalysis;

public sealed class FlowAnalyzer(SemanticModel semanticModel)
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Stack<List<FlowState>?> _loopExitScopes = [];
    private readonly Dictionary<Node, FlowState> _states = [];

    public FlowState GetState(Node node) => _states.TryGetValue(node, out var existingState) ? existingState : new FlowState();
    public FlowAnalyzerResult Analyze() => new(BindState(semanticModel.Tree, AnalyzeStatements(semanticModel.Tree.Statements, new FlowState())), _diagnostics);

    private FlowState AnalyzeStatements(IReadOnlyList<Statement> statements, FlowState state) =>
        statements.Aggregate(state, (current, statement) => AnalyzeStatement(statement, current));

    private FlowState AnalyzeStatement(Statement statement, FlowState state)
    {
        var newState = statement switch
        {
            Block block => AnalyzeBlock(block, state),
            VariableDeclaration variableDeclaration => AnalyzeVariableDeclaration(variableDeclaration, state),
            FunctionDeclaration functionDeclaration => AnalyzeFunctionDeclaration(functionDeclaration, state),
            Implement implement => AnalyzeImplement(implement, state),
            Return @return => AnalyzeReturn(@return, state),
            Break @break => AnalyzeBreak(@break, state),
            Continue @continue => AnalyzeContinue(@continue, state),
            If @if => AnalyzeIf(@if, state),
            After after => AnalyzeAfter(after, state),
            While @while => AnalyzeWhile(@while, state),
            For @for => AnalyzeFor(@for, state),
            ExpressionStatement expressionStatement => AnalyzeExpressionStatement(expressionStatement, state),
            _ => new FlowState(statement.Children.OfType<Statement>().Select(e => state = AnalyzeStatement(e, state)).LastOrDefault(state))
        };

        if (state.IsUnreachable)
            _diagnostics.Warn(statement, InternalCodes.UnreachableCode, "Unreachable code detected.");

        return newState;
    }

    private FlowState AnalyzeImplement(Implement implement, FlowState state)
    {
        var bodyState = new FlowState(state);
        foreach (var symbol in semanticModel.GetDeclarationSymbols(implement))
        {
            bodyState.DefinitelyInitialized.Add(symbol);
            bodyState.MaybeInitialized.Add(symbol);
        }
        
        foreach (var declaration in implement.Body.Implementations)
        {
            if (semanticModel.GetDeclarationSymbol(declaration, SymbolKind.Function) is not { } symbol) continue;
            bodyState.DefinitelyInitialized.Add(symbol);
            bodyState.MaybeInitialized.Add(symbol);
        }

        return AnalyzeStatement(implement.Body, bodyState);
    }

    private FlowState AnalyzeExpression(Expression expression, FlowState state)
    {
        BindState(expression, state);
        return expression switch
        {
            AssignmentOperator assignmentOperator => AnalyzeAssignment(assignmentOperator, state),
            Identifier identifier => AnalyzeIdentifier(identifier, state),
            _ => new FlowState(expression.Children.Select(e => state = AnalyzeExpression(e, state)).LastOrDefault(state))
        };
    }

    private FlowState AnalyzeBlock(Block block, FlowState state) => BindState(block, AnalyzeStatements(block.Statements, state));
    
    private FlowState AnalyzeFunctionDeclaration(FunctionDeclaration functionDeclaration, FlowState state)
    {
        var newState = new FlowState(state);
        if (semanticModel.GetDeclarationSymbol(functionDeclaration) is { } symbol)
        {
            newState.DefinitelyInitialized.Add(symbol);
            newState.MaybeInitialized.Add(symbol);
        }
        
        var functionState = new FlowState(newState);
        if (functionDeclaration.Parameters != null)
        {
            foreach (var parameter in functionDeclaration.Parameters.ParameterList)
            {
                if (semanticModel.GetDeclarationSymbol(parameter) is not { } parameterSymbol) continue;
                functionState.DefinitelyInitialized.Add(parameterSymbol);
                functionState.MaybeInitialized.Add(parameterSymbol);
            }
        }

        AnalyzeStatement(functionDeclaration.Body, functionState);
        return BindState(functionDeclaration, newState);
    }

    private FlowState AnalyzeVariableDeclaration(VariableDeclaration variableDeclaration, FlowState state)
    {
        if (variableDeclaration.EqualsValueClause == null)
            return BindState(variableDeclaration, new FlowState(state));

        var result = new FlowState(AnalyzeExpression(variableDeclaration.EqualsValueClause.Value, state));
        var symbol = semanticModel.GetDeclarationSymbol(variableDeclaration);
        if (symbol == null)
            return BindState(variableDeclaration, result);

        result.DefinitelyInitialized.Add(symbol);
        result.MaybeInitialized.Add(symbol);
        return BindState(variableDeclaration, result);
    }

    private FlowState AnalyzeIf(If @if, FlowState state)
    {
        AnalyzeExpression(@if.Condition, state);
        var thenState = AnalyzeStatement(@if.ThenBranch, new FlowState(state));
        var elseState = @if.ElseBranch != null ? AnalyzeStatement(@if.ElseBranch.Branch, new FlowState(state)) : new FlowState(state);
        return BindState(@if, thenState.Merge(elseState));
    }

    private FlowState AnalyzeAfter(After after, FlowState state)
    {
        var bodyState = new FlowState(AnalyzeExpression(after.Duration, state));
        var (body, _) = CaptureExitStates(after.Body, bodyState, null);
        return BindState(after, state.Merge(body));
    }

    private FlowState AnalyzeWhile(While @while, FlowState state)
    {
        var bodyState = new FlowState(AnalyzeExpression(@while.Condition, state));
        var (body, breakStates) = CaptureExitStates(@while.Body, bodyState, []);
        return BindState(@while, ComputeLoopExitState(state, body, breakStates));
    }

    private FlowState AnalyzeFor(For @for, FlowState state)
    {
        var bodyState = new FlowState(AnalyzeExpression(@for.CollectionExpression, state));
        foreach (var symbol in @for.Names.Select(name => semanticModel.GetDeclarationSymbol(name)).OfType<Symbol>())
        {
            bodyState.DefinitelyInitialized.Add(symbol);
            bodyState.MaybeInitialized.Add(symbol);
        }

        var (body, breakStates) = CaptureExitStates(@for.Body, bodyState, []);
        return BindState(@for, ComputeLoopExitState(state, body, breakStates));
    }

    private FlowState AnalyzeBreak(Break @break, FlowState state)
    {
        var result = new FlowState(state) { IsUnreachable = true };
        if (_loopExitScopes.Count > 0 && _loopExitScopes.Peek() is { } scope)
            scope.Add(result);

        return BindState(@break, result);
    }

    private FlowState AnalyzeContinue(Continue @continue, FlowState state) => BindState(@continue, new FlowState(state) { IsUnreachable = true });

    private FlowState AnalyzeReturn(Return @return, FlowState state) =>
        BindState(@return, new FlowState(@return.Expression != null ? AnalyzeExpression(@return.Expression, state) : state) { IsUnreachable = true });

    private FlowState AnalyzeExpressionStatement(ExpressionStatement statement, FlowState state)
    {
        state = AnalyzeExpression(statement.Expression, state);
        return BindState(statement, state);
    }

    private FlowState AnalyzeAssignment(AssignmentOperator assignment, FlowState state)
    {
        if (assignment.Left is Identifier identifier)
        {
            var symbol = semanticModel.GetSymbol(identifier);
            if (symbol == null)
                return BindState(assignment, state);

            if (assignment.Operator.Kind != SyntaxKind.Equals)
            {
                state = AnalyzeIdentifier(identifier, state);
                state = AnalyzeExpression(assignment.Right, state);
                CheckReassignment(assignment, identifier, symbol, state);
                return BindState(assignment, state);
            }

            CheckReassignment(assignment, identifier, symbol, state);
            state = AnalyzeExpression(assignment.Right, state);
            state.DefinitelyInitialized.Add(symbol);
            state.MaybeInitialized.Add(symbol);
        }
        else
        {
            state = AnalyzeExpression(assignment.Left, state);
            state = AnalyzeExpression(assignment.Right, state);
        }
        
        return BindState(assignment, state);
    }

    private void CheckReassignment(AssignmentOperator assignment, Identifier identifier, Symbol symbol, FlowState state)
    {
        if (symbol.IsMutable) return;
        _diagnostics.Error(
            assignment,
            InternalCodes.AssignToImmutable,
            $"Cannot assign to immutable variable '{identifier}'.",
            $"did you mean to declare '{identifier}' as mutable?"
        );
    }

    private FlowState AnalyzeIdentifier(Identifier identifier, FlowState state)
    {
        var symbol = semanticModel.GetSymbol(identifier);
        if (symbol is null)
            return BindState(identifier, state);

        if (symbol.IsIntrinsic || symbol.Declaration.FirstAncestorOfType<Declare>() is not null)
            return BindState(identifier, state);

        if (symbol is not { IsValueSymbol: true } || state.DefinitelyInitialized.Contains(symbol))
            return BindState(identifier, state);

        if (state.MaybeInitialized.Contains(symbol))
            _diagnostics.Error(identifier, InternalCodes.UseOfMaybeUninitialized, $"Variable '{symbol.Name}' might not be initialized on this path.");
        else
            _diagnostics.Error(identifier, InternalCodes.UseOfUninitialized, $"Use of uninitialized variable '{symbol.Name}'.");

        return BindState(identifier, state);
    }

    private (FlowState State, List<FlowState> ExitStates) CaptureExitStates(Statement statement, FlowState bodyState, List<FlowState>? initial)
    {
        _loopExitScopes.Push(initial);
        var body = AnalyzeStatement(statement, bodyState);
        return (body, _loopExitScopes.Pop()!);
    }

    private static FlowState ComputeLoopExitState(FlowState entryState, FlowState bodyState, List<FlowState> breakStates)
    {
        var exitPaths = breakStates.Prepend(entryState);
        var definitely = exitPaths.Aggregate<FlowState, HashSet<Symbol>?>(
            null,
            (current, exit) => current == null ? [..exit.DefinitelyInitialized] : [..current.Intersect(exit.DefinitelyInitialized)]
        );

        var maybe = new HashSet<Symbol>(entryState.MaybeInitialized);
        maybe.UnionWith(bodyState.MaybeInitialized);
        foreach (var exit in breakStates)
            maybe.UnionWith(exit.MaybeInitialized);

        return new FlowState(definitely ?? [], maybe, isUnreachable: entryState.IsUnreachable);
    }

    private FlowState BindState(Node node, FlowState state)
    {
        _states[node] = state;
        return state;
    }
}