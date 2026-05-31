using Loom.TypeChecking.Types;

namespace Loom.Testing;
using static PrimitiveType;

[Collection("Assembly")]
public class TypesTest
{
    [Fact]
    public void Optional_Assignability()
    {
        var required = String;
        var optional = new OptionalType(required);
        Assert.False(optional.IsAssignableTo(required));
        Assert.True(required.IsAssignableTo(optional));
    }
        
    [Fact]
    public void Intersection_Assignability()
    {
        var union1 = new UnionType([Number, Bool]);
        var union2 = new UnionType([Bool, String]);
        var intersection = new IntersectionType([union1, union2]);
        Assert.False(None.IsAssignableTo(union1));
        Assert.False(None.IsAssignableTo(union2));
        Assert.False(None.IsAssignableTo(intersection));
        Assert.True(union1.IsAssignableTo(union2));
        Assert.True(union2.IsAssignableTo(union1));
        Assert.True(intersection.IsAssignableTo(union1));
        Assert.True(intersection.IsAssignableTo(union2));
        Assert.True(intersection.IsAssignableTo(intersection));
        Assert.True(Bool.IsAssignableTo(intersection));
        Assert.True(Bool.IsAssignableTo(union1));
        Assert.True(Bool.IsAssignableTo(union2));
        Assert.True(intersection.IsAssignableTo(Bool));
    }
    
    [Fact]
    public void Intersection_Literal_Assignability()
    {
        var union1 = new UnionType([Number, Bool]);
        var union2 = new UnionType([Bool, String]);
        var intersection = new IntersectionType([union1, union2]);
        var literal = new LiteralType(false);
        Assert.True(literal.IsAssignableTo(union1));
        Assert.True(literal.IsAssignableTo(union2));
        Assert.True(union1.IsAssignableTo(union2));
        Assert.True(union2.IsAssignableTo(union1));
        Assert.True(literal.IsAssignableTo(intersection));
        Assert.False(intersection.IsAssignableTo(literal));
        Assert.True(literal.IsAssignableTo(Bool));
        Assert.False(literal.IsAssignableTo(String));
        Assert.False(literal.IsAssignableTo(Number));
    }
    
    [Fact]
    public void Union_Assignability()
    {
        var union1 = new UnionType([Bool, Number]);
        var union2 = new UnionType([Bool, Number, String]);
        Assert.False(union1.IsAssignableTo(Bool));
        Assert.False(None.IsAssignableTo(union1));
        Assert.False(None.IsAssignableTo(union2));
        Assert.True(Bool.IsAssignableTo(union1));
        Assert.True(Bool.IsAssignableTo(union2));
        Assert.True(union2.IsAssignableTo(union1));
        Assert.True(union1.IsAssignableTo(union2));
    }
    
    [Fact]
    public void Union_Literal_Assignability()
    {
        var union = new UnionType([Bool, Number]);
        var literal = new LiteralType(69.420);
        Assert.False(union.IsAssignableTo(literal));
        Assert.True(literal.IsAssignableTo(union));
        Assert.True(literal.IsAssignableTo(Number));
        Assert.False(literal.IsAssignableTo(Bool));
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
        Assert.True(integer.IsAssignableTo(widenedInteger));
        Assert.True(integer.IsAssignableTo(widenedNumber));
        Assert.True(number.IsAssignableTo(widenedInteger));
        Assert.True(number.IsAssignableTo(widenedNumber));
        Assert.False(widenedInteger.IsAssignableTo(integer));
        Assert.False(widenedInteger.IsAssignableTo(number));
        Assert.False(widenedNumber.IsAssignableTo(integer));
        Assert.False(widenedNumber.IsAssignableTo(number));
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
        Assert.False(Number.IsAssignableTo(Never));
        Assert.False(Unknown.IsAssignableTo(Number));
        Assert.True(Number.IsAssignableTo(Unknown));
    }
    
    [Fact]
    public void Optional_Equality()
    {
        var a = new OptionalType(String);
        var b = new OptionalType(new PrimitiveType(PrimitiveTypeKind.String));
        var c = new OptionalType(Number);
        var d = Number;
        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(b.Equals(c));
        Assert.False(d.Equals(a));
        Assert.False(d.Equals(b));
        Assert.False(d.Equals(c));
        Assert.False(a.Equals(d));
        Assert.False(b.Equals(d));
        Assert.False(c.Equals(d));
    }
    
    [Fact]
    public void Intersection_Equality()
    {
        var a = new IntersectionType([new PrimitiveType(PrimitiveTypeKind.Bool), String]);
        var b = new IntersectionType([Bool, new PrimitiveType(PrimitiveTypeKind.String)]);
        var c = new IntersectionType([Number, Bool]);
        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(b.Equals(c));
    }
    
    [Fact]
    public void Union_Equality()
    {
        var a = new UnionType([new PrimitiveType(PrimitiveTypeKind.Bool), String]);
        var b = new UnionType([Bool, new PrimitiveType(PrimitiveTypeKind.String)]);
        var c = new UnionType([Number, Bool]);
        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(b.Equals(c));
    }
    
    [Fact]
    public void Literal_Equality()
    {
        var a = new LiteralType(69);
        var b = new LiteralType(69);
        var c = new LiteralType(420);
        Assert.True(a.Equals(a));
        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
        Assert.False(b.Equals(c));
    }
    
    [Fact]
    public void Primitive_Equality()
    {
        Assert.True(Bool.Equals(Bool));
        Assert.True(Bool.Equals(new PrimitiveType(PrimitiveTypeKind.Bool)));
        Assert.False(Bool.Equals(String));
    }
}