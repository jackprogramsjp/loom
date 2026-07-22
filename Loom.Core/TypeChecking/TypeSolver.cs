using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Core.Text;
using Loom.Core.TypeChecking.Types;
using ArrayType = Loom.Core.TypeChecking.Types.ArrayType;
using FunctionType = Loom.Core.TypeChecking.Types.FunctionType;
using IndexedType = Loom.Core.TypeChecking.Types.IndexedType;
using IntersectionType = Loom.Core.TypeChecking.Types.IntersectionType;
using PrimitiveType = Loom.Core.TypeChecking.Types.PrimitiveType;
using Type = Loom.Core.TypeChecking.Types.Type;
using TypeParameter = Loom.Core.TypeChecking.Types.TypeParameter;
using UnionType = Loom.Core.TypeChecking.Types.UnionType;

namespace Loom.Core.TypeChecking;

public sealed class TypeSolver(DiagnosticBag diagnostics)
{
    private sealed record TypeConstraint
    {
        public Type Actual { get; }
        public Type Expected { get; }
        public LocationSpan Span { get; }

        public TypeConstraint(Type actual, Type expected, LocationSpan span)
        {
            ArgumentNullException.ThrowIfNull(actual);
            ArgumentNullException.ThrowIfNull(expected);
            Actual = actual;
            Expected = expected;
            Span = span;
        }
    }

    public DiagnosticBag Diagnostics { get; } = diagnostics;

    private readonly List<TypeConstraint> _constraints = [];
    private readonly Dictionary<NodeId, Type> _nodeTypes = [];
    private readonly Dictionary<int, Type> _substitutions = [];
    private int _nextVariableId;

    public bool CheckCircular(ref Type type, Token name)
    {
        switch (type)
        {
            case UnionType unionType:
            {
                var members = unionType.Types.ToList();
                var circular = false;
                for (var i = 0; i < members.Count; i++)
                {
                    var member = members[i];
                    if (!CheckCircular(ref member, name)) continue;

                    members[i] = member;
                    circular = true;
                }

                if (circular)
                    type = new UnionType(members);

                return circular;
            }
            case IntersectionType intersectionType:
            {
                var members = intersectionType.Types.ToList();
                var circular = false;
                for (var i = 0; i < members.Count; i++)
                {
                    var member = members[i];
                    if (!CheckCircular(ref member, name)) continue;

                    members[i] = member;
                    circular = true;
                }

                if (circular)
                    type = new IntersectionType(members);

                return circular;
            }

            case TypeVariable:
                type = PrimitiveType.Never;
                ReportInfiniteType(name.GetLocation(), name.Text);
                return true;
        }

        return false;
    }

    public static Type Transform(Type type, Converter<Type, Type> fn, Type? defaultValue = null, bool simplify = true)
    {
        var transformed = type switch
        {
            IndexedType indexedType => new IndexedType(fn(indexedType.Target), fn(indexedType.Index)),
            ArrayType arrayType => new ArrayType(fn(arrayType.ElementType), arrayType.IsMutable),
            InterfaceType interfaceType => new InterfaceType(
                interfaceType.Name,
                interfaceType.Constraints.ConvertAll(fn).OfType<InterfaceType>().ToList(),
                (ObjectType)fn(interfaceType.ObjectType),
                interfaceType.TraitMethodNames
            ),
            ObjectType objectType => new ObjectType(
                objectType.Indexer != null
                    ? new ObjectIndexer(objectType.Indexer.IsMutable, fn(objectType.Indexer.KeyType), fn(objectType.Indexer.ValueType))
                    : null,
                objectType.Properties.ConvertAll(p => new ObjectProperty(p.IsMutable, p.Name, fn(p.ValueType)))
            ),
            IntersectionType intersectionType => new IntersectionType(intersectionType.Types.ConvertAll(fn)),
            UnionType unionType => new UnionType(unionType.Types.ConvertAll(fn)),
            FunctionType functionType => new FunctionType(
                functionType.TypeParameters,
                functionType.ParameterTypes.ConvertAll(fn),
                fn(functionType.ReturnType)
            ),
            GenericType genericType => new GenericType(
                genericType.Declaration,
                genericType.Parameters,
                fn(genericType.UnderlyingType)
            ),
            InstantiatedType instantiatedType => new InstantiatedType(
                instantiatedType.GenericType,
                instantiatedType.Arguments.ConvertAll(fn)
            ),
            _ => defaultValue ?? type
        };

        return simplify ? TypeSimplifier.Simplify(transformed) : transformed;
    }

    public void SetType(Node node, Type type) => _nodeTypes[node.Id] = type;

    public Type GetType(Node node)
    {
        if (_nodeTypes.TryGetValue(node.Id, out var type))
            return type;

        var variable = CreateTypeVariable();
        _nodeTypes.Add(node.Id, variable);
        return variable;
    }

    public void AddConstraint(Type actual, Type expected, Node node) => AddConstraint(actual, expected, node.LocationSpan);
    public void AddConstraint(Type actual, Type expected, LocationSpan span) => _constraints.Add(new TypeConstraint(actual, expected, span));

    public bool SolveConstraints()
    {
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var constraint in _constraints)
            {
                var resolvedA = Substitute(constraint.Actual);
                var resolvedB = Substitute(constraint.Expected);
                if (!TryUnify(resolvedA, resolvedB, constraint.Span, out var updated))
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
            (FunctionType f1, FunctionType f2) => UnifyFunctionTypes(f1, f2, span, out updated),
            (ObjectType o1, ObjectType o2) => UnifyObjectTypes(o1, o2, span, out updated),
            (ObjectType o, InterfaceType i) => UnifyObjectWithInterface(o, i, span, out updated),
            (InterfaceType i, ObjectType o) => UnifyObjectWithInterface(o, i, span, out updated),
            (InterfaceType i1, InterfaceType i2) => UnifyInterfaceTypes(i1, i2, span, out updated),
            (TypeParameter p1, TypeParameter p2) => UnifyTypeParameters(p1, p2, span, out updated),

            _ when a.IsAssignableTo(b) => true,
            _ => ReportTypeMismatch(a, b, span)
        };
    }

    private bool UnifyTypeParameters(TypeParameter p1, TypeParameter p2, LocationSpan span, out bool updated)
    {
        updated = false;
        if (p1.Constraint != null && p2.Constraint != null && !p1.Constraint.IsAssignableTo(p2.Constraint))
            return ReportTypeMismatch(p1, p2, span);

        if (p1.Constraint == null || p2.Constraint == null)
            return true;

        return TryUnify(p1.Constraint, p2.Constraint, span, out updated);
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
            return ReportInfiniteType(span, type.ToString());

        _substitutions[variable.Id] = type;
        updated = true;
        return true;
    }

    private bool UnifyInstantiatedPair(InstantiatedType a, InstantiatedType b, LocationSpan span, out bool updated)
    {
        updated = false;
        if (!a.GenericType.Equals(b.GenericType) || a.Arguments.Count != b.Arguments.Count)
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

    private bool UnifyObjectTypes(ObjectType a, ObjectType b, LocationSpan span, out bool updated)
    {
        updated = false;
        var success = true;

        if (a.Indexer != null && b.Indexer != null)
        {
            if (!TryUnify(a.Indexer.KeyType, b.Indexer.KeyType, span, out var keyUpdated))
                success = false;
            else if (keyUpdated)
                updated = true;

            if (!TryUnify(a.Indexer.ValueType, b.Indexer.ValueType, span, out var valueUpdated))
                success = false;
            else if (valueUpdated)
                updated = true;

            if (a.Indexer.IsMutable != b.Indexer.IsMutable)
            {
                if (!ReportTypeMismatch(a, b, span, $"Indexer types match, but indexer mutability of type '{a}' does not match that of type '{b}'."))
                    success = false;
            }
        }
        else if (a.Indexer == null && b.Indexer != null)
        {
            var noIndexerType = a.Indexer == null ? a : b;
            var indexerType = a.Indexer != null ? a : b;
            if (!ReportTypeMismatch(a, b, span, $"Type '{noIndexerType}' is missing indexer from type '{indexerType}'"))
                success = false;
        }

        var aProps = a.Properties.ToDictionary(p => p.Name);
        var bProps = b.Properties.ToDictionary(p => p.Name);
        var allPropertyNames = aProps.Keys.Union(bProps.Keys).ToList();
        foreach (var name in allPropertyNames)
        {
            if (!aProps.TryGetValue(name, out var propA) || !bProps.TryGetValue(name, out var propB)) continue;

            if (!TryUnify(propA.ValueType, propB.ValueType, span, out var propUpdated))
                success = false;
            else if (propUpdated)
                updated = true;

            if (propA.IsMutable == propB.IsMutable) continue;
            if (!ReportTypeMismatch(a, b, span, $"Property types match, but mutability of property '{name}' does not match that of type '{b}'."))
                success = false;
        }

        return success;
    }

    private bool UnifyObjectWithInterface(ObjectType objectType, InterfaceType interfaceType, LocationSpan span, out bool updated)
    {
        updated = false;
        return TryUnify(objectType, interfaceType.AssignabilityType, span, out updated);
    }

    private bool UnifyInterfaceTypes(InterfaceType a, InterfaceType b, LocationSpan span, out bool updated)
    {
        updated = false;
        return TryUnify(a.AssignabilityType, b.AssignabilityType, span, out updated);
    }

    private bool UnifyFunctionTypes(FunctionType a, FunctionType b, LocationSpan span, out bool updated)
    {
        updated = false;
        if (a.TypeParameters.Count != b.TypeParameters.Count || a.ParameterTypes.Count != b.ParameterTypes.Count)
            return ReportTypeMismatch(a, b, span);

        var success = true;
        for (var i = 0; i < a.TypeParameters.Count; i++)
        {
            if (!TryUnify(a.TypeParameters[i], b.TypeParameters[i], span, out var constraintUpdated))
                success = false;
            else if (constraintUpdated)
                updated = true;
        }

        var freshVars = a.TypeParameters.Select(_ => CreateTypeVariable()).ToList();
        var aMapping = a.TypeParameters.Zip(freshVars).ToDictionary(p => p.First, p => p.Second);
        var bMapping = b.TypeParameters.Zip(freshVars).ToDictionary(p => p.First, p => p.Second);
        var aParamTypes = a.ParameterTypes.ConvertAll(t => SubstituteTypeParameters(aMapping, t));
        var bParamTypes = b.ParameterTypes.ConvertAll(t => SubstituteTypeParameters(bMapping, t));
        var aReturnType = SubstituteTypeParameters(aMapping, a.ReturnType);
        var bReturnType = SubstituteTypeParameters(bMapping, b.ReturnType);
        for (var i = 0; i < aParamTypes.Count; i++)
        {
            if (!TryUnify(aParamTypes[i], bParamTypes[i], span, out var paramUpdated))
                success = false;
            else if (paramUpdated)
                updated = true;
        }

        if (!TryUnify(aReturnType, bReturnType, span, out var returnUpdated))
            success = false;
        else if (returnUpdated)
            updated = true;

        return success;
    }

    private static bool OccursIn(TypeVariable variable, Type type) =>
        type switch
        {
            TypeVariable tv => tv.Id == variable.Id,
            IndexedType indexedType => OccursIn(variable, indexedType.Target) || OccursIn(variable, indexedType.Index),
            InterfaceType i => i.Constraints.Any(t => OccursIn(variable, t)) || OccursIn(variable, i.ObjectType),
            ObjectType obj => obj.Indexer != null && (OccursIn(variable, obj.Indexer.KeyType) || OccursIn(variable, obj.Indexer.ValueType))
                || obj.Properties.Any(p => OccursIn(variable, p.ValueType)),
            GenericType generic => OccursIn(variable, generic.UnderlyingType),
            InstantiatedType inst => inst.Arguments.Any(a => OccursIn(variable, a)),
            IntersectionType inter => inter.Types.Any(t => OccursIn(variable, t)),
            UnionType union => union.Types.Any(t => OccursIn(variable, t)),
            FunctionType fn => fn.TypeParameters.Any(p => OccursIn(variable, p))
                || fn.ParameterTypes.Any(t => OccursIn(variable, t))
                || OccursIn(variable, fn.ReturnType),
            TypeParameter tp => tp.Constraint != null && OccursIn(variable, tp.Constraint) || tp.DefaultType != null && OccursIn(variable, tp.DefaultType),
            _ => false
        };

    private void ApplySubstitutions()
    {
        if (_substitutions.Count == 0) return;

        foreach (var nodeId in _nodeTypes.Keys.ToList())
            _nodeTypes[nodeId] = Substitute(_nodeTypes[nodeId]);
    }

    private static Type SubstituteTypeParameters(Dictionary<TypeParameter, TypeVariable> mapping, Type type) =>
        type switch
        {
            TypeParameter typeParameter => mapping.TryGetValue(typeParameter, out var tv) ? tv : type,
            _ => Transform(type, t => SubstituteTypeParameters(mapping, t))
        };

    private Type Substitute(Type type) => Substitute(type, new Dictionary<Type, Type>());

    private Type Substitute(Type type, Dictionary<Type, Type> visitedSubstitutions)
    {
        if (visitedSubstitutions.TryGetValue(type, out var existing))
            return existing;

        var original = type;
        var visited = new HashSet<int>();
        while (type is TypeVariable tv && _substitutions.TryGetValue(tv.Id, out var replacement))
        {
            if (!visited.Add(tv.Id)) break;
            type = replacement;
        }

        visitedSubstitutions[original] = type;
        type = Transform(type, t => Substitute(t, visitedSubstitutions), null, false);
        if (type is InstantiatedType instantiated && instantiated.Arguments.All(a => a is not TypeVariable))
            type = instantiated.Expand();

        type = TypeSimplifier.Simplify(type);
        visitedSubstitutions[original] = type;
        return type;
    }

    private bool ReportTypeMismatch(Type a, Type b, LocationSpan span, string? info = null)
    {
        Diagnostics.Error(
            span,
            InternalCodes.TypeMismatch,
            $"Type '{a}' is not assignable to type '{b}'.{(info != null ? " " + info : "")}"
        );

        return false;
    }

    internal bool ReportInfiniteType(LocationSpan span, string name)
    {
        Diagnostics.Error(span, InternalCodes.InfiniteType, $"Type '{name}' circularly references itself.");
        return false;
    }

    private TypeVariable CreateTypeVariable() => new(Interlocked.Increment(ref _nextVariableId));
}