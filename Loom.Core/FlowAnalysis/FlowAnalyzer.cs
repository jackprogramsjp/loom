using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Resolving;
using Loom.Text;

namespace Loom.FlowAnalysis;

public sealed class FlowAnalyzer(SemanticModel semanticModel)
{
    private readonly DiagnosticBag _diagnostics = new();
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
            Return @return => AnalyzeReturn(@return, state),
            Break @break => AnalyzeBreak(@break, state),
            Continue @continue => AnalyzeContinue(@continue, state),
            If @if => AnalyzeIf(@if, state),
            After after => AnalyzeAfter(after, state),
            While @while => AnalyzeWhile(@while, state),
            For @for => AnalyzeFor(@for, state),
            ExpressionStatement expressionStatement => AnalyzeExpressionStatement(expressionStatement, state),
            _ => new FlowState(statement.Children.OfType<Statement>().Select(e => state = AnalyzeStatement(e, state)).LastOrDefault(new FlowState()))
        };
        
        if (state.IsUnreachable)
            _diagnostics.Warn(statement, InternalCodes.UnreachableCode, "Unreachable code detected.");

        return newState;
    }

    private FlowState AnalyzeExpression(Expression expression, FlowState state)
    {
        BindState(expression, state);
        return expression switch
        {
            AssignmentOperator assignmentOperator => AnalyzeAssignment(assignmentOperator, state),
            Identifier identifier => AnalyzeIdentifier(identifier, state),
            _ => new FlowState(expression.Children.Select(e => state = AnalyzeExpression(e, state)).LastOrDefault(new FlowState()))
        };
    }

    private FlowState AnalyzeBlock(Block block, FlowState state) => BindState(block, AnalyzeStatements(block.Statements, state));

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
        BindState(@if.Condition, state);
        AnalyzeExpression(@if.Condition, state);
        var thenState = AnalyzeStatement(@if.ThenBranch, new FlowState(state));
        var elseState = @if.ElseBranch != null ? AnalyzeStatement(@if.ElseBranch.Branch, new FlowState(state)) : new FlowState(state);
        return BindState(@if, state.Merge(thenState.Merge(elseState)));
    }
    
    private FlowState AnalyzeAfter(After after, FlowState state)
    {
        var bodyState = new FlowState(AnalyzeExpression(after.Duration, state));
        var body = AnalyzeStatement(after.Body, bodyState);
        return BindState(after, state.Merge(body));
    }

    private FlowState AnalyzeWhile(While @while, FlowState state)
    {
        var bodyState = new FlowState(AnalyzeExpression(@while.Condition, state));
        var body = AnalyzeStatement(@while.Body, bodyState);
        return BindState(@while, state.Merge(body));
    }

    private FlowState AnalyzeFor(For @for, FlowState state)
    {
        var bodyState = new FlowState(AnalyzeExpression(@for.CollectionExpression, state));
        foreach (var symbol in @for.Names.Select(name => semanticModel.GetDeclarationSymbol(name)).OfType<Symbol>())
        {
            bodyState.DefinitelyInitialized.Add(symbol);
            bodyState.MaybeInitialized.Add(symbol);
        }

        var body = AnalyzeStatement(@for.Body, bodyState);
        return BindState(@for, state.Merge(body));
    }
    
    private FlowState AnalyzeBreak(Break @break, FlowState state) => BindState(@break, new FlowState(state) { IsUnreachable = true });
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
        state = AnalyzeExpression(assignment.Right, state);
        if (assignment.Left is Identifier identifier)
        {
            var symbol = semanticModel.GetSymbol(identifier);
            if (symbol == null || assignment.Operator.Kind != SyntaxKind.Equals)
                return BindState(assignment, state);

            state.DefinitelyInitialized.Add(symbol);
            state.MaybeInitialized.Add(symbol);
        }
        else
        {
            state = AnalyzeExpression(assignment.Left, state);
        }

        return BindState(assignment, state);
    }

    private FlowState AnalyzeIdentifier(Identifier identifier, FlowState state)
    {
        var symbol = semanticModel.GetSymbol(identifier);
        if (symbol is not { IsValueSymbol: true } || state.DefinitelyInitialized.Contains(symbol))
            return BindState(identifier, state);

        if (state.MaybeInitialized.Contains(symbol))
            _diagnostics.Error(identifier, InternalCodes.UseOfMaybeUninitialized, $"Variable '{symbol.Name}' might not be initialized on this path.");
        else
            _diagnostics.Error(identifier, InternalCodes.UseOfUninitialized, $"Use of uninitialized variable '{symbol.Name}'.");

        return BindState(identifier, state);
    }

    private FlowState BindState(Node node, FlowState state)
    {
        _states[node] = state;
        return state;
    }
}