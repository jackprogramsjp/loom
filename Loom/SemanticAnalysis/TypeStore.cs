using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;
using Type = Loom.TypeChecking.Types.Type;
using Types_Type = Loom.TypeChecking.Types.Type;

namespace Loom.SemanticAnalysis;

public class TypeStore(DiagnosticBag diagnostics)
{
    private readonly List<TypeConstraint> _constraints = [];
    private readonly Dictionary<NodeId, Type> _nodeTypes = [];
    private int _nextVariableId;

    public void SetType(Node node, Type type) => _nodeTypes[node.Id] = type;

    public Type GetType(Node node)
    {
        if (_nodeTypes.TryGetValue(node.Id, out var type))
            return type;

        var variable = new TypeVariable(Interlocked.Increment(ref _nextVariableId));
        _nodeTypes.Add(node.Id, variable);
        return variable;
    }

    public Type CreateTypeVariable() => new TypeVariable(Interlocked.Increment(ref _nextVariableId));
    public void AddConstraint(Type a, Type b, LocationSpan span) => _constraints.Add(new TypeConstraint(a, b, span));

    public bool SolveConstraints()
    {
        var substitutions = new Dictionary<int, Type>();
        var changed = true;
        while (changed)
        {
            changed = false;

            foreach (var constraint in _constraints)
            {
                var a = Substitute(constraint.A, substitutions);
                var b = Substitute(constraint.B, substitutions);
                if (TryUnify(a,
                             b,
                             constraint.Span,
                             substitutions,
                             out var updated))
                {
                    if (updated) changed = true;
                }
                else
                {
                    return false; // error already reported
                }
            }
        }

        ApplySubstitutions(substitutions);
        return true;
    }

    private bool TryUnify(Type a,
                          Type b,
                          LocationSpan span,
                          Dictionary<int, Type> substitutions,
                          out bool updated)
    {
        updated = false;

        switch (a)
        {
            case TypeVariable varA when b is TypeVariable varB:
            {
                if (varA.Id == varB.Id)
                    return true;

                substitutions[varA.Id] = varB;
                updated = true;
                return true;
            }

            case TypeVariable var:
                return BindVariable(var,
                                    b,
                                    span,
                                    substitutions,
                                    out updated);
        }

        if (b is TypeVariable var2)
            return BindVariable(var2,
                                a,
                                span,
                                substitutions,
                                out updated);

        if (a.IsAssignableTo(b))
            return true;

        diagnostics.Error(span,
                          InternalCodes.TypeMismatch,
                          $"Type '{a}' is not assignable to type '{b}'.");

        return false;
    }

    private bool BindVariable(TypeVariable variable,
                              Type type,
                              LocationSpan span,
                              Dictionary<int, Type> substitutions,
                              out bool updated)
    {
        updated = false;

        if (OccursIn(variable, type))
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

    private static bool OccursIn(TypeVariable variable, Type type) => type is TypeVariable tv && tv.Id == variable.Id;

    private static Type Substitute(Type type, Dictionary<int, Type> substitutions)
    {
        while (type is TypeVariable tv && substitutions.TryGetValue(tv.Id, out var replacement))
            type = replacement;

        return type;
    }

    private record TypeConstraint(Type A, Type B, LocationSpan Span);

    private class TypeVariable(int id) : Types_Type
    {
        public int Id { get; } = id;

        public override bool Equals(Type? other) => other is TypeVariable v && Id == v.Id;
        
        public override string ToString() => $"T{Id}";
    }
}