using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Luau;
using Loom.Luau.AST;
using TypeName = Loom.Luau.AST.TypeName;

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

                var typeArguments = traitDeclaration.TypeParameters?.ParameterList.ConvertAll(LuauType (p) => new Luau.AST.TypeName(p.Name.Text));
                parameterTypes.Insert(0, new Luau.AST.TypeName(traitDeclaration.Name.Text, typeArguments));

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

        var properties = propertyDeclarations.Select(p => new TableTypeProperty(
                    p.MutKeyword == null ? LuauVisibility.Read : null,
                    p.Name.Text,
                    Visit(p.ColonTypeClause)
                )
            )
            .ToList();

        var tableType = new TableType(tableIndexer, properties);
        var typeParameters = GenerateTypeParameters(interfaceDeclaration.TypeParameters);
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

    public override LuauNode VisitInterfaceInvocationPropertyInitializer(InterfaceInvocationPropertyInitializer propertyInitializer) =>
        new PropertyTableInitializer(propertyInitializer.Name.Text, Visit(propertyInitializer.Expression));

    public override LuauNode VisitInterfaceInvocationIndexInitializer(InterfaceInvocationIndexInitializer indexInitializer) =>
        new ComputedPropertyTableInitializer(Visit(indexInitializer.IndexExpression), Visit(indexInitializer.Expression));

    private static string GetImplementationMetaName(Implement implement)
    {
        var typeArguments = implement.TraitName.TypeArguments;
        var typeArgumentText = typeArguments != null ? '_' + string.Join('_', typeArguments.ArgumentsList.Select(a => a.ToString())) : "";
        return $"{implement.TraitName.Name.Text}{typeArgumentText}_for_{implement.InterfaceName.Name.Text}";
    }
}