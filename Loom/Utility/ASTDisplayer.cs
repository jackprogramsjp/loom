using System.Reflection;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Utility;

public class ASTDisplayer(Tree ast) : Visitor<string>
{
    private int _indent;
    private static readonly HashSet<string> _ignoredProperties = ["Parent", "Span", "Tokens", "Children", "Id", "Keyword"];

    public void Display()
    {
        var content = DisplayNode(ast);
        Console.WriteLine(content);
    }

    protected override string Visit(Node node) => node.Accept(this);
    public override string VisitLiteral(Literal literal) => $"Literal({literal})";
    public override string VisitIdentifier(Identifier identifier) => $"Identifier({identifier})";
    public override string VisitTypeName(TypeName typeName) => $"TypeName({typeName})";
    public override string VisitLiteralType(LiteralType literalType) => $"LiteralType({literalType})";

    public override string VisitPrimitiveType(PrimitiveType primitiveType) => $"PrimitiveType({primitiveType})";

    private string DisplayNode(object node)
    {
        var type = node.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !_ignoredProperties.Contains(p.Name) && p.GetIndexParameters().Length == 0);

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => !_ignoredProperties.Contains(f.Name));

        _indent++;
        var members = (
            from prop in properties
            let value = prop.GetValue(node)
            let display = FormatValue(value)
            select Indented(display.Split('(').First() == prop.Name ? display : $"{prop.Name}: {display}")).ToList();

        _indent--;

        members.AddRange(
            from field in fields
            let value = field.GetValue(node)
            let display = FormatValue(value)
            select Indented(display.Split('(').First() == field.Name ? display : $"{field.Name}: {display}")
        );

        return type.Name + "(\n" + string.Join('\n', members) + "\n" + Indented(")");
    }

    private string FormatValue(object? value)
    {
        switch (value)
        {
            case null:
                return "null";
            case string s:
                return $"\"{s}\"";
            case Token token:
                return $"Token({token.Kind}, \"{token.Text}\")";
            case Node node:
            {
                return DisplayNode(node);
            }
            case IEnumerable<object> nodes:
            {
                _indent++;
                var items = nodes.Select(n => Indented(DisplayNode(n))).ToList();
                _indent--;
                return items.Count == 0
                    ? "[]"
                    : "[\n" + string.Join('\n', items) + "\n" + Indented("]");
            }
            
            default:
                return value.ToString() ?? "???";
        }
    }

    private string Indented(string content) => new string(' ', 2 * _indent) + content;
}