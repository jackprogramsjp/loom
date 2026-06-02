using System.Globalization;
using System.Numerics;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Literal(Token token, object? value)
    : Expression([token], [])
{
    public Token Token { get; } = token;
    public object? Value { get; } = value;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitLiteral(this);
    
    public static object? ResolveValue(Token token) =>
        token.Kind switch
        {
            SyntaxKind.NumberLiteral => ResolveNumber(token),
            SyntaxKind.StringLiteral => token.Text[1..^1],
            SyntaxKind.TrueLiteral => true,
            SyntaxKind.FalseLiteral => false,
            _ => null,
        };

    private static object ResolveNumber(Token token)
    {
        var value = ParseNumberValue(token);
        if (Math.Abs(Math.Floor(value) - value) < 1e-7)
            return (long)value;
        
        return value;
    }

    private static double ParseNumberValue(Token token)
    {
        var text = token.Text.Replace("_", "");
        return text switch
        {
            _ when text.EndsWith("hz", StringComparison.OrdinalIgnoreCase) => 1 / double.Parse(text[..^2]),
            _ when text.EndsWith("ms", StringComparison.OrdinalIgnoreCase) => double.Parse(text[..^2]) / 1000,
            _ when text.EndsWith("s", StringComparison.OrdinalIgnoreCase) => double.Parse(text[..^1]),
            _ when text.EndsWith("m", StringComparison.OrdinalIgnoreCase) => 60 * double.Parse(text[..^1]),
            _ when text.EndsWith("h", StringComparison.OrdinalIgnoreCase) => 3600 * double.Parse(text[..^1]),
            _ when text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) => long.Parse(text[2..], NumberStyles.HexNumber),
            _ when text.StartsWith("0b", StringComparison.OrdinalIgnoreCase) => long.Parse(text[2..], NumberStyles.BinaryNumber),
            _ when text.StartsWith("0o", StringComparison.OrdinalIgnoreCase) => Convert.ToInt64(text[2..], 8),
            _ => double.Parse(text),
        };
    }
}