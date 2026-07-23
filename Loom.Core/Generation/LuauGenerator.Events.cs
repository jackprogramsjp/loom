using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Core.Text;
using Loom.Luau;
using Loom.Luau.AST;
using BinaryOperator = Loom.Luau.AST.BinaryOperator;
using ExpressionStatement = Loom.Core.Parsing.AST.ExpressionStatement;
using Identifier = Loom.Core.Parsing.AST.Identifier;
using PropertyAccess = Loom.Luau.AST.PropertyAccess;
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
        var leftSymbol = _semanticModel.GetSymbol(assignmentOperator.Left);
        if (leftSymbol is { Kind: SymbolKind.Event } && assignmentOperator.Operator.Kind is SyntaxKind.PlusEquals or SyntaxKind.MinusEquals)
            return GenerateEventAssignment(assignmentOperator, leftSymbol);

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

    private LuauExpression GenerateEventAssignment(AssignmentOperator assignmentOperator, Symbol eventSymbol)
    {
        var eventTarget = Visit(assignmentOperator.Left);
        return assignmentOperator.Operator.Kind == SyntaxKind.PlusEquals
            ? GenerateEventConnect(assignmentOperator, eventTarget, eventSymbol)
            : GenerateEventDisconnect(assignmentOperator, eventSymbol);
    }

    private LuauExpression GenerateEventConnect(AssignmentOperator assignmentOperator, LuauExpression eventTarget, Symbol eventSymbol)
    {
        var function = assignmentOperator.Right;
        var luauFunction = WrapAnonymousFunction(function, Visit(function), new UnitType());
        var connect = new Call(new PropertyAccess(eventTarget, ["Connect"]), [luauFunction], true);
        if (luauFunction is AnonymousFunction || function is not Identifier identifier || _semanticModel.GetSymbol(identifier) is not { } functionSymbol)
            return connect;

        if (assignmentOperator.Parent is EqualsValueClause { Parent: VariableDeclaration declaration })
        {
            _eventConnections.Track(eventSymbol, functionSymbol, new Luau.AST.Identifier(declaration.Name.Text));
            return connect;
        }

        var connectionVariable = _state.PushToVariable($"{identifier.Name.Text}_conn", connect);
        _eventConnections.Track(eventSymbol, functionSymbol, connectionVariable);

        return connectionVariable;
    }

    private LuauExpression GenerateEventDisconnect(AssignmentOperator assignmentOperator, Symbol eventSymbol)
    {
        var function = assignmentOperator.Right;
        if (function is Identifier identifier
            && _semanticModel.GetSymbol(identifier) is { } functionSymbol
            && _eventConnections.TryGetConnection(eventSymbol, functionSymbol, out var connection))
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