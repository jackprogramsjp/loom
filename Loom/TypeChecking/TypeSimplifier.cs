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
            _ => type
        };

    private static Type SimplifyUnion(UnionType union)
    {
        var simplifiedMembers = union.Types.ConvertAll(Simplify);
        var absorbed = ApplyAbsorption(simplifiedMembers, isUnion: true);
        var distinct = RemoveDuplicates(absorbed);
        var flattened = FlattenNestedUnions(distinct);
        return flattened.Count switch
        {
            0 => PrimitiveType.Never,
            1 => flattened.First(),
            _ => new UnionType(flattened)
        };
    }

    private static Type SimplifyIntersection(IntersectionType intersection)
    {
        var simplifiedMembers = intersection.Types.ConvertAll(Simplify);
        var absorbed = ApplyAbsorption(simplifiedMembers, isUnion: false);
        var distinct = RemoveDuplicates(absorbed);
        var flattened = FlattenNestedIntersections(distinct);
        if (flattened.Count == 0 || flattened.Contains(PrimitiveType.Never))
            return PrimitiveType.Never;

        return flattened.Count == 1 ? flattened.First() : new IntersectionType(flattened);
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
            (UnionType ua, UnionType ub) => ua.Types.All(t => IsSubsetOf(t, ub)),
            (UnionType ua, _) => ua.Types.All(t => IsSubsetOf(t, b)),
            (_, UnionType ub) => ub.Types.Any(t => IsSubsetOf(a, t)),
            (IntersectionType ia, _) => ia.Types.All(t => IsSubsetOf(t, b)),
            (_, IntersectionType ib) => ib.Types.Any(t => IsSubsetOf(a, t)),
            _ => a.Equals(b) || a.IsAssignableTo(b)
        };

    private static List<Type> RemoveDuplicates(List<Type> types) =>
        types
            .Aggregate(new List<Type>(), (unique, item) =>
            {
                if (!unique.Any(item.Equals))
                    unique.Add(item);
                
                return unique;
            })
            .Where(t => t is not PrimitiveType { Kind: PrimitiveTypeKind.Never })
            .ToList();

    private static List<Type> FlattenNestedUnions(List<Type> types) =>
        types.SelectMany(t => t is UnionType union ? union.Types : [t]).ToList();

    private static List<Type> FlattenNestedIntersections(List<Type> types) =>
        types.SelectMany(t => t is IntersectionType intersection ? intersection.Types : [t]).ToList();
}