using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Core.Text;
using Loom.Luau;
using Loom.Luau.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using CorePropertyAccess = Loom.Core.Parsing.AST.PropertyAccess;
using ExpressionStatement = Loom.Core.Parsing.AST.ExpressionStatement;
using Identifier = Loom.Core.Parsing.AST.Identifier;
using PropertyAccess = Loom.Luau.AST.PropertyAccess;
using QualifiedName = Loom.Core.Parsing.AST.QualifiedName;
using TypeName = Loom.Luau.AST.TypeName;

namespace Loom.Core.Generation;

public sealed partial class LuauGenerator
{
    public override LuauNode VisitEventDeclaration(EventDeclaration eventDeclaration)
    {
        // TODO: generic events
        if (eventDeclaration.TypeParameters != null)
            _diagnostics.NotImplemented(eventDeclaration.TypeParameters, "Generic event declarations are not yet supported.");

        _semanticModel.RuntimeReferences += 2;
        var parameterTypes = eventDeclaration.Parameters?.ParameterList.ConvertAll(p => Visit(p.ColonTypeClause!.Type)) ?? [];
        var eventType = LuauFactory.QualifyRuntimeType(new TypeName("Event", parameterTypes));
        return new ConstVariable(eventDeclaration.Name.Text, eventType, LuauFactory.RuntimeLibraryCall(["Event", "new"], []));
    }

    public override LuauNode VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Operator.Kind is SyntaxKind.PlusEquals or SyntaxKind.MinusEquals
            && ResolveEventTarget(assignmentOperator.Left) is { } eventTarget)
            return GenerateEventAssignment(assignmentOperator, eventTarget);

        if (assignmentOperator.Parent is ExpressionStatement)
            return VisitBinaryOperator(assignmentOperator);

        if (assignmentOperator.Left is Identifier)
        {
            var binary = (BinaryOperator)VisitBinaryOperator(assignmentOperator);
            var assignmentStatement = new Luau.AST.ExpressionStatement(binary);
            _state.Prereq(assignmentStatement);

            return binary.Left;
        }

        var left = Visit(assignmentOperator.Left);
        var right = Visit(assignmentOperator.Right);
        if (assignmentOperator.Parent is EqualsValueClause { Parent: NamedDeclaration declaration })
        {
            var identifierAssignment = new BinaryOperator(left, "=", new Luau.AST.Identifier(declaration.Name.Text));
            _state.Postreq(new Luau.AST.ExpressionStatement(identifierAssignment));

            return right;
        }

        var assigned = _state.PushToVariable("_assigned", right);
        var boundAssignment = new BinaryOperator(left, "=", assigned);
        _state.Prereq(new Luau.AST.ExpressionStatement(boundAssignment));

        return assigned;
    }

    private EventTarget? ResolveEventTarget(Expression left)
    {
        if (_semanticModel.GetSymbol(left) is { Kind: SymbolKind.Event } globalEventSymbol)
            return new EventTarget(null, globalEventSymbol);

        if (_semanticModel.GetPropertySymbol(left) is not { Kind: SymbolKind.Event } propertySymbol)
            return null;

        return new EventTarget(GetInstanceKey(left), propertySymbol);
    }

    private object? GetInstanceKey(Expression left) => left switch
    {
        CorePropertyAccess { Expression: Identifier identifier } => _semanticModel.GetSymbol(identifier),
        QualifiedName { Identifier: var identifier } => _semanticModel.GetSymbol(identifier),
        _ => new object()
    };

    private LuauExpression GenerateEventAssignment(AssignmentOperator assignmentOperator, EventTarget eventTarget)
    {
        var connectionTarget = Visit(assignmentOperator.Left);
        return assignmentOperator.Operator.Kind == SyntaxKind.PlusEquals
            ? GenerateEventConnect(assignmentOperator, connectionTarget, eventTarget)
            : GenerateEventDisconnect(assignmentOperator, eventTarget);
    }

    private LuauExpression GenerateEventConnect(AssignmentOperator assignmentOperator, LuauExpression connectionTarget, EventTarget eventTarget)
    {
        var function = assignmentOperator.Right;
        var luauFunction = WrapAnonymousFunction(function, Visit(function), new UnitType());
        var connect = new Call(new PropertyAccess(connectionTarget, ["Connect"]), [luauFunction], true);
        if (luauFunction is AnonymousFunction || function is not Identifier identifier || _semanticModel.GetSymbol(identifier) is not { } functionSymbol)
            return connect;

        if (assignmentOperator.Parent is EqualsValueClause { Parent: VariableDeclaration declaration })
        {
            _eventConnections.Track(eventTarget, functionSymbol, new Luau.AST.Identifier(declaration.Name.Text));
            return connect;
        }

        var connectionVariable = _state.PushToVariable($"{identifier.Name.Text}_conn", connect);
        _eventConnections.Track(eventTarget, functionSymbol, connectionVariable);

        return connectionVariable;
    }

    private LuauExpression GenerateEventDisconnect(AssignmentOperator assignmentOperator, EventTarget eventTarget)
    {
        var function = assignmentOperator.Right;
        if (function is Identifier identifier
            && _semanticModel.GetSymbol(identifier) is { } functionSymbol
            && _eventConnections.TryGetConnection(eventTarget, functionSymbol, out var connection))
            return new Call(new PropertyAccess(connection, ["Disconnect"]), [], true);

        if (function is not Identifier && IsMethodReference(function))
        {
            _diagnostics.Error(
                function,
                InternalCodes.AnonymousEventDisconnect,
                "Cannot disconnect a function reference that gets wrapped into a new Luau closure on every connection.",
                "store the connection returned from '+=' and disconnect that instead."
            );

            return new NilLiteral();
        }

        _diagnostics.Error(
            assignmentOperator,
            InternalCodes.UnresolvedEventDisconnect,
            "No event connection exists for this function, connect it with '+=' before disconnecting it."
        );

        return new NilLiteral();
    }
}
