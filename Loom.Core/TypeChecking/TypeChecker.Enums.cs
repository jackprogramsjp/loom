using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public sealed partial class TypeChecker
{
    public override Type VisitEnumDeclaration(EnumDeclaration enumDeclaration)
    {
        var baseType = MaybeVisit(enumDeclaration.ColonTypeClause) ?? Types.PrimitiveType.Number;
        if (enumDeclaration.ColonTypeClause != null
            && !baseType.IsAssignableTo(Types.PrimitiveType.String)
            && !baseType.IsAssignableTo(Types.PrimitiveType.Number))
        {
            _diagnostics.Error(
                enumDeclaration.ColonTypeClause,
                InternalCodes.InvalidEnumBaseType,
                "Invalid enum base type.",
                "valid types are 'string' and 'number'"
            );

            return BindType(enumDeclaration, Types.PrimitiveType.Never);
        }

        var properties = baseType.IsAssignableTo(Types.PrimitiveType.String)
            ? BuildStringEnumProperties(enumDeclaration, baseType)
            : BuildNumericEnumProperties(enumDeclaration, baseType);

        return properties != null
            ? BindType(enumDeclaration, new ObjectType(null, properties))
            : BindType(enumDeclaration, Types.PrimitiveType.Never);
    }

    private List<ObjectProperty> BuildNumericEnumProperties(EnumDeclaration enumDeclaration, Type baseType)
    {
        var properties = new List<ObjectProperty>();
        var nextValue = 0d;
        foreach (var member in enumDeclaration.Members)
        {
            var memberValue = nextValue;
            if (member.EqualsValueClause != null)
            {
                var explicitType = Visit(member.EqualsValueClause);
                if (CheckEnumMemberIsConstant(member, explicitType))
                {
                    memberValue = ExtractNumericLiteralValue(explicitType, nextValue);
                    _semanticModel.TypeSolver.AddConstraint(explicitType, baseType, member.EqualsValueClause.Value);
                }
            }

            properties.Add(new ObjectProperty(false, member.Name.Text, new Types.LiteralType(memberValue)));
            nextValue = memberValue + 1;
        }

        return properties;
    }

    private List<ObjectProperty>? BuildStringEnumProperties(EnumDeclaration enumDeclaration, Type baseType)
    {
        var properties = new List<ObjectProperty>();
        foreach (var member in enumDeclaration.Members)
        {
            if (member.EqualsValueClause == null)
            {
                _diagnostics.Error(
                    member,
                    InternalCodes.StringEnumMemberMustHaveInitializer,
                    $"Member '{member.Name.Text}' of string enum '{enumDeclaration.Name.Text}' must have an initializer."
                );

                return null;
            }

            var type = Visit(member.EqualsValueClause);
            if (!CheckEnumMemberIsConstant(member, type)) continue;

            _semanticModel.TypeSolver.AddConstraint(type, baseType, member.EqualsValueClause.Value);
            properties.Add(new ObjectProperty(false, member.Name.Text, type));
        }

        return properties;
    }

    private static double ExtractNumericLiteralValue(Type type, double fallback) =>
        type switch
        {
            Types.LiteralType { Value: long l } => l,
            Types.LiteralType { Value: int i } => i,
            Types.LiteralType { Value: double d } => d,
            _ => fallback
        };

    private bool CheckEnumMemberIsConstant(EnumMember member, Type type)
    {
        if (type is Types.LiteralType { Value: string or long or int or double })
            return true;

        _diagnostics.Error(
            member.EqualsValueClause!.Value,
            InternalCodes.DynamicEnumMemberInitializer,
            "Enum member initializers must be constant values."
        );

        return false;
    }
}