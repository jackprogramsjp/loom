using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Loom.TypeGenerator;

internal partial class ReflectionMetadataReader(string body)
{
    private readonly XElement _metadata = XElement.Parse(body);

    public string ReadFunctionDescriptor(string className, string name) =>
        ReadMemberDescriptor(className, name, ["ReflectionMetadataFunctions", "ReflectionMetadataYieldFunctions"]);

    public string ReadPropertyDescriptor(string className, string name) => ReadMemberDescriptor(className, name, ["ReflectionMetadataProperties"]);
    public string ReadCallbackDescriptor(string className, string name) => ReadMemberDescriptor(className, name, ["ReflectionMetadataCallbacks"]);
    public string ReadEventDescriptor(string className, string name) => ReadMemberDescriptor(className, name, ["ReflectionMetadataEvents"]);
    public string ReadClassDescriptor(string name) => Get($"{ClassPrefix(name)}Properties/string[@name='summary']");

    private string Get(string query)
    {
        var result = _metadata.XPathSelectElement(query);
        return result != null ? HyperlinkToMarkdown(result.ToString()) : string.Empty;
    }

    public string ReadMemberDescriptor(string className, string name, string[] specifier)
    {
        var specifierString = string.Join(" or ", specifier.Select(v => $"@class='{v}'"));
        var query = $"{ClassPrefix(className)}Item[{specifierString}]/"
            + "Item[@class='ReflectionMetadataMember']/"
            + "Properties/"
            + $"string[@name='Name'][text()='{name}']"
            + "/../string[@name='summary']";

        return Get(query);
    }

    private static string ClassPrefix(string name) => $"//Item[@class='ReflectionMetadataClass']/Properties/string[@name='Name'][text()='{name}']/../../";
    private static string HyperlinkToMarkdown(string s) => FilterRegex().Replace(s, m => $"[{m.Groups[2].Value}]({m.Groups[1].Value})");

    [GeneratedRegex("<a href=\"([^\"]+)\"[^>]+>([^<]+)</a>")]
    private static partial Regex FilterRegex();
}