using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Testing;

public class TypeSimplifierTest
{
    public static readonly List<object[]> NoChangeTypes = new List<Type>(
        [PrimitiveType.Bool, PrimitiveType.None, PrimitiveType.Never, new UnionType([PrimitiveType.Bool, PrimitiveType.String])]
    ).ConvertAll<object[]>(t => [t]);

    [Theory]
    [MemberData(nameof(NoChangeTypes))]
    public void NoChange_Simplification(Type type)
    {
        var simplified = TypeSimplifier.Simplify(type);
        var equal = type.Equals(simplified);
        Assert.True(equal, $"Expected type {type} to not change during simplification. Got: {simplified}");
    }

    [Fact]
    public void Intersection_Idempotence()
    {
        var intersection1 = new IntersectionType([PrimitiveType.Bool, PrimitiveType.Bool]);
        var intersection2 = new IntersectionType([PrimitiveType.String, PrimitiveType.String, PrimitiveType.String]);
        var simplified1 = TypeSimplifier.Simplify(intersection1);
        var simplified2 = TypeSimplifier.Simplify(intersection2);
        Assert.True(simplified1.Equals(PrimitiveType.Bool));
        Assert.True(simplified2.Equals(PrimitiveType.String));
        Assert.False(simplified1.Equals(intersection1));
        Assert.False(simplified2.Equals(intersection2));
    }
    
    [Fact]
    public void Intersection_Absorption()
    {
        var boolOrString = new UnionType([PrimitiveType.Bool, PrimitiveType.String]);
        var type = new IntersectionType([PrimitiveType.Bool, boolOrString]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Bool), $"Expected 'bool', got '{simplified}'");

        var withNever = new IntersectionType([PrimitiveType.String, PrimitiveType.Never]);
        var simplifiedNever = TypeSimplifier.Simplify(withNever);
        Assert.True(simplifiedNever.Equals(PrimitiveType.Never), $"Expected 'never', got '{simplifiedNever}'");
    }
    
    [Fact]
    public void Union_Idempotence()
    {
        var union1 = new UnionType([PrimitiveType.Bool, PrimitiveType.Bool]);
        var union2 = new UnionType([PrimitiveType.String, PrimitiveType.String, PrimitiveType.String]);
        var simplified1 = TypeSimplifier.Simplify(union1);
        var simplified2 = TypeSimplifier.Simplify(union2);
        Assert.True(simplified1.Equals(PrimitiveType.Bool));
        Assert.True(simplified2.Equals(PrimitiveType.String));
        Assert.False(simplified1.Equals(union1));
        Assert.False(simplified2.Equals(union2));
    }

    [Fact]
    public void Union_Absorption()
    {
        var nestedUnion = new UnionType([PrimitiveType.Number, PrimitiveType.String]);
        var type = new UnionType([PrimitiveType.Number, nestedUnion]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new UnionType([PrimitiveType.Number, PrimitiveType.String]);
        Assert.True(simplified.Equals(expected));

        var withNever = new UnionType([PrimitiveType.Number, PrimitiveType.Never]);
        var simplifiedNever = TypeSimplifier.Simplify(withNever);
        Assert.True(simplifiedNever.Equals(PrimitiveType.Number));
    }

    [Fact]
    public void Deduplication()
    {
        var duplicateNumberUnion = new UnionType([PrimitiveType.Number, PrimitiveType.Number]);
        var simplifiedUnion = TypeSimplifier.Simplify(duplicateNumberUnion);
        Assert.True(simplifiedUnion.Equals(PrimitiveType.Number), $"Expected 'number', got '{simplifiedUnion}'");

        var duplicateStringIntersection = new IntersectionType([PrimitiveType.String, PrimitiveType.String]);
        var simplifiedIntersection = TypeSimplifier.Simplify(duplicateStringIntersection);
        Assert.True(simplifiedIntersection.Equals(PrimitiveType.String), $"Expected 'string', got '{simplifiedIntersection}'");

        var boolOrString = new UnionType([PrimitiveType.Bool, PrimitiveType.String]);
        var stringOrBool = new UnionType([PrimitiveType.String, PrimitiveType.Bool]);
        var duplicateUnion = new UnionType([boolOrString, stringOrBool]);
        var simplifiedDuplicateUnion = TypeSimplifier.Simplify(duplicateUnion);
        Assert.True(simplifiedDuplicateUnion.Equals(boolOrString), $"Expected '{boolOrString}', got '{simplifiedDuplicateUnion}'");

        var boolAndString = new IntersectionType([PrimitiveType.Bool, PrimitiveType.String]);
        var stringAndBool = new IntersectionType([PrimitiveType.String, PrimitiveType.Bool]);
        var duplicateIntersection = new IntersectionType([boolAndString, stringAndBool]);
        var simplifiedDuplicateIntersection = TypeSimplifier.Simplify(duplicateIntersection);
        Assert.True(simplifiedDuplicateIntersection.Equals(boolAndString), $"Expected {boolAndString}, got {simplifiedDuplicateIntersection}");
    }

    [Fact]
    public void Combined_AbsorptionAndDeduplication()
    {
        var numberAndString = new IntersectionType([PrimitiveType.Number, PrimitiveType.String]);
        var boolOrNever = new UnionType([PrimitiveType.Bool, numberAndString]);
        var boolOrString = new UnionType([PrimitiveType.Bool, PrimitiveType.String]);
        var type = new IntersectionType([boolOrNever, boolOrString, boolOrString]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new UnionType([PrimitiveType.Bool, PrimitiveType.String]);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }
}