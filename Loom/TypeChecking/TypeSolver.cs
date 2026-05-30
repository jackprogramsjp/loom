using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public class TypeSolver(DiagnosticBag diagnostics)
{
    private record TypeConstraint(Type A, Type B, LocationSpan Span);

    private readonly List<TypeConstraint> _constraints = [];
    private readonly Dictionary<NodeId, Type> _nodeTypes = [];
    private int _nextVariableId;

    public void SetType(Node node, Type type) => _nodeTypes[node.Id] = type;

    public Type GetType(Node node)
    {
        if (_nodeTypes.TryGetValue(node.Id, out var type))
            return type;

        var variable = CreateTypeVariable();
        _nodeTypes.Add(node.Id, variable);
        return variable;
    }
    
    public void AddConstraint(Type a, Type b, LocationSpan span) => _constraints.Add(new TypeConstraint(a, b, span));

    public bool SolveConstraints()
    {
        var substitutions = new Dictionary<int, Type>();
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var (a, b, span) in _constraints)
            {
                var resolvedA = Substitute(a, substitutions);
                var resolvedB = Substitute(b, substitutions);
                if (!TryUnify(resolvedA, resolvedB, span, substitutions, out var updated))
                    return false;

                if (updated)
                    changed = true;
            }
        }

        ApplySubstitutions(substitutions);
        return true;
    }

    private bool TryUnify(Type a, Type b, LocationSpan span, Dictionary<int, Type> substitutions, out bool updated)
    {
        updated = false;
        if (a is TypeVariable varA && b is TypeVariable varB)
        {
            if (varA.Id == varB.Id)
                return true;

            substitutions[varA.Id] = varB;
            updated = true;
            return true;
        }

        if (a is TypeVariable var)
            return BindVariable(var, b, span, substitutions, out updated);

        if (b is TypeVariable var2)
            return BindVariable(var2, a, span, substitutions, out updated);

        if (a.IsAssignableTo(b))
            return true;

        diagnostics.Error(span, InternalCodes.TypeMismatch, $"Type '{a}' is not assignable to type '{b}'.");
        return false;
    }

    private bool BindVariable(TypeVariable variable,
        Type type,
        LocationSpan span,
        Dictionary<int, Type> substitutions,
        out bool updated)
    {
        updated = false;
        if (type is TypeVariable tv && tv.Id == variable.Id)
        {
            diagnostics.Error(span, InternalCodes.InfiniteType, "Type is infinitely recursive.");
            return false;
        }

        substitutions[variable.Id] = type;
        updated = true;
        return true;
    }

    private void ApplySubstitutions(Dictionary<int, Type> substitutions)
    {
        if (substitutions.Count == 0) return;

        var nodeIds = _nodeTypes.Keys.ToList();
        foreach (var nodeId in nodeIds)
            _nodeTypes[nodeId] = Substitute(_nodeTypes[nodeId], substitutions);
    }
    
    private static Type Substitute(Type type, Dictionary<int, Type> substitutions)
    {
        var visited = new HashSet<int>();
        while (type is TypeVariable tv && substitutions.TryGetValue(tv.Id, out var replacement))
        {
            if (!visited.Add(tv.Id))
                break;
            type = replacement;
        }

        return type;
    }
    
    private TypeVariable CreateTypeVariable() => new(Interlocked.Increment(ref _nextVariableId));
}