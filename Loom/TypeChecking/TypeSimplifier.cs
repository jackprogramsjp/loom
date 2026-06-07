using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public static class TypeSimplifier
{
    public static Type Simplify(Type type) =>
        type switch
        {
            UnionType union => SimplifyUnion(union),
            IntersectionType intersection => SimplifyIntersection(intersection),
            InstantiatedType instantiated => Simplify(instantiated.Expand()),
            GenericType generic => Simplify(generic.Underlying),
            _ => type
        };

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
        
        if (distinct.Any(t => t is UnionType))
            return Simplify(DistributeIntersection(distinct));

        var absorbed = ApplyAbsorption(distinct, isUnion: false);
        if (absorbed.Count > 1 && absorbed.All(t => t is PrimitiveType))
            return PrimitiveType.Never;
        
        return absorbed.Count == 1 ? absorbed.First() : new IntersectionType(absorbed);
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
        (a, b) switch
        {
            // _ when Type.IsNever(a) => true,
            // _ when Type.IsNever(b) => false,

            (UnionType ua, UnionType ub) => ua.Types.All(t => IsSubsetOf(t, ub)),
            (UnionType ua, _) => ua.Types.All(t => IsSubsetOf(t, b)),
            (_, UnionType ub) => ub.Types.Any(t => IsSubsetOf(a, t)),
            (IntersectionType ia, _) => ia.Types.All(t => IsSubsetOf(t, b)),
            (_, IntersectionType ib) => ib.Types.All(t => IsSubsetOf(a, t)),
            _ => a.Equals(b) || a.IsAssignableTo(b)
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