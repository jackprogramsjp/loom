using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Testing;

[Collection("Assembly")]
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
    public void Union_LiteralAbsorption_WithPrimitive()
    {
        var type = new UnionType([new LiteralType(0), PrimitiveType.Number]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Number), $"Expected 'number', got '{simplified}'");
    }

    [Fact]
    public void Union_LiteralAbsorption_WithString()
    {
        var type = new UnionType([new LiteralType("hello"), PrimitiveType.String]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.String), $"Expected 'string', got '{simplified}'");
    }

    [Fact]
    public void Union_LiteralAbsorption_WithBool()
    {
        var type = new UnionType([new LiteralType(true), PrimitiveType.Bool]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Bool), $"Expected 'bool', got '{simplified}'");
    }

    [Fact]
    public void Union_MultipleLiteralsWithPrimitive()
    {
        var type = new UnionType([new LiteralType(1), new LiteralType(2), PrimitiveType.Number]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Number), $"Expected 'number', got '{simplified}'");
    }

    [Fact]
    public void Union_MultipleLiteralsOnly()
    {
        var type = new UnionType([new LiteralType(1), new LiteralType(2)]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new UnionType([new LiteralType(1), new LiteralType(2)]);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Union_MixedLiteralTypes()
    {
        var type = new UnionType([new LiteralType(1), new LiteralType("hello")]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new UnionType([new LiteralType(1), new LiteralType("hello")]);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Union_WithNone_BecomesOptional()
    {
        var type = new UnionType([PrimitiveType.Number, PrimitiveType.None]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new OptionalType(PrimitiveType.Number);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Union_WithNoneAndLiteral()
    {
        var type = new UnionType([new LiteralType(1), PrimitiveType.None]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new OptionalType(new LiteralType(1));
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Intersection_PrimitiveAndLiteral()
    {
        var type = new IntersectionType([PrimitiveType.Number, new LiteralType(1)]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new LiteralType(1);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Intersection_StringAndLiteral()
    {
        var type = new IntersectionType([PrimitiveType.String, new LiteralType("hello")]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new LiteralType("hello");
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Intersection_BoolAndLiteral()
    {
        var type = new IntersectionType([PrimitiveType.Bool, new LiteralType(true)]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new LiteralType(true);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Intersection_NumberAndString()
    {
        var type = new IntersectionType([PrimitiveType.Number, PrimitiveType.String]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Never), $"Expected 'never', got '{simplified}'");
    }

    [Fact]
    public void Intersection_TwoDifferentLiterals()
    {
        var type = new IntersectionType([new LiteralType(1), new LiteralType(2)]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Never), $"Expected 'never', got '{simplified}'");
    }

    [Fact]
    public void Intersection_LiteralAndUnion_WithOverlap()
    {
        var type = new IntersectionType([new LiteralType(1), new UnionType([new LiteralType(1), new LiteralType(2)])]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new LiteralType(1);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Intersection_TwoUnions_WithOverlap()
    {
        var u1 = new UnionType([new LiteralType(1), new LiteralType(2)]);
        var u2 = new UnionType([new LiteralType(2), new LiteralType(3)]);
        var type = new IntersectionType([u1, u2]);
        var simplified = TypeSimplifier.Simplify(type);
        var expected = new LiteralType(2);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
    }

    [Fact]
    public void Intersection_UnionAndPrimitive_WithLiteralAbsorption()
    {
        var union = new UnionType([new LiteralType(1), PrimitiveType.Number]);
        var type = new IntersectionType([union, PrimitiveType.Number]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Number), $"Expected 'number', got '{simplified}'");
    }

    [Fact]
    public void Union_WithNestedUnion_LiteralAbsorption()
    {
        var inner = new UnionType([new LiteralType(1), PrimitiveType.Number]);
        var outer = new UnionType([inner, PrimitiveType.String]);
        var simplified = TypeSimplifier.Simplify(outer);
        var expected = new UnionType([PrimitiveType.Number, PrimitiveType.String]);
        Assert.True(simplified.Equals(expected), $"Expected '{expected}', got '{simplified}'");
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

        var numberOrBool = new UnionType([PrimitiveType.Number, PrimitiveType.Bool]);
        var combinedIntersection = new IntersectionType([boolOrString, numberOrBool]);
        var simplifiedCombinedIntersection = TypeSimplifier.Simplify(combinedIntersection);
        Assert.True(simplifiedCombinedIntersection.Equals(PrimitiveType.Bool), $"Expected 'bool', got '{simplifiedCombinedIntersection}'");

        var boolAndString = new IntersectionType([PrimitiveType.Bool, PrimitiveType.String]);
        var stringAndBool = new IntersectionType([PrimitiveType.String, PrimitiveType.Bool]);
        var duplicateIntersection = new IntersectionType([boolAndString, stringAndBool]);
        var simplifiedDuplicateIntersection = TypeSimplifier.Simplify(duplicateIntersection);
        Assert.True(Type.IsNever(simplifiedDuplicateIntersection), $"Expected 'never', got '{simplifiedDuplicateIntersection}'");
    }

    [Fact]
    public void Combined_AbsorptionAndDeduplication()
    {
        var numberAndString = new IntersectionType([PrimitiveType.Number, PrimitiveType.String]);
        var boolOrNever = new UnionType([PrimitiveType.Bool, numberAndString]);
        var boolOrString = new UnionType([PrimitiveType.Bool, PrimitiveType.String]);
        var type = new IntersectionType([boolOrNever, boolOrString, boolOrString]);
        var simplified = TypeSimplifier.Simplify(type);
        Assert.True(simplified.Equals(PrimitiveType.Bool), $"Expected 'bool', got '{simplified}'");
    }
}