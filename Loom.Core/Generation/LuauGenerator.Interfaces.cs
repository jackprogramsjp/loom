using System.Diagnostics.CodeAnalysis;
using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Luau;
using Loom.Luau.AST;
using TypeName = Loom.Luau.AST.TypeName;
using TypeParameters = Loom.Luau.AST.TypeParameters;

namespace Loom.Core.Generation;

public sealed partial class LuauGenerator
{
    public override LuauNode VisitImplement(Implement implement)
    {
        var interfaceName = Visit(implement.InterfaceName);
        var metaName = GetImplementationMetaName(implement);
        var identifier = new Luau.AST.Identifier(metaName);
        var variable = new LocalVariable(metaName, null, new Table([]));
        _state.Postreq(new Luau.AST.ExpressionStatement(new Luau.AST.BinaryOperator(new Luau.AST.PropertyAccess(identifier, ["__index"]), "=", identifier)));
        _state.Postreq(new Luau.AST.ExpressionStatement(new Luau.AST.BinaryOperator(identifier, "=", new TypeCast(identifier, interfaceName))));

        foreach (var declaration in implement.Body.Implementations)
        {
            var typeParameters = MaybeVisit<Luau.AST.TypeParameters>(declaration.TypeParameters);
            var parameters = declaration.Parameters?.ParameterList.ConvertAll(Visit<Luau.AST.Parameter>) ?? [];
            parameters.Insert(0, new Luau.AST.Parameter("self", interfaceName));

            var returnType = MaybeVisit<LuauType>(declaration.ReturnType);
            var body = GenerateFunctionBody(declaration);
            _state.Postreq(
                new Function(
                    $"{metaName}.{declaration.Name.Text}",
                    typeParameters,
                    parameters,
                    returnType,
                    body,
                    false
                )
            );
        }

        return variable;
    }

    public override LuauNode VisitTraitDeclaration(TraitDeclaration traitDeclaration)
    {
        var signatures = traitDeclaration.Body.Members;
        var properties = signatures.ConvertAll(signature =>
            {
                var parameterTypes = signature.Parameters?.ParameterList.FindAll(p => p.ColonTypeClause != null).ConvertAll(p => Visit(p.ColonTypeClause!.Type))
                    ?? [];

                var typeArguments = traitDeclaration.TypeParameters?.ParameterList.ConvertAll(LuauType (p) => new TypeName(p.Name.Text));
                parameterTypes.Insert(0, new TypeName(traitDeclaration.Name.Text, typeArguments));

                return new TableTypeProperty(
                    null,
                    signature.Name.Text,
                    new Luau.AST.FunctionType(
                        GenerateTypeParameters(signature.TypeParameters),
                        parameterTypes,
                        Visit(signature.ReturnType)
                    )
                );
            }
        );

        var tableType = new TableType(null, properties);
        var typeParameters = GenerateTypeParameters(traitDeclaration.TypeParameters);
        return new Luau.AST.TypeAlias(traitDeclaration.Name.Text, typeParameters, tableType);
    }

    public override LuauNode VisitInterfaceDeclaration(InterfaceDeclaration interfaceDeclaration)
    {
        var symbol = _semanticModel.GetDeclarationSymbol(interfaceDeclaration, SymbolKind.Interface);
        if (symbol is not InterfaceSymbol interfaceSymbol)
            return new NoOpStatement();

        var indexer = interfaceDeclaration.Body?.Members.OfType<IndexerDeclaration>().FirstOrDefault();
        var propertyDeclarations = interfaceDeclaration.Body?.Members.OfType<PropertyDeclaration>() ?? [];
        var tableIndexer = indexer != null
            ? new TableTypeIndexer(indexer.MutKeyword == null ? LuauVisibility.Read : null, Visit(indexer.IndexType), Visit(indexer.ColonTypeClause))
            : null;

        var typeParameters = GenerateTypeParameters(interfaceDeclaration.TypeParameters);
        var properties = propertyDeclarations
            .Select(property => GenerateInterfacePropertyType(interfaceDeclaration, property, typeParameters))
            .ToList();

        var tableType = new TableType(tableIndexer, properties);
        return new Luau.AST.TypeAlias(
            interfaceDeclaration.Name.Text,
            typeParameters,
            interfaceDeclaration.ColonTypeListClause != null || interfaceSymbol.Implementations.Count > 0
                ? new Luau.AST.IntersectionType(
                    [
                        ..interfaceDeclaration.ColonTypeListClause?.Types.ConvertAll(Visit) ?? [],
                        tableType,
                        ..interfaceSymbol.Implementations.ConvertAll(i => Visit(i.TraitName))
                    ]
                )
                : tableType
        );
    }

    private TableTypeProperty GenerateInterfacePropertyType(InterfaceDeclaration interfaceDeclaration, PropertyDeclaration property, TypeParameters typeParameters)
    {
        LuauVisibility? visibility = property.MutKeyword == null ? LuauVisibility.Read : null;
        var name = property.Name.Text;
        var type = Visit(property.ColonTypeClause);
        var tableProperty = new TableTypeProperty(visibility, name, type);
        if (property.TryGetIntrinsicAttribute(_semanticModel, "luau_name", out var luauNameAttribute)
            && ValidateLuauNameAttribute(luauNameAttribute, out var nameLiteral))
        {
            tableProperty = new TableTypeProperty(visibility, nameLiteral.Value, type);
        }

        if (!property.TryGetIntrinsicAttribute(_semanticModel, "luau_method", out var luauMethodAttribute))
            return tableProperty;

        if (type is not Luau.AST.FunctionType functionType)
        {
            _diagnostics.Error(
                luauMethodAttribute.Attribute,
                InternalCodes.InvalidLuauMethodAttribute,
                "May only use function types directly when using 'luau_method' attribute"
            );

            return tableProperty;
        }

        var typeArguments = typeParameters.Parameters.Count > 0
            ? typeParameters.Parameters.ConvertAll(LuauType (param) => new TypeName(param.Name))
            : null;

        var selfType = new TypeName(interfaceDeclaration.Name.Text, typeArguments);
        functionType.ParameterTypes.Insert(0, selfType);

        return tableProperty;
    }

    private bool ValidateLuauNameAttribute(AttributeSymbol luauNameAttribute, [MaybeNullWhen(false)] out StringLiteral nameLiteral)
    {
        var luauName = Visit(luauNameAttribute.Attribute.Arguments.ArgumentList[0]);
        if (luauName is not StringLiteral stringLiteral)
        {
            _diagnostics.Error(
                luauNameAttribute.Attribute,
                InternalCodes.InvalidLuauNameAttribute,
                "May only use string literals for name parameter on 'luau_name' attribute"
            );

            nameLiteral = null;
            return false;
        }

        nameLiteral = stringLiteral;
        return true;
    }

    public override LuauNode VisitInterfaceInvocation(InterfaceInvocation interfaceInvocation)
    {
        var symbol = _semanticModel.GetSymbol(interfaceInvocation.Name, SymbolKind.Interface);
        if (symbol is not InterfaceSymbol interfaceSymbol)
            return new NoOpStatement();

        var table = GenerateInterfaceInvocationBody(interfaceInvocation.Body, interfaceSymbol);
        if (interfaceSymbol.Implements.Count == 0)
            return table;

        _semanticModel.RuntimeReferences += 1; // merge_meta;
        var metaNames = interfaceSymbol.Implementations.ConvertAll(LuauExpression (i) => new Luau.AST.Identifier(GetImplementationMetaName(i)));
        var call = LuauFactory.SetMetatableCall(table, LuauFactory.RuntimeLibraryCall(["merge_meta"], metaNames));
        return new TypeCast(call, new TypeName(interfaceInvocation.Name.Token.Text));
    }

    private Table GenerateInterfaceInvocationBody(InterfaceInvocationBody interfaceInvocationBody, InterfaceSymbol interfaceSymbol) =>
        new(
            interfaceInvocationBody.Initializers.ConvertAll(TableInitializer (i) => i switch
                {
                    InterfaceInvocationIndexInitializer indexInitializer => GenerateInterfaceInvocationIndexInitializer(indexInitializer),
                    InterfaceInvocationPropertyInitializer propertyInitializer => GenerateInterfaceInvocationPropertyInitializer(
                        propertyInitializer,
                        interfaceSymbol
                    ),
                    InterfaceInvocationShorthandPropertyInitializer shorthandPropertyInitializer => GenerateInterfaceInvocationShorthandPropertyInitializer(
                        shorthandPropertyInitializer,
                        interfaceSymbol
                    ),
                    _ => (_diagnostics.CompilerError(i, "Unhandled interface invocation initializer type") as TableInitializer)!
                }
            )
        );

    private ComputedPropertyTableInitializer GenerateInterfaceInvocationIndexInitializer(InterfaceInvocationIndexInitializer indexInitializer) =>
        new(Visit(indexInitializer.IndexExpression), Visit(indexInitializer.Expression));

    private PropertyTableInitializer GenerateInterfaceInvocationPropertyInitializer(
        InterfaceInvocationPropertyInitializer propertyInitializer,
        InterfaceSymbol interfaceSymbol)
    {
        var name = GetRenamedPropertyName(interfaceSymbol, propertyInitializer.Name.Text);
        var initializedValue = Visit(propertyInitializer.Expression);
        return new PropertyTableInitializer(name, initializedValue);
    }

    private string GetRenamedPropertyName(InterfaceSymbol interfaceSymbol, string name)
    {
        var propertySymbol = interfaceSymbol.GetPropertyAtPath([name]);
        return propertySymbol == null
            || !propertySymbol.TryGetIntrinsicAttribute("luau_name", out var luauNameAttribute)
            || !ValidateLuauNameAttribute(luauNameAttribute, out var nameLiteral)
                ? name
                : nameLiteral.Value;
    }

    private PropertyTableInitializer GenerateInterfaceInvocationShorthandPropertyInitializer(
        InterfaceInvocationShorthandPropertyInitializer shorthandPropertyInitializer,
        InterfaceSymbol interfaceSymbol) =>
        new(GetRenamedPropertyName(interfaceSymbol, shorthandPropertyInitializer.Identifier.Name.Text), Visit(shorthandPropertyInitializer.Identifier));

    private static string GetImplementationMetaName(Implement implement)
    {
        var typeArguments = implement.TraitName.TypeArguments;
        var typeArgumentText = typeArguments != null ? '_' + string.Join('_', typeArguments.ArgumentsList.Select(a => a.ToString())) : "";
        return $"{implement.TraitName.Name.Text}{typeArgumentText}_for_{implement.InterfaceName.Name.Text}";
    }
}