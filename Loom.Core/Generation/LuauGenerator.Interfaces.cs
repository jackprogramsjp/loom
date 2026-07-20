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
        var propertySymbol = _semanticModel.GetDeclarationSymbol(property, SymbolKind.Property) as PropertySymbol;
        var type = Visit(property.ColonTypeClause);
        if (propertySymbol?.Attributes.Find(a => a is { Name: "luau_method", IsIntrinsic: true }) is not { } luauMethodAttribute)
            return new TableTypeProperty(visibility, name, type);

        if (type is not Luau.AST.FunctionType functionType)
        {
            _diagnostics.Error(
                luauMethodAttribute.Declaration,
                InternalCodes.InvalidLuauMethodAttribute,
                "May only use function types directly when using 'luau_method' attribute"
            );

            return new TableTypeProperty(visibility, name, type);
        }

        var typeArguments = typeParameters.Parameters.Count > 0
            ? typeParameters.Parameters.ConvertAll(LuauType (param) => new TypeName(param.Name))
            : null;

        var selfType = new TypeName(interfaceDeclaration.Name.Text, typeArguments);
        functionType.ParameterTypes.Insert(0, selfType);

        return new TableTypeProperty(visibility, name, type);
    }

    public override LuauNode VisitInterfaceInvocation(InterfaceInvocation interfaceInvocation)
    {
        var symbol = _semanticModel.GetSymbol(interfaceInvocation.Name, SymbolKind.Interface);
        if (symbol is not InterfaceSymbol interfaceSymbol)
            return new NoOpStatement();

        var table = Visit<Table>(interfaceInvocation.Body);
        if (interfaceSymbol.Implements.Count == 0)
            return table;

        _semanticModel.RuntimeReferences += 1; // merge_meta;
        var metaNames = interfaceSymbol.Implementations.ConvertAll(LuauExpression (i) => new Luau.AST.Identifier(GetImplementationMetaName(i)));
        var call = LuauFactory.SetMetatableCall(table, LuauFactory.RuntimeLibraryCall(["merge_meta"], metaNames));
        return new TypeCast(call, new TypeName(interfaceInvocation.Name.Token.Text));
    }

    public override LuauNode VisitInterfaceInvocationBody(InterfaceInvocationBody interfaceInvocationBody) =>
        new Table(interfaceInvocationBody.Initializers.ConvertAll(Visit<TableInitializer>));

    public override LuauNode VisitInterfaceInvocationIndexInitializer(InterfaceInvocationIndexInitializer indexInitializer) =>
        new ComputedPropertyTableInitializer(Visit(indexInitializer.IndexExpression), Visit(indexInitializer.Expression));

    public override LuauNode VisitInterfaceInvocationPropertyInitializer(InterfaceInvocationPropertyInitializer propertyInitializer) =>
        new PropertyTableInitializer(propertyInitializer.Name.Text, Visit(propertyInitializer.Expression));

    public override LuauNode VisitInterfaceInvocationShorthandPropertyInitializer(InterfaceInvocationShorthandPropertyInitializer shorthandPropertyInitializer) =>
        new PropertyTableInitializer(shorthandPropertyInitializer.Identifier.Name.Text, Visit(shorthandPropertyInitializer.Identifier));

    private static string GetImplementationMetaName(Implement implement)
    {
        var typeArguments = implement.TraitName.TypeArguments;
        var typeArgumentText = typeArguments != null ? '_' + string.Join('_', typeArguments.ArgumentsList.Select(a => a.ToString())) : "";
        return $"{implement.TraitName.Name.Text}{typeArgumentText}_for_{implement.InterfaceName.Name.Text}";
    }
}