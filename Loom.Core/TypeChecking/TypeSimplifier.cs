using System.Runtime.CompilerServices;
using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public static class TypeSimplifier
{
    private static readonly ConditionalWeakTable<Type, Type> _simplifyCache = [];

    public static Type Simplify(Type type)
    {
        if (_simplifyCache.TryGetValue(type, out var cached))
            return cached;

        var simplified = type switch
        {
            InterfaceType interfaceType => interfaceType.Constraints.Count > 0
                ? new InterfaceType(
                    interfaceType.Name,
                    [],
                    (ObjectType)Simplify(new IntersectionType([interfaceType.ObjectType, ..interfaceType.Constraints.Select(Simplify)]))
                )
                : interfaceType,
            ObjectType objectType => SimplifyObject(objectType),
            UnionType union => SimplifyUnion(union),
            IntersectionType intersection => SimplifyIntersection(intersection),
            InstantiatedType instantiated => Simplify(instantiated.Expand()),
            GenericType generic => Simplify(generic.UnderlyingType),
            _ => type
        };

        _simplifyCache.Add(type, simplified);
        return simplified;
    }

    private static Type SimplifyObject(ObjectType objectType) =>
        objectType.Properties.Count == 0 && objectType.Indexer != null && objectType.Indexer.KeyType.Equals(PrimitiveType.Number)
            ? new ArrayType(objectType.Indexer.ValueType, objectType.Indexer.IsMutable)
            : objectType;

    private static Type SimplifyUnion(UnionType union)
    {
        var flattened = FlattenNestedUnions(union.Types.ConvertAll(Simplify));
        var distinct = RemoveDuplicates(flattened, isUnion: true);
        var absorbed = ApplyAbsorption(distinct, isUnion: true);
        if (!absorbed.Any(Type.IsNone))
            return absorbed.Count switch
            {
                0 => PrimitiveType.Never,
                1 => absorbed.First(),
                _ => new UnionType(absorbed)
            };

        var nonNullable = absorbed.FindAll(Type.IsDefined);
        return nonNullable.Count switch
        {
            0 => PrimitiveType.None,
            1 => new OptionalType(nonNullable.First()),
            _ => new OptionalType(SimplifyUnion(new UnionType(nonNullable)))
        };
    }

    private static Type SimplifyIntersection(IntersectionType intersection)
    {
        var flattened = FlattenNestedIntersections(intersection.Types.ConvertAll(Simplify));
        var distinct = RemoveDuplicates(flattened, isUnion: false);

        if (distinct.Count == 0 || distinct.Any(Type.IsNever))
            return PrimitiveType.Never;

        if (distinct.All(t => t is ObjectType or InterfaceType))
            return Simplify(MergeObjectTypes(distinct.ToList()));

        if (distinct.Any(t => t is UnionType))
            return Simplify(DistributeIntersection(distinct));

        var absorbed = ApplyAbsorption(distinct, isUnion: false);
        if (absorbed.Count > 1 && absorbed.All(t => t is PrimitiveType))
            return PrimitiveType.Never;

        return absorbed.Count == 1 ? absorbed.First() : new IntersectionType(absorbed);
    }

    private static Type MergeObjectTypes(List<Type> types)
    {
        var propertyDictionary = new Dictionary<string, ObjectProperty>();
        ObjectIndexer? mergedIndexer = null;

        var objectTypes = types.ConvertAll(t => t is InterfaceType i ? i.ObjectType : t).Cast<ObjectType>().ToList();
        foreach (var objectType in objectTypes)
        {
            foreach (var property in objectType.Properties)
            {
                if (propertyDictionary.TryGetValue(property.Name, out var existing))
                {
                    var valueType = Simplify(new IntersectionType([existing.ValueType, property.ValueType]));
                    var isMutable = existing.IsMutable && property.IsMutable;
                    propertyDictionary[property.Name] = new ObjectProperty(isMutable, property.Name, valueType);
                }
                else
                {
                    propertyDictionary[property.Name] = property;
                }
            }

            if (objectType.Indexer == null) continue;
            if (mergedIndexer == null)
            {
                mergedIndexer = new ObjectIndexer(objectType.Indexer.IsMutable, objectType.Indexer.KeyType, objectType.Indexer.ValueType);
            }
            else
            {
                var newKeyType = Simplify(new IntersectionType([mergedIndexer.KeyType, objectType.Indexer.KeyType]));
                var newValueType = Simplify(new IntersectionType([mergedIndexer.ValueType, objectType.Indexer.ValueType]));
                var newIsMutable = mergedIndexer.IsMutable && objectType.Indexer.IsMutable;
                mergedIndexer = new ObjectIndexer(newIsMutable, newKeyType, newValueType);
            }
        }

        var properties = propertyDictionary.Values.ToList();
        if (mergedIndexer == null || properties.Count != 0 || !mergedIndexer.KeyType.Equals(PrimitiveType.Number))
            return new ObjectType(mergedIndexer, properties);

        if (objectTypes.All(t => t is ArrayType))
            return new ArrayType(mergedIndexer.ValueType, mergedIndexer.IsMutable);

        return new ObjectType(mergedIndexer, properties);
    }

    private static Type DistributeIntersection(List<Type> types)
    {
        for (var i = 0; i < types.Count; i++)
        {
            if (types[i] is not UnionType union)
                continue;

            var rest = new List<Type>(types);
            rest.RemoveAt(i);

            var distributed = union.Types
                .Select(variant => new IntersectionType(new List<Type> { variant }.Concat(rest).ToList()))
                .Select(Simplify)
                .Where(simplified => !Type.IsNever(simplified))
                .ToList();

            return distributed.Count switch
            {
                0 => PrimitiveType.Never,
                1 => distributed.First(),
                _ => new UnionType(distributed)
            };
        }

        return new IntersectionType(types);
    }

    private static List<Type> ApplyAbsorption(List<Type> types, bool isUnion)
    {
        var result = new List<Type>();
        foreach (var t1 in types)
        {
            var isAbsorbed = false;
            foreach (var t2 in types.Where(t2 => !t1.Equals(t2)))
            {
                if (isUnion)
                {
                    if (!IsSubsetOf(t1, t2)) continue;
                    isAbsorbed = true;
                    break;
                }

                if (!IsSubsetOf(t2, t1)) continue;
                isAbsorbed = true;
                break;
            }

            if (isAbsorbed) continue;
            result.Add(t1);
        }

        return result;
    }

    private static bool IsSubsetOf(Type a, Type b) =>
        a switch
        {
            LiteralType la when b is LiteralType lb => Equals(la.Value, lb.Value),
            LiteralType lit when b is PrimitiveType prim => lit.Value switch
            {
                double or long or int => prim.Kind == PrimitiveTypeKind.Number,
                string => prim.Kind == PrimitiveTypeKind.String,
                bool => prim.Kind == PrimitiveTypeKind.Bool,
                null => prim.Kind is PrimitiveTypeKind.Unknown or PrimitiveTypeKind.None,
                _ => false
            },
            PrimitiveType when b is LiteralType => false,
            _ => (a, b) switch
            {
                (UnionType ua, UnionType ub) => ua.Types.All(t => IsSubsetOf(t, ub)),
                (UnionType ua, _) => ua.Types.All(t => IsSubsetOf(t, b)),
                (_, UnionType ub) => ub.Types.Any(t => IsSubsetOf(a, t)),
                (IntersectionType ia, _) => ia.Types.All(t => IsSubsetOf(t, b)),
                (_, IntersectionType ib) => ib.Types.All(t => IsSubsetOf(a, t)),
                _ => a.Equals(b) || a.IsAssignableTo(b)
            }
        };

    private static List<Type> RemoveDuplicates(List<Type> types, bool isUnion) =>
        types
            .Aggregate(
                new List<Type>(),
                (unique, item) =>
                {
                    if (!unique.Any(item.Equals))
                        unique.Add(item);

                    return unique;
                }
            )
            .Where(t => !isUnion || Type.IsNotNever(t))
            .ToList();

    private static List<Type> FlattenNestedUnions(List<Type> types) => types.SelectMany(t => t is UnionType union ? union.Types : [t]).ToList();

    private static List<Type> FlattenNestedIntersections(List<Type> types) =>
        types.SelectMany(t => t is IntersectionType intersection ? intersection.Types : [t]).ToList();
}