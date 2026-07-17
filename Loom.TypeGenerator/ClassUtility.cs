using Loom.TypeGenerator.ApiTypes;

namespace Loom.TypeGenerator;

using System.Text.Json;

internal static class ClassUtility
{
    // public static string? SafePropertyType(string? valueType) =>
    //     string.IsNullOrEmpty(valueType)
    //         ? null
    //         : Constants.PropertyTypeMap.GetValueOrDefault(valueType, valueType);

    // public static string SafeRenamedInstance(string? name) =>
    //     name != null && Constants.RenamableAutoTypes.TryGetValue(name, out var value)
    //         ? value
    //         : SafeName(name);

    public static string SafeValueType(ApiTypes.ValueType valueType)
    {
        if (valueType.Category == "Enum")
            return $"Enum[\"{valueType.Name}\"]";

        var valueTypeName = SafeName(valueType.Name);
        if (string.IsNullOrEmpty(valueTypeName) || !valueTypeName.EndsWith('?'))
            return Constants.ValueTypeMap.GetValueOrDefault(valueType.Name, valueTypeName);

        var nonOptionalType = valueTypeName[..^1];
        return $"{Constants.ValueTypeMap.GetValueOrDefault(nonOptionalType, nonOptionalType)}?";
    }

    public static string? SafeReturnType(ApiTypes.ValueType[]? valueTypes)
    {
        var typeNames = valueTypes?
            .Select(SafeValueType)
            .Where(valueType => !string.IsNullOrEmpty(valueType))
            .Select(valueType => Constants.ReturnTypeMap.GetValueOrDefault(valueType, valueType))
            .ToArray();

        return typeNames?.Length switch
        {
            <= 0 => null,
            null => "void",
            1 => typeNames[0],
            > 1 => $"({string.Join(", ", typeNames)})" // undecided tuple syntax
        };
    }

    public static string? SafeParamName(string? name) =>
        name != null && Constants.ParameterNameMap.TryGetValue(name, out var value)
            ? value
            : name;

    public static Security GetSecurity(string className, MemberBase member)
    {
        if (!Constants.SecurityOverrides.TryGetValue(className, out var classSecurity))
            return member.MemberType switch
            {
                "Callback" => new Security { Read = "NotAccessibleSecurity", Write = member.Security?.ToString()! },
                "Function" => new Security { Read = member.Security?.ToString()!, Write = "NotAccessibleSecurity" },
                "Event" => new Security { Read = member.Security?.ToString()!, Write = "NotAccessibleSecurity" },
                "Property" => JsonSerializer.Deserialize<Security>(member.Security?.ToString()!)!,
                _ => throw new NotSupportedException($"Member type not supported: {member.MemberType}")
            };

        if (member.Name != null && classSecurity!.TryGetValue(member.Name, out var securityOverride)) return securityOverride;

        return member.MemberType switch
        {
            "Callback" => new Security { Read = "NotAccessibleSecurity", Write = member.Security?.ToString()! },
            "Function" => new Security { Read = member.Security?.ToString()!, Write = "NotAccessibleSecurity" },
            "Event" => new Security { Read = member.Security?.ToString()!, Write = "NotAccessibleSecurity" },
            "Property" => JsonSerializer.Deserialize<Security>(member.Security?.ToString()!)!,
            _ => throw new NotSupportedException($"Member type not supported: {member.MemberType}")
        };
    }

    public static bool HasTag(MemberBase container, string tag) => container.Tags != null && container.Tags.Select(t => t.ToString()).Contains(tag);

    public static bool HasTag(Class container, string tag) => container.Tags != null && container.Tags.Select(t => t.ToString()).Contains(tag);

    public static bool IsCreatable(Class rbxClass) =>
        !Constants.CreatableBlacklist.Contains(rbxClass.Name)
        && !HasTag(rbxClass, "NotCreatable")
        && !HasTag(rbxClass, "Service");

    public static string FormatComment(string s) => "#:\n" + string.Join('\n', s.Trim().Split('\n')) + "\n:#";

    public static string SafeName(string? name) =>
        name == null
            ? ""
            : ContainsBadCharacter(name)
                ? name.Replace("\"", "\\\"")
                : name;

    private static bool ContainsBadCharacter(string name) => Constants.BadNameCharacters.Any(name.Contains);
}