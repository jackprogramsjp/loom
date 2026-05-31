using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using IntersectionType = Loom.TypeChecking.Types.IntersectionType;
using Type = Loom.TypeChecking.Types.Type;
using TypeParameter = Loom.TypeChecking.Types.TypeParameter;
using UnionType = Loom.TypeChecking.Types.UnionType;

namespace Loom.TypeChecking;

public class TypeSolver(DiagnosticBag diagnostics)
{
    private record TypeConstraint(Type A, Type B, LocationSpan Span);

    private readonly List<TypeConstraint> _constraints = [];
    private readonly Dictionary<NodeId, Type> _nodeTypes = [];
    private readonly Dictionary<int, Type> _substitutions = [];
    private readonly Dictionary<TypeParameter, TypeVariable> _parameterVariables = [];
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
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var (a, b, span) in _constraints)
            {
                var resolvedA = Substitute(a);
                var resolvedB = Substitute(b);
                if (!TryUnify(resolvedA, resolvedB, span, out var updated))
                    return false;

                if (updated)
                    changed = true;
            }
        }

        ApplySubstitutions();
        return true;
    }

    private bool TryUnify(Type a, Type b, LocationSpan span, out bool updated)
    {
        updated = false;
        return (a, b) switch
        {
            (TypeVariable va, TypeVariable vb) => UnifyBothVariables(va, vb, out updated),
            (TypeVariable v, _) => BindVariable(v, b, span, out updated),
            (_, TypeVariable v) => BindVariable(v, a, span, out updated),
            (InstantiatedType i1, InstantiatedType i2) => UnifyInstantiatedPair(i1, i2, span, out updated),
            (InstantiatedType i, GenericType g) => UnifyInstantiatedWithGeneric(i, g, span, out updated),
            (GenericType g, InstantiatedType i) => UnifyInstantiatedWithGeneric(i, g, span, out updated),

            _ when a.IsAssignableTo(b) => true,
            _ => ReportTypeMismatch(a, b, span)
        };
    }

    private bool UnifyBothVariables(TypeVariable va, TypeVariable vb, out bool updated)
    {
        if (va.Id == vb.Id)
        {
            updated = false;
            return true;
        }

        _substitutions[va.Id] = vb;
        updated = true;
        return true;
    }

    private bool BindVariable(TypeVariable variable, Type type, LocationSpan span, out bool updated)
    {
        updated = false;
        if (OccursIn(variable, type))
            return ReportInfiniteType(span);

        _substitutions[variable.Id] = type;
        updated = true;
        return true;
    }

    private bool UnifyInstantiatedPair(InstantiatedType a, InstantiatedType b, LocationSpan span, out bool updated)
    {
        updated = false;
        if (!a.Generic.Equals(b.Generic) || a.Arguments.Count != b.Arguments.Count)
            return ReportTypeMismatch(a, b, span);

        var success = true;
        for (var i = 0; i < a.Arguments.Count; i++)
        {
            if (!TryUnify(a.Arguments[i], b.Arguments[i], span, out var argUpdated))
                success = false;
            else if (argUpdated)
                updated = true;
        }

        return success;
    }

    private bool UnifyInstantiatedWithGeneric(InstantiatedType instantiated,
        GenericType generic,
        LocationSpan span,
        out bool updated)
    {
        updated = false;
        if (!instantiated.Generic.Equals(generic) || instantiated.Arguments.Count != generic.Parameters.Count)
            return ReportTypeMismatch(instantiated, generic, span);

        var success = true;
        for (var i = 0; i < instantiated.Arguments.Count; i++)
        {
            var paramVar = GetOrCreateParameterVariable(generic.Parameters[i]);
            if (!TryUnify(instantiated.Arguments[i], paramVar, span, out var argUpdated))
                success = false;
            else if (argUpdated)
                updated = true;
        }

        return success;
    }

    private static bool OccursIn(TypeVariable variable, Type type) =>
        type switch
        {
            TypeVariable tv => tv.Id == variable.Id,
            InstantiatedType inst => inst.Arguments.Any(a => OccursIn(variable, a)),
            IntersectionType inter => inter.Types.Any(t => OccursIn(variable, t)),
            UnionType union => union.Types.Any(t => OccursIn(variable, t)),
            _ => false
        };

    private TypeVariable GetOrCreateParameterVariable(TypeParameter param)
    {
        if (_parameterVariables.TryGetValue(param, out var variable))
            return variable;

        variable = CreateTypeVariable();
        _parameterVariables[param] = variable;
        return variable;
    }

    private void ApplySubstitutions()
    {
        if (_substitutions.Count == 0) return;

        foreach (var nodeId in _nodeTypes.Keys.ToList())
            _nodeTypes[nodeId] = Substitute(_nodeTypes[nodeId]);
    }

    private Type Substitute(Type type)
    {
        var visited = new HashSet<int>();
        while (type is TypeVariable tv && _substitutions.TryGetValue(tv.Id, out var replacement))
        {
            if (!visited.Add(tv.Id)) break;
            type = replacement;
        }

        type = type switch
        {
            InstantiatedType inst => new InstantiatedType(inst.Generic, inst.Arguments.ConvertAll(Substitute)),
            IntersectionType inter => new IntersectionType(inter.Types.ConvertAll(Substitute)),
            UnionType union => new UnionType(union.Types.ConvertAll(Substitute)),
            _ => type
        };
        
        if (type is InstantiatedType instantiated && instantiated.Arguments.All(a => a is not TypeVariable))
            type = instantiated.Expand();

        return type;
    }

    private bool ReportTypeMismatch(Type a, Type b, LocationSpan span)
    {
        diagnostics.Error(
            span,
            InternalCodes.TypeMismatch,
            $"Type '{a}' is not assignable to type '{b}'."
        );

        return false;
    }

    private bool ReportInfiniteType(LocationSpan span)
    {
        diagnostics.Error(span, InternalCodes.InfiniteType, "Type is infinitely recursive.");
        return false;
    }

    private TypeVariable CreateTypeVariable() => new(Interlocked.Increment(ref _nextVariableId));
}