using Loom.Parsing;
using Loom.Parsing.AST;
using Loom.Text;
using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using ArrayType = Loom.TypeChecking.Types.ArrayType;
using FunctionType = Loom.TypeChecking.Types.FunctionType;
using IntersectionType = Loom.TypeChecking.Types.IntersectionType;
using LiteralType = Loom.TypeChecking.Types.LiteralType;
using OptionalType = Loom.TypeChecking.Types.OptionalType;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;
using TypeParameter = Loom.TypeChecking.Types.TypeParameter;
using UnionType = Loom.TypeChecking.Types.UnionType;

namespace Loom.Testing;
using TypeParameterSubstitution = Dictionary<TypeParameter, Type>;

[Collection("Assembly")]
public class TypeInferrerTest
{
    private class MockGenericNamedDeclaration(string name)
        : GenericNamedDeclaration(
            [],
            new Token(SyntaxKind.TypeKeyword, LocationSpan.Empty(), "type"),
            new Token(SyntaxKind.Identifier, LocationSpan.Empty(), name),
            new TypeParameters(
                new Token(SyntaxKind.LArrow, LocationSpan.Empty(), "<"),
                new Token(SyntaxKind.RArrow, LocationSpan.Empty(), ">"),
                []
            )
        )
    {
        public override T Accept<T>(Visitor<T> visitor) => default!;
    }

    private sealed class ExpressionStub() : Expression([], [])
    {
        public override T Accept<T>(Visitor<T> visitor) => default!;
    }

    private static TypeParameter TypeParameter(string name, Type? constraint = null, Type? defaultType = null) =>
        new(name, constraint, defaultType);

    private static GenericType GenericType(string name, List<TypeParameter> parameters, Type underlying) =>
        new(new MockGenericNamedDeclaration(name), parameters, underlying);

    private static InterfaceType InterfaceType(ObjectType objectType) => new("", [], objectType);

    private static ObjectType ObjectType(List<ObjectProperty> properties, ObjectIndexer? indexer = null) =>
        new(indexer, properties);

    private static ObjectProperty ObjectProperty(string name, Type type) => new(false, name, type);

    private static ObjectIndexer ObjectIndexer(Type keyType, Type valueType) => new(false, keyType, valueType);

    private static FunctionType FunctionType(List<TypeParameter> typeParameters, List<Type> parameterTypes, Type returnType) =>
        new(typeParameters, parameterTypes, returnType);

    private static TypeParameterSubstitution RunInterfaceInference(
        string interfaceName,
        List<TypeParameter> typeParameters,
        ObjectType underlying,
        List<(string name, Type type)> properties,
        List<(Type keyType, Type valueType)> indices)
    {
        var generic = GenericType(interfaceName, typeParameters, underlying);
        var interfaceType = InterfaceType(underlying);
        var typeMap = new Dictionary<Node, Type>();
        var initializers = new List<InterfaceInvocationInitializer>();
        foreach (var (name, propType) in properties)
        {
            var expr = new ExpressionStub();
            typeMap[expr] = propType;
            initializers.Add(new InterfaceInvocationPropertyInitializer(
                TokenFactory.Identifier(name),
                TokenFactory.Operator(SyntaxKind.Colon),
                expr));
        }

        foreach (var (keyType, valueType) in indices)
        {
            var keyExpr = new ExpressionStub();
            var valueExpr = new ExpressionStub();
            typeMap[keyExpr] = keyType;
            typeMap[valueExpr] = valueType;
            initializers.Add(new InterfaceInvocationIndexInitializer(
                TokenFactory.Operator(SyntaxKind.LBracket),
                TokenFactory.Operator(SyntaxKind.RBracket),
                TokenFactory.Operator(SyntaxKind.Colon),
                keyExpr,
                valueExpr));
        }

        var body = new InterfaceInvocationBody(
            TokenFactory.Operator(SyntaxKind.LBrace),
            TokenFactory.Operator(SyntaxKind.RBrace),
            initializers);

        var node = new InterfaceInvocation(
            TokenFactory.Keyword(SyntaxKind.InterfaceKeyword),
            new Identifier(TokenFactory.Identifier(interfaceName)),
            null,
            body);

        var inferrer = new TypeInferrer(n => typeMap[n]);
        return inferrer.InferInterfaceTypeArguments(node, generic, interfaceType);
    }
    
    
    [Fact]
    public void InferInterfaceTypeArguments_EmptyInitializers_UsesDefaultTypes()
    {
        var typeParameter = TypeParameter("T", defaultType: PrimitiveType.String);
        var objectType = ObjectType([ObjectProperty("Value", typeParameter)]);
        var result = RunInterfaceInference("Container", [typeParameter], objectType, properties: [], indices: []);
        Assert.Equal(PrimitiveType.String, result[typeParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_IndexOnObjectWithoutIndexer_Ignored()
    {
        var typeParameter = TypeParameter("T", defaultType: PrimitiveType.String);
        var objectType = ObjectType([ObjectProperty("Value", typeParameter)]); // no indexer
        var result = RunInterfaceInference("Container", [typeParameter], objectType,
            properties: [],
            indices: [(PrimitiveType.Number, PrimitiveType.Bool)]);
        Assert.Equal(PrimitiveType.String, result[typeParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_ConflictingPropertyTypesWithoutCommonWidened_FirstBindingWins()
    {
        var typeParameter = TypeParameter("T");
        var objectType = ObjectType([
            ObjectProperty("First", typeParameter),
            ObjectProperty("Second", typeParameter)
        ]);
        var result = RunInterfaceInference("Pair", [typeParameter], objectType,
            properties: [("First", PrimitiveType.Number), ("Second", PrimitiveType.String)],
            indices: []);
        Assert.Equal(PrimitiveType.Number, result[typeParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_MissingPropertyInArgument_FailsThatPairButOtherStillBinds()
    {
        var firstParameter = TypeParameter("T");
        var secondParameter = TypeParameter("U");
        var objectType = ObjectType([
            ObjectProperty("Name", firstParameter),
            ObjectProperty("Age", secondParameter)
        ]);
        var result = RunInterfaceInference("Rec", [firstParameter, secondParameter], objectType,
            properties: [("Age", PrimitiveType.Number)], // "Name" missing
            indices: []);
        Assert.Equal(PrimitiveType.Unknown, result[firstParameter]);
        Assert.Equal(PrimitiveType.Number, result[secondParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_NoTypeParameters_ReturnsEmptySubstitution()
    {
        var function = FunctionType([], [PrimitiveType.Number], PrimitiveType.Bool);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.Number]);
        Assert.Empty(result);
    }

    [Fact]
    public void InferFunctionTypeArguments_FewerArgumentsThanParameters_UsesDefaultsForMissing()
    {
        var firstParameter = TypeParameter("T", defaultType: PrimitiveType.String);
        var secondParameter = TypeParameter("U", defaultType: PrimitiveType.Number);
        var function = FunctionType([firstParameter, secondParameter], [firstParameter, secondParameter], firstParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.Bool]);
        Assert.Equal(PrimitiveType.Bool, result[firstParameter]);
        Assert.Equal(PrimitiveType.Number, result[secondParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_MoreArgumentsThanParameters_ExtraIgnored()
    {
        var typeParameter = TypeParameter("T");
        var function = FunctionType([typeParameter], [typeParameter], typeParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function,
            [PrimitiveType.Number, PrimitiveType.String, PrimitiveType.Bool]);
        Assert.Equal(PrimitiveType.Number, result[typeParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_ContextualTypeWithConcreteReturnType_DoesNotThrow()
    {
        var function = FunctionType([], [PrimitiveType.Number], PrimitiveType.Bool);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.Number], contextualType: PrimitiveType.String);
        Assert.Empty(result);
    }

    [Fact]
    public void InferFunctionTypeArguments_MultipleOccurrencesConflicting_WidensToCommonType()
    {
        var typeParameter = TypeParameter("T");
        var function = FunctionType([typeParameter], [typeParameter], typeParameter);
        var literalInt = new LiteralType(10);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [literalInt], contextualType: PrimitiveType.Number);
        Assert.True(result[typeParameter].Equals(PrimitiveType.Number) ||
                    result[typeParameter].Equals(literalInt.Widen()));
    }

    [Fact]
    public void InferFunctionTypeArguments_FunctionParameterCountMismatch_NoInferenceFromThatPair()
    {
        var typeParameter = TypeParameter("T");
        var innerFunction = FunctionType([typeParameter], [typeParameter], typeParameter);
        var outerFunction = FunctionType([typeParameter], [innerFunction], typeParameter);
        var argumentFunction = FunctionType([], [PrimitiveType.Number, PrimitiveType.String], PrimitiveType.Bool);
        var result = TypeInferrer.InferFunctionTypeArguments(outerFunction, [argumentFunction]);
        Assert.Equal(PrimitiveType.Unknown, result[typeParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_NestedGenericType_InferMultipleParameters()
    {
        var keyParameter = TypeParameter("K");
        var valueParameter = TypeParameter("V");
        var mapGeneric = GenericType("Map", [keyParameter, valueParameter], PrimitiveType.Unknown);
        var mapParameterType = new InstantiatedType(mapGeneric, [keyParameter, valueParameter]);
        var function = FunctionType([keyParameter, valueParameter], [mapParameterType], mapParameterType);
        var argumentMap = new InstantiatedType(mapGeneric, [PrimitiveType.String, PrimitiveType.Number]);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [argumentMap]);
        Assert.Equal(PrimitiveType.Unknown, result[keyParameter]);
        Assert.Equal(PrimitiveType.Unknown, result[valueParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_SingleProperty_BindsTypeParameter()
    {
        var typeParameter = TypeParameter("T");
        var objectType = ObjectType([ObjectProperty("Value", typeParameter)]);
        var result = RunInterfaceInference("Container", [typeParameter], objectType,
            properties: [("Value", PrimitiveType.Number)],
            indices: []);
        Assert.Equal(PrimitiveType.Number, result[typeParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_PropertyNotFound_UsesDefaultType()
    {
        var typeParameter = TypeParameter("T", defaultType: PrimitiveType.String);
        var objectType = ObjectType([ObjectProperty("Existing", PrimitiveType.Number)]);
        var result = RunInterfaceInference("Container", [typeParameter], objectType,
            properties: [("Missing", PrimitiveType.Bool)],
            indices: []);
        Assert.Equal(PrimitiveType.String, result[typeParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_IndexInitializer_BindsKeyAndValueTypeParameters()
    {
        var keyParameter = TypeParameter("K");
        var valueParameter = TypeParameter("V");
        var objectType = ObjectType([], ObjectIndexer(keyParameter, valueParameter));
        var result = RunInterfaceInference("Dictionary", [keyParameter, valueParameter], objectType,
            properties: [],
            indices: [(PrimitiveType.String, PrimitiveType.Number)]);
        Assert.Equal(PrimitiveType.String, result[keyParameter]);
        Assert.Equal(PrimitiveType.Number, result[valueParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_MixedPropertyAndIndex_BindsSharedTypeParameter()
    {
        var sharedParameter = TypeParameter("T");
        var objectType = ObjectType(
            [ObjectProperty("Name", sharedParameter)],
            ObjectIndexer(PrimitiveType.String, sharedParameter));
        var result = RunInterfaceInference("Record", [sharedParameter], objectType,
            properties: [("Name", PrimitiveType.Number)],
            indices: [(PrimitiveType.String, PrimitiveType.Number)]);
        Assert.Equal(PrimitiveType.Number, result[sharedParameter]);
    }

    [Fact]
    public void InferInterfaceTypeArguments_WideningWhenConflictingTypes_InferCommonSupertype()
    {
        var typeParameter = TypeParameter("T");
        var objectType = ObjectType([
            ObjectProperty("First", typeParameter),
            ObjectProperty("Second", typeParameter)
        ]);
        var result = RunInterfaceInference("Pair", [typeParameter], objectType,
            properties: [("First", PrimitiveType.Number), ("Second", new LiteralType(42))],
            indices: []);
        Assert.True(result[typeParameter].Equals(PrimitiveType.Number) ||
                    result[typeParameter].Equals(new LiteralType(42).Widen()));
    }

    [Fact]
    public void InferFunctionTypeArguments_SingleParameter_BindsTypeParameter()
    {
        var typeParameter = TypeParameter("T");
        var function = FunctionType([typeParameter], [typeParameter], typeParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.Number]);
        Assert.Equal(PrimitiveType.Number, result[typeParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_MultipleParameters_BindAll()
    {
        var firstParameter = TypeParameter("T");
        var secondParameter = TypeParameter("U");
        var function = FunctionType([firstParameter, secondParameter], [firstParameter, secondParameter], firstParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.Number, PrimitiveType.String]);
        Assert.Equal(PrimitiveType.Number, result[firstParameter]);
        Assert.Equal(PrimitiveType.String, result[secondParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_WithContextualType_WidensParameter()
    {
        var typeParameter = TypeParameter("T");
        var function = FunctionType([typeParameter], [typeParameter], typeParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.Number], contextualType: PrimitiveType.String);
        var inferred = result[typeParameter];
        Assert.True(inferred.Equals(PrimitiveType.Number.Widen()) || inferred.Equals(PrimitiveType.String.Widen()));
    }

    [Fact]
    public void InferFunctionTypeArguments_NoArguments_UsesDefaultType()
    {
        var typeParameter = TypeParameter("T", defaultType: PrimitiveType.Bool);
        var function = FunctionType([typeParameter], [], typeParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, []);
        Assert.Equal(PrimitiveType.Bool, result[typeParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_ArrayParameter_InferElementType()
    {
        var elementParameter = TypeParameter("T");
        var arrayType = new ArrayType(elementParameter, isMutable: false);
        var function = FunctionType([elementParameter], [arrayType], elementParameter);
        var argumentArray = new ArrayType(PrimitiveType.Number, isMutable: false);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [argumentArray]);
        Assert.Equal(PrimitiveType.Number, result[elementParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_OptionalParameter_WithOptionalArgument()
    {
        var elementParameter = TypeParameter("T");
        var optionalParameter = new OptionalType(elementParameter);
        var function = FunctionType([elementParameter], [optionalParameter], elementParameter);
        var optionalArgument = new OptionalType(PrimitiveType.Number);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [optionalArgument]);
        Assert.Equal(PrimitiveType.Number, result[elementParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_OptionalParameter_WithNonNullableArgument()
    {
        var elementParameter = TypeParameter("T");
        var optionalParameter = new OptionalType(elementParameter);
        var function = FunctionType([elementParameter], [optionalParameter], elementParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.Number]);
        Assert.Equal(PrimitiveType.Number, result[elementParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_ObjectType_InferPropertyType()
    {
        var propertyTypeParameter = TypeParameter("T");
        var parameterObject = ObjectType([ObjectProperty("Data", propertyTypeParameter)]);
        var argumentObject = ObjectType([ObjectProperty("Data", PrimitiveType.Number)]);
        var function = FunctionType([propertyTypeParameter], [parameterObject], propertyTypeParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [argumentObject]);
        Assert.Equal(PrimitiveType.Number, result[propertyTypeParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_FunctionType_InferInnerTypes()
    {
        var inputParameter = TypeParameter("T");
        var outputParameter = TypeParameter("U");
        var innerFunction = FunctionType([], [inputParameter], outputParameter);
        var outerFunction = FunctionType([inputParameter, outputParameter], [innerFunction], innerFunction);
        var argumentFunction = FunctionType([], [PrimitiveType.Number], PrimitiveType.String);
        var result = TypeInferrer.InferFunctionTypeArguments(outerFunction, [argumentFunction]);
        Assert.Equal(PrimitiveType.Number, result[inputParameter]);
        Assert.Equal(PrimitiveType.String, result[outputParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_UnionWithSingleTypeParameter_Infer()
    {
        var elementParameter = TypeParameter("T");
        var union = new UnionType([elementParameter, PrimitiveType.Number]);
        var function = FunctionType([elementParameter], [union], elementParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.String]);
        Assert.Equal(PrimitiveType.String, result[elementParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_IntersectionWithSingleTypeParameter_Infer()
    {
        var elementParameter = TypeParameter("T");
        var intersection = new IntersectionType([elementParameter, PrimitiveType.Number]);
        var function = FunctionType([elementParameter], [intersection], elementParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [PrimitiveType.String]);
        Assert.Equal(PrimitiveType.String, result[elementParameter]);
    }

    [Fact]
    public void InferFunctionTypeArguments_GenericTypeArgument_InferTypeArgument()
    {
        var elementParameter = TypeParameter("T");
        var listGeneric = GenericType("List", [elementParameter], PrimitiveType.Unknown);
        var instantiatedParameter = new InstantiatedType(listGeneric, [elementParameter]);
        var instantiatedArgument = new InstantiatedType(listGeneric, [PrimitiveType.Number]);
        var function = FunctionType([elementParameter], [instantiatedParameter], elementParameter);
        var result = TypeInferrer.InferFunctionTypeArguments(function, [instantiatedArgument]);
        Assert.Equal(PrimitiveType.Unknown, result[elementParameter]);
    }
}