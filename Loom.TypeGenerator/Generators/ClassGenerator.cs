using Loom.TypeGenerator.ApiTypes;

namespace Loom.TypeGenerator.Generators;

internal sealed class ClassGenerator(
    string filePath,
    ReflectionMetadataReader metadata,
    HashSet<string> definedClassNames,
    string security,
    string? lowerSecurity = null
)
    : BaseGenerator(filePath, metadata)
{
    private readonly Dictionary<string, Class> _classRefs = [];
    private readonly Dictionary<string, HashSet<string>> _definedMemberNames = [];

    public void Generate(Class[] rbxClasses)
    {
        foreach (var rbxClass in rbxClasses)
        {
            var className = rbxClass.Name;
            // rbxClass.Subclasses = [];
            _classRefs[className] = rbxClass;

            // var superclass = GetSuperclass(rbxClass);
            // superclass?.Subclasses.Add(className);
        }

        var classesToGenerate = rbxClasses.Where(ShouldGenerateClass).ToArray();
        GenerateClasses(classesToGenerate);
        WriteContentsOfFile("roblox_ext.loom");
    }


    private void GenerateClasses(Class[] rbxClasses)
    {
        Write("## Generated Roblox instance classes");
        Write();
        foreach (var rbxClass in rbxClasses)
            GenerateClass(rbxClass);
    }
    
    private void GenerateClass(Class rbxClass)
    {
        definedClassNames.Add(rbxClass.Name);
        _definedMemberNames[rbxClass.Name] = [];

        var className = ClassUtility.AssertClassName(rbxClass.Name, _classRefs);
        var members = rbxClass.Members;
        var noSecurity = security == "None" || IsPluginOnlyClass(rbxClass);
        switch (noSecurity)
        {
            case true:
            {
                var description = !string.IsNullOrEmpty(rbxClass.Description)
                    ? rbxClass.Description
                    : Metadata.ReadClassDescription(rbxClass.Name);
                
                WriteDescription(description);
                break;
            }

            case false when members.Length == 0:
                return;
        }

        if (className == "Studio") return;
        
        var membersToGenerate = members.Where(member => ShouldGenerateMember(rbxClass, member)).ToArray();
        var isValidSuperclass = rbxClass.Superclass != Constants.RootClassName;
        var superclassText = isValidSuperclass ? $": {rbxClass.Superclass}" : "";
        var emitCreatable = ClassUtility.IsCreatable(rbxClass) && !ClassUtility.HasMatchingSuperclass(rbxClass, _classRefs, ClassUtility.IsCreatable);
        var emitService = ClassUtility.IsService(rbxClass) && !ClassUtility.HasMatchingSuperclass(rbxClass, _classRefs, ClassUtility.IsService);
        var useBraces = emitCreatable || emitService || membersToGenerate.Length > 0;
        Write($"declare interface {className}{superclassText}{(useBraces ? " {" : ";")}");
        if (useBraces)
            PushIndent();
        
        if (emitCreatable)
            Write("_is_creatable: true");

        if (emitService)
            Write("_is_service: true");

        foreach (var member in membersToGenerate)
        {
            _definedMemberNames[rbxClass.Name].Add(member.Name!.Trim());
            switch (member)
            {
                case Property property:
                    GenerateProperty(property, rbxClass);
                    continue;
                // case Event @event:
                //     GenerateEvent(@event, rbxClass);
                //     break;
                // case Function function:
                //     GenerateFunction(function, rbxClass);
                //     break;
                case not Event and not Function and Callback callback:
                    GenerateCallback(callback, rbxClass);
                    continue;
                // default:
                //     Log.Fatal($"received unsupported member type: {member.MemberType}");
                //     break;
            }
        }

        if (!useBraces) return;
        PopIndent();
        Write("}");
        Write();
    }

    private void WriteDescription(string description)
    {
        if (!string.IsNullOrEmpty(description))
            Write(ClassUtility.FormatComment(description));
    }
    
    private void GenerateProperty(Property property, Class rbxClass)
    {
        var valueType = ClassUtility.SafeValueType(property.ValueType);
        var (name, description) = GetMemberNameAndDescription(property, rbxClass);
        var definitelyDefined = property.ValueType.Category != "Class";
        var extraPropertyData = CanWrite(rbxClass.Name, property) && !ClassUtility.HasTag(property, "ReadOnly") ? "mut " : "";
        WriteDescription(description);
        Write($"{extraPropertyData}{name.Replace(" ", "")}: {valueType}{(definitelyDefined || valueType.EndsWith('?') ? "" : "?")};");
    }

    // private void GenerateEvent(Event @event, Class rbxClass)
    // {
    //     var paramTypeList = GenerateParameterList(@event.Parameters);
    //     var (name, description) = GetMemberNameAndDescription(@event, rbxClass);
    //     WriteDescription(description);
    //     Write($"event {name}{paramTypeList};");
    // }
    
    private void GenerateCallback(Callback callback, Class rbxClass)
    {
        if (callback.ReturnType is { Length: > 1 })
            return; // skip for now cause no tuple types
        
        var (name, description) = GetMemberNameAndDescription(callback, rbxClass);
        var parameterList = GenerateParameterList(callback.Parameters);
        var returnType = ClassUtility.SafeReturnType(callback.ReturnType);
        WriteDescription(description);
        Write($"mut {name}: fn{parameterList}: {returnType};");
    }

    private static string GenerateParameterList(Parameter[] parameters) =>
        parameters.Length > 0
            ? '(' + string.Join(", ", parameters.Select(GenerateParameter)) + ')'
            : "";

    private static string GenerateParameter(Parameter param) => ClassUtility.SafeParamName(param.Name) + ": " + ClassUtility.SafeValueType(param.Type);

    private (string Name, string Description) GetMemberNameAndDescription(MemberBase member, Class rbxClass)
    {
        var name = ClassUtility.SafeName(member.Name);
        Func<string, string, string>? readDescription = member switch
        {
            Property => Metadata.ReadPropertyDescription,
            Event => Metadata.ReadEventDescription,
            Function => Metadata.ReadFunctionDescription,
            Callback => Metadata.ReadCallbackDescription,
            _ => null
        };
        
        var description = readDescription == null || !string.IsNullOrWhiteSpace(member.Description)
            ? member.Description
            : readDescription(rbxClass.Name, name);

        return (name, description);
    }
    
    private Class? GetSuperclass(Class rbxClass) =>
        rbxClass.Superclass != Constants.RootClassName
            ? _classRefs[rbxClass.Superclass]
            : null;

    private bool CanRead(string className, MemberBase member)
    {
        var readSecurity = ClassUtility.GetSecurity(className, member).Read;
        return readSecurity == security
            || Constants.PluginOnlyClasses.Contains(className) && readSecurity == lowerSecurity;
    }

    private bool CanWrite(string className, MemberBase member)
    {
        var security1 = ClassUtility.GetSecurity(className, member);
        if (security1 is { Read: "None", Write: "PluginSecurity" })
            return true; // dumb hack to fix PluginSecurity writable things being marked as readonly in None.loom

        return security1.Write == security || Constants.PluginOnlyClasses.Contains(className) && security1.Write == lowerSecurity;
    }

    private bool IsPluginOnlyClass(Class rbxClass) =>
        Constants.PluginOnlyClasses.Contains(rbxClass.Name)
        || GetSuperclass(rbxClass) is { } superClass && IsPluginOnlyClass(superClass);

    private bool ShouldGenerateClass(Class rbxClass)
    {
        var superClass = rbxClass.Superclass != Constants.RootClassName ? _classRefs[rbxClass.Superclass] : null;
        if (superClass != null && !ShouldGenerateClass(superClass)) return false;
        if (Constants.ClassBlacklist.Contains(rbxClass.Name)) return false;

        return security == "PluginSecurity" || !Constants.PluginOnlyClasses.Contains(rbxClass.Name);
    }

    private bool ShouldGenerateMember(Class rbxClass, MemberBase member)
    {
        if (member.Name == null)
            return false;

        var isBlacklisted = Constants.MemberBlacklist.TryGetValue(rbxClass.Name, out var value) && value.Contains(member.Name);
        if (isBlacklisted || !CanRead(rbxClass.Name, member))
            return false;

        if (ClassUtility.HasTag(member, "Deprecated"))
        {
            var firstCharacter = member.Name[0];
            if (char.IsLower(firstCharacter))
            {
                var pascalCaseName = char.ToUpper(firstCharacter) + member.Name[1..];
                var pascalCaseMember = rbxClass.Members.FirstOrDefault(v => v.Name == pascalCaseName);
                if (pascalCaseMember != null)
                    return false;
            }
        }

        if (ClassUtility.HasTag(member, "Hidden")) return false;
        if (ClassUtility.HasTag(member, "NotScriptable")) return false;
        return !_definedMemberNames[rbxClass.Name].Contains(member.Name.Trim());
    }
}