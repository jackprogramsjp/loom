using Loom.Core.Parsing.AST;
using Loom.Core.TypeChecking.Types;
using Loom.Luau;
using Loom.Luau.AST;
using LiteralType = Loom.Core.TypeChecking.Types.LiteralType;
using OptionalType = Loom.Luau.AST.OptionalType;
using Parameter = Loom.Core.Parsing.AST.Parameter;
using PrimitiveType = Loom.Core.TypeChecking.Types.PrimitiveType;
using TypeAlias = Loom.Core.Parsing.AST.TypeAlias;
using TypeParameters = Loom.Luau.AST.TypeParameters;
using UnionType = Loom.Core.TypeChecking.Types.UnionType;

namespace Loom.Core.Generation;

public sealed partial class LuauGenerator
{
    public override LuauNode VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var typeParameters = MaybeVisit<TypeParameters>(functionDeclaration.TypeParameters);
        if (typeParameters != null)
            foreach (var typeParameter in typeParameters.Parameters)
                typeParameter.OfFunction = true;

        var parameters = functionDeclaration.Parameters?.ParameterList.ConvertAll(VisitParameter) ?? [];
        var returnType = MaybeVisit<LuauType>(functionDeclaration.ReturnType);
        var statements = GenerateFunctionBody(functionDeclaration);
        return new Function(functionDeclaration.Name.Text, typeParameters, parameters, returnType, statements);
    }

    public override Luau.AST.Parameter VisitParameter(Parameter parameter)
    {
        var type = MaybeVisit<LuauType>(parameter.ColonTypeClause?.Type);
        if (type != null && parameter.EqualsValueClause != null && type is not OptionalType)
            type = new OptionalType(type);

        return new Luau.AST.Parameter(parameter.Name.Text, type);
    }

    public override LuauNode VisitTypeAlias(TypeAlias typeAlias)
    {
        var typeParameters = typeAlias.TypeParameters != null
            ? Visit<TypeParameters>(typeAlias.TypeParameters)
            : new TypeParameters();

        var type = Visit(typeAlias.EqualsTypeClause.Type);
        return new Luau.AST.TypeAlias(typeAlias.Name.Text, typeParameters, type);
    }

    public override LuauNode VisitEnumDeclaration(EnumDeclaration enumDeclaration)
    {
        if (_semanticModel.GetType(enumDeclaration) is not ObjectType objectType)
            return LuauFactory.EmptyVariable();

        var propertyUnion = objectType.PropertyUnion();
        return new Luau.AST.TypeAlias(
            enumDeclaration.Name.Text,
            new TypeParameters(),
            propertyUnion switch
            {
                UnionType union =>
                    union.Types.Any(t => t.IsAssignableTo(PrimitiveType.Number))
                        ? Luau.AST.PrimitiveType.Number
                        : new Luau.AST.UnionType(
                            union.Types.ConvertAll(t => t is LiteralType { Value: string s }
                                    ? new StringLiteralType(s)
                                    : Luau.AST.PrimitiveType.Number
                                )
                                .OfType<LuauType>()
                                .ToList()
                        ),
                LiteralType { Value: string s } => new StringLiteralType(s),
                _ => Luau.AST.PrimitiveType.Number
            }
        );
    }
}