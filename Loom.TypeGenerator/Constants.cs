namespace Loom.TypeGenerator;

internal static class Constants
{
    public const string RootClassName = "<<<ROOT>>>";
    public static readonly List<string> BadNameCharacters = [" ", "/", "\""];

    public static readonly Dictionary<string, Dictionary<string, ApiTypes.Security>?> SecurityOverrides = new()
    {
        ["StarterGui"] = new Dictionary<string, ApiTypes.Security> { ["ShowDevelopmentGui"] = new() { Read = "PluginSecurity", Write = "PluginSecurity" } }
    };
}