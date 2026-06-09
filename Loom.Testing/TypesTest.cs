using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.Testing;

using static PrimitiveType;

[Collection("Assembly")]
public class TypesTest
{
    [Fact]
    public void IsNever_ReturnsTrueForNever() => Assert.True(Type.IsNever(Never));

    [Fact]
    public void IsNever_ReturnsFalseForOtherTypes()
    {
        Assert.False(Type.IsNever(Number));
        Assert.False(Type.IsNever(String));
        Assert.False(Type.IsNever(Bool));
        Assert.False(Type.IsNever(Void));
        Assert.False(Type.IsNever(None));
        Assert.False(Type.IsNever(Unknown));
        Assert.False(Type.IsNever(new OptionalType(Number)));
        Assert.False(Type.IsNever(new UnionType([Number, String])));
        Assert.False(Type.IsNever(new IntersectionType([Number, String])));
        Assert.False(Type.IsNever(new LiteralType(42)));
    }

    [Fact]
    public void IsNotNever_ReturnsFalseForNever() => Assert.False(Type.IsNotNever(Never));

    [Fact]
    public void IsNotNever_ReturnsTrueForOtherTypes()
    {
        Assert.True(Type.IsNotNever(Number));
        Assert.True(Type.IsNotNever(String));
        Assert.True(Type.IsNotNever(new OptionalType(Number)));
        Assert.True(Type.IsNotNever(new UnionType([Number, String])));
    }

    [Fact]
    public void IsNone_ReturnsTrueForNoneAndVoid()
    {
        Assert.True(Type.IsNone(None));
        Assert.True(Type.IsNone(Void));
    }

    [Fact]
    public void IsNone_ReturnsFalseForOtherTypes()
    {
        Assert.False(Type.IsNone(Number));
        Assert.False(Type.IsNone(String));
        Assert.False(Type.IsNone(Bool));
        Assert.False(Type.IsNone(Never));
        Assert.False(Type.IsNone(Unknown));
        Assert.False(Type.IsNone(new OptionalType(Number)));
        Assert.False(Type.IsNone(new UnionType([Number, String])));
    }

    [Fact]
    public void IsDefined_ReturnsFalseForNoneAndVoid()
    {
        Assert.False(Type.IsDefined(None));
        Assert.False(Type.IsDefined(Void));
    }

    [Fact]
    public void IsDefined_ReturnsTrueForOtherTypes()
    {
        Assert.True(Type.IsDefined(Number));
        Assert.True(Type.IsDefined(String));
        Assert.True(Type.IsDefined(Bool));
        Assert.True(Type.IsDefined(Never));
        Assert.True(Type.IsDefined(Unknown));
        Assert.True(Type.IsDefined(new OptionalType(Number)));
        Assert.True(Type.IsDefined(new UnionType([Number, String])));
    }

    [Fact]
    public void IsOptional_ReturnsTrueForOptionalType() => Assert.True(Type.IsOptional(new OptionalType(Number)));

    [Fact]
    public void IsOptional_ReturnsTrueForNoneAndVoid()
    {
        Assert.True(Type.IsOptional(None));
        Assert.True(Type.IsOptional(Void));
    }

    [Fact]
    public void IsOptional_ReturnsFalseForRequiredTypes()
    {
        Assert.False(Type.IsOptional(Number));
        Assert.False(Type.IsOptional(String));
        Assert.False(Type.IsOptional(Bool));
        Assert.False(Type.IsOptional(Never));
        Assert.False(Type.IsOptional(new LiteralType(42)));
        Assert.False(Type.IsOptional(new IntersectionType([Number, new OptionalType(String)])));
    }

    [Fact]
    public void IsOptional_ReturnsTrueForUnionContainingNoneOrVoidOrOptional()
    {
        var unionWithNone = new UnionType([Number, None]);
        var unionWithVoid = new UnionType([String, Void]);
        var optional = new OptionalType(Bool);
        var unionWithOptional = new UnionType([Number, optional]);
        var nestedUnion = new UnionType([new UnionType([Number, None]), String]);

        Assert.True(Type.IsOptional(unionWithNone));
        Assert.True(Type.IsOptional(unionWithVoid));
        Assert.True(Type.IsOptional(unionWithOptional));
        Assert.True(Type.IsOptional(nestedUnion));
    }

    [Fact]
    public void IsOptional_ReturnsFalseForUnionWithoutOptionalOrNone()
    {
        var union = new UnionType([Number, String]);
        Assert.False(Type.IsOptional(union));
    }

    [Fact]
    public void IsNotOptional_ReturnsFalseForOptionalNoneAndVoid()
    {
        Assert.False(Type.IsNotOptional(new OptionalType(Number)));
        Assert.False(Type.IsNotOptional(None));
        Assert.False(Type.IsNotOptional(Void));
        Assert.False(Type.IsNotOptional(new UnionType([Number, None])));
    }

    [Fact]
    public void IsNotOptional_ReturnsTrueForRequiredTypes()
    {
        Assert.True(Type.IsNotOptional(Number));
        Assert.True(Type.IsNotOptional(String));
        Assert.True(Type.IsNotOptional(Bool));
        Assert.True(Type.IsNotOptional(new LiteralType(42)));
    }

    [Fact]
    public void NonNullable_OnNoneOrVoid_ReturnsNever()
    {
        Assert.Same(Never, None.NonNullable());
        Assert.Same(Never, Void.NonNullable());
    }

    [Fact]
    public void NonNullable_OnOptionalType_ReturnsNonNullableType()
    {
        var optional = new OptionalType(Number);
        Assert.Same(Number, optional.NonNullable());
    }

    [Fact]
    public void NonNullable_OnOptionalOptionalType_ReturnsInnerType()
    {
        var innerOptional = new OptionalType(String);
        var outerOptional = new OptionalType(innerOptional);
        Assert.Same(String, outerOptional.NonNullable());
    }

    [Fact]
    public void NonNullable_OnRequiredType_ReturnsItself()
    {
        Assert.Same(Number, Number.NonNullable());
        Assert.Same(Never, Never.NonNullable());
        Assert.Same(Unknown, Unknown.NonNullable());

        var literal = new LiteralType(42);
        Assert.Same(literal, literal.NonNullable());

        var intersection = new IntersectionType([Number, String]);
        Assert.Same(intersection, intersection.NonNullable());
    }

    [Fact]
    public void NonNullable_OnUnionWithoutNone_ReturnsItself()
    {
        var union = new UnionType([Number, String]);
        Assert.Same(union, union.NonNullable());
    }

    [Fact]
    public void NonNullable_OnUnionWithNoneOrVoid_RemovesThem()
    {
        var unionWithNone = new UnionType([Number, None, String]);
        var expected1 = new UnionType([Number, String]);
        Assert.True(expected1.Equals(unionWithNone.NonNullable()));

        var unionWithVoid = new UnionType([Number, Void]);
        Assert.True(Number.Equals(unionWithVoid.NonNullable()));
    }

    [Fact]
    public void NonNullable_OnUnionWithOptional_RemovesOptionalWrapper()
    {
        var optional = new OptionalType(Bool);
        var union = new UnionType([Number, optional]);
        var expected = new UnionType([Number, Bool]);
        Assert.True(expected.Equals(union.NonNullable()));
    }

    [Fact]
    public void NonNullable_OnUnionWithOnlyNone_ReturnsNever()
    {
        var union = new UnionType([None]);
        Assert.Same(Never, union.NonNullable());
    }

    [Fact]
    public void NonNullable_OnUnionWithOnlyOptional_ReturnsUnderlyingType()
    {
        var optional = new OptionalType(String);
        var union = new UnionType([optional]);
        Assert.Same(String, union.NonNullable());
    }

    [Fact]
    public void NonNullable_OnComplexUnion_Simplifies()
    {
        var union = new UnionType([Number, new OptionalType(String), None, new UnionType([Bool, Void])]);
        var expected = new UnionType([Number, String, Bool]);
        Assert.True(expected.Equals(union.NonNullable()));
    }

    [Fact]
    public void IsOptional_And_NonNullable_WorkTogether()
    {
        var union = new UnionType([Number, None]);
        Assert.True(Type.IsOptional(union));

        var nonNullable = union.NonNullable();
        Assert.False(Type.IsOptional(nonNullable));
        Assert.Same(Number, nonNullable);
    }

    [Fact]
    public void ArrayType_Assignability_CovariantImmutable()
    {
        var immutBases = new ArrayType(Unknown, isMutable: false);
        var immutSubtypes = new ArrayType(String, isMutable: false);
        var immutNumbers = new ArrayType(Number, isMutable: false);
        Assert.True(immutSubtypes.IsAssignableTo(immutBases));
        Assert.True(immutNumbers.IsAssignableTo(immutBases));
        Assert.False(immutSubtypes.IsAssignableTo(immutNumbers));
        Assert.True(immutSubtypes.IsAssignableTo(immutSubtypes));
    }
    
    [Fact]
    public void ArrayType_Assignability_ContravariantMutable()
    {
        var immutBase = new ArrayType(Number, isMutable: true);
        var immutSubtype = new ArrayType(Never, isMutable: true);
        Assert.True(immutSubtype.IsAssignableTo(immutBase));
        Assert.False(immutBase.IsAssignableTo(immutSubtype));
    }

    [Fact]
    public void ArrayType_Assignability_InvariantMutable()
    {
        var mutNumbers = new ArrayType(Number, isMutable: true);
        var mutStrings = new ArrayType(String, isMutable: true);
        var mutUnknown = new ArrayType(Unknown, isMutable: true);
        Assert.True(mutNumbers.IsAssignableTo(mutNumbers));
        Assert.True(mutStrings.IsAssignableTo(mutStrings));
        Assert.False(mutStrings.IsAssignableTo(mutUnknown));
        Assert.False(mutNumbers.IsAssignableTo(mutUnknown));
        Assert.False(mutStrings.IsAssignableTo(mutNumbers));

        var mutStrings2 = new ArrayType(String, isMutable: true);
        Assert.True(mutStrings.IsAssignableTo(mutStrings2));
    }

    [Fact]
    public void ArrayType_Assignability_CrossMutability()
    {
        var immutNumbers = new ArrayType(Number, isMutable: false);
        var mutNumbers = new ArrayType(Number, isMutable: true);
        Assert.False(immutNumbers.IsAssignableTo(mutNumbers));
        Assert.True(mutNumbers.IsAssignableTo(immutNumbers));

        var immutStrings = new ArrayType(String, isMutable: false);
        var mutStrings = new ArrayType(String, isMutable: true);
        Assert.False(immutStrings.IsAssignableTo(mutStrings));
        Assert.True(mutStrings.IsAssignableTo(immutStrings));
    }

    [Fact]
    public void ArrayType_Assignability_WithOptionalElements()
    {
        var optionalNumber = new OptionalType(Number);
        var requiredNumber = Number;
        var arrayOfOptional = new ArrayType(optionalNumber, isMutable: false);
        var arrayOfRequired = new ArrayType(requiredNumber, isMutable: false);
        Assert.True(arrayOfRequired.IsAssignableTo(arrayOfOptional));
        Assert.False(arrayOfOptional.IsAssignableTo(arrayOfRequired));

        var mutArrayOfOptional = new ArrayType(optionalNumber, isMutable: true);
        var mutArrayOfRequired = new ArrayType(requiredNumber, isMutable: true);
        Assert.False(mutArrayOfRequired.IsAssignableTo(mutArrayOfOptional));
        Assert.False(mutArrayOfOptional.IsAssignableTo(mutArrayOfRequired));
    }

    [Fact]
    public void ArrayType_Assignability_NestedArrays()
    {
        var arrayOfNumberArray = new ArrayType(new ArrayType(Number, false), false);
        var arrayOfStringArray = new ArrayType(new ArrayType(String, false), false);
        var arrayOfUnknownArray = new ArrayType(new ArrayType(Unknown, false), false);
        Assert.True(arrayOfStringArray.IsAssignableTo(arrayOfUnknownArray));
        Assert.False(arrayOfNumberArray.IsAssignableTo(arrayOfStringArray));

        var mutOuterImmutInner = new ArrayType(new ArrayType(Number, false), true);
        var immutOuterMutInner = new ArrayType(new ArrayType(Number, true), false);
        Assert.False(mutOuterImmutInner.IsAssignableTo(new ArrayType(new ArrayType(Unknown, false), true)));
        Assert.False(immutOuterMutInner.IsAssignableTo(new ArrayType(new ArrayType(Unknown, true), false)));
    }

    [Fact]
    public void FunctionType_Assignability_SameSignature()
    {
        var fn1 = new FunctionType([], [Number], String);
        var fn2 = new FunctionType([], [Number], String);
        Assert.True(fn1.IsAssignableTo(fn2));
        Assert.True(fn2.IsAssignableTo(fn1));
    }

    [Fact]
    public void FunctionType_Assignability_CovariantReturn()
    {
        var fn1 = new FunctionType([], [Number], String);
        var fn2 = new FunctionType([], [Number], Unknown);
        Assert.True(fn1.IsAssignableTo(fn2));
        Assert.False(fn2.IsAssignableTo(fn1));
    }

    [Fact]
    public void FunctionType_Assignability_ContravariantParameters()
    {
        var fn1 = new FunctionType([], [Unknown], String);
        var fn2 = new FunctionType([], [String], String);
        Assert.True(fn1.IsAssignableTo(fn2));
        Assert.True(fn1.IsAssignableTo(fn2));
        Assert.False(fn2.IsAssignableTo(fn1));
    }

    [Fact]
    public void FunctionType_Assignability_ContravariantMultipleParameters()
    {
        var fnWide = new FunctionType([], [Unknown, Unknown], String);
        var fnNarrow = new FunctionType([], [String, Number], String);
        Assert.True(fnWide.IsAssignableTo(fnNarrow));
        Assert.False(fnNarrow.IsAssignableTo(fnWide));
    }

    [Fact]
    public void FunctionType_Assignability_WithTypeParameters()
    {
        var paramT = new TypeParameter("T");
        var paramU = new TypeParameter("U");
        var fn1 = new FunctionType([paramT], [paramT], paramT);
        var fn2 = new FunctionType([paramU], [paramU], paramU);
        Assert.False(fn1.IsAssignableTo(fn2));
        Assert.False(fn2.IsAssignableTo(fn1));
    }

    [Fact]
    public void FunctionType_Assignability_WithInstantiatedTypeArguments()
    {
        var param = new TypeParameter("T");
        var genericFn = new FunctionType([param], [param], param);
        var instantiatedFn = new FunctionType([], [Number], Number);
        Assert.False(genericFn.IsAssignableTo(instantiatedFn));
        Assert.False(instantiatedFn.IsAssignableTo(genericFn));
    }

    [Fact]
    public void FunctionType_Assignability_HigherOrder()
    {
        var innerWide = new FunctionType([], [Number], Unknown);
        var innerNarrow = new FunctionType([], [Number], String);
        var outerWide = new FunctionType([], [innerWide], Bool);
        var outerNarrow = new FunctionType([], [innerNarrow], Bool);
        Assert.True(outerWide.IsAssignableTo(outerNarrow));
        Assert.False(outerNarrow.IsAssignableTo(outerWide));
    }

    [Fact]
    public void FunctionType_Assignability_WithUnionParameter()
    {
        var withUnionParameter = new FunctionType([], [new UnionType([Number, String])], Bool);
        var withNumberParameter = new FunctionType([], [Number], Bool);
        Assert.True(withUnionParameter.IsAssignableTo(withNumberParameter));
        Assert.False(withNumberParameter.IsAssignableTo(withUnionParameter));
    }

    [Fact]
    public void FunctionType_Assignability_WithNeverIntersectionReturn()
    {
        var returnIntersection = new FunctionType([], [Number], new IntersectionType([String, Bool]));
        var returnString = new FunctionType([], [Number], String);
        Assert.False(returnIntersection.IsAssignableTo(returnString));
        Assert.False(returnString.IsAssignableTo(returnIntersection));
    }

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
    public void ArrayType_Equality()
    {
        var arr1 = new ArrayType(Number, isMutable: true);
        var arr2 = new ArrayType(Number, isMutable: true);
        var arr3 = new ArrayType(Number, isMutable: false);
        var arr4 = new ArrayType(String, isMutable: true);
        Assert.True(arr1.Equals(arr1));
        Assert.True(arr1.Equals(arr2));
        Assert.False(arr1.Equals(arr3));
        Assert.False(arr1.Equals(arr4));
        Assert.False(arr1.Equals(Number));

        var immut1 = new ArrayType(Bool, isMutable: false);
        var immut2 = new ArrayType(Bool, isMutable: false);
        var mutable = new ArrayType(Bool, isMutable: true);
        Assert.True(immut1.Equals(immut2));
        Assert.False(immut1.Equals(mutable));
    }

    [Fact]
    public void FunctionType_Equality()
    {
        var param1 = new TypeParameter("T");
        var param2 = new TypeParameter("U");
        var fn1 = new FunctionType([param1], [Number], Bool);
        var fn2 = new FunctionType([param1], [Number], Bool);
        var fn3 = new FunctionType([param1], [String], Bool);
        var fn4 = new FunctionType([param2], [Number], Bool);
        var fn5 = new FunctionType([], [Number], Bool);

        Assert.True(fn1.Equals(fn2));
        Assert.False(fn1.Equals(fn3));
        Assert.True(fn1.Equals(fn4)); // type parameter name doesn't matter for equality (structural)
        Assert.False(fn1.Equals(fn5));
        Assert.False(fn1.Equals(Number));
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