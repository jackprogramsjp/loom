using Loom.TypeChecking.Types;

namespace Loom.Testing;
using static PrimitiveType;

public class TypesTest
{
    [Fact]
    public void Intersection_Assignability()
    {
        var union1 = new UnionType([Number, Bool]);
        var union2 = new UnionType([Bool, String]);
        var intersection = new IntersectionType([union1, union2]);
        Assert.True(intersection.IsAssignableTo(union1));
        Assert.True(intersection.IsAssignableTo(union2));
        Assert.True(intersection.IsAssignableTo(intersection));
        Assert.True(Bool.IsAssignableTo(intersection));
        Assert.True(intersection.IsAssignableTo(Bool));
    }
    
    [Fact]
    public void Union_Assignability()
    {
        var union1 = new UnionType([Bool, Number]);
        var union2 = new UnionType([Bool, Number, String]);
        Assert.False(union1.IsAssignableTo(Bool));
        Assert.True(Bool.IsAssignableTo(union1));
        Assert.True(union2.IsAssignableTo(union1));
        Assert.True(union1.IsAssignableTo(union2));
    }
    
    [Fact]
    public void Literal_Assignability()
    {
        var integer = new LiteralType(69);
        var number = new LiteralType(420.69);
        Assert.True(integer.IsAssignableTo(integer));
        Assert.True(number.IsAssignableTo(number));
        Assert.False(integer.IsAssignableTo(number));
        Assert.False(number.IsAssignableTo(integer));

        var widenedInteger = Assert.IsType<PrimitiveType>(integer.Widen());
        var widenedNumber = Assert.IsType<PrimitiveType>(number.Widen());
        Assert.Equal(PrimitiveTypeKind.Number, widenedInteger.Kind);
        Assert.Equal(PrimitiveTypeKind.Number, widenedNumber.Kind);
        Assert.True(widenedInteger.IsAssignableTo(widenedNumber));
    }

    [Fact]
    public void Primitive_Assignability()
    {
        Assert.True(Bool.IsAssignableTo(Bool));
        Assert.False(Bool.IsAssignableTo(String));
        Assert.True(Number.IsAssignableTo(Number));
        Assert.True(String.IsAssignableTo(String));
        Assert.True(None.IsAssignableTo(None));
        Assert.True(Void.IsAssignableTo(Void));
        Assert.True(Void.IsAssignableTo(None));
        Assert.True(None.IsAssignableTo(Void));
        Assert.True(Never.IsAssignableTo(Number));
        Assert.False(Unknown.IsAssignableTo(Number));
        Assert.True(Number.IsAssignableTo(Unknown));
        Assert.False(Number.IsAssignableTo(Never));
    }
}