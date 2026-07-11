using System.Text.RegularExpressions;
using Loom.Core.Diagnostics;
using Loom.Core.Text;

namespace Loom.Core.Lexing;

using static LexerRule;
using static SyntaxKind;

public static class LexerRules
{
    private const string Int = @"\d[\d_]*\d*_*";
    private const string FloatScientific = $"({Float}{Exponent})";
    private const string Float = $@"({Int}\.{Int}|\.{Int}|{Int}\.{Int})";
    private const string IntScientific = $"({Int}{Exponent})";
    private const string Exponent = $"[eE]-?{Int}";
    private const string HexInt = "(0[xX][a-fA-F0-9_]+)";
    private const string BinaryInt = "(0[bB][01_]+)";
    private const string OctalInt = "(0[oO][0-7_]+)";
    private const string Number = $"({HexInt}|{BinaryInt}|{OctalInt}|{FloatScientific}|{IntScientific}|{Float}|{Int})";
    private const string HzNumber = $"({Number}[hH][zZ])";
    private const string MsNumber = $"({Number}[mM][sS])";
    private const string SecondsNumber = $"({Number}[sS])";
    private const string MinutesNumber = $"({Number}[mM])";
    private const string HoursNumber = $"({Number}[hH])";

    public static readonly List<LexerRule> Standard =
    [
        ..SyntaxFacts.OperatorMap.Select(pair => pair.Key.Length == 1
            ? SingleCharacter(pair.Value, pair.Key[0])
            : MultiCharacter(pair.Value, pair.Key)
        ),
        ..SyntaxFacts.KeywordMap.Select(pair => MultiCharacter(pair.Value, pair.Key)),
        RegEx(NumberLiteral, $"{HzNumber}|{MsNumber}|{SecondsNumber}|{MinutesNumber}|{HoursNumber}|{Number}"),
        RegEx(StringLiteral, "\"([^\"\\\\]|\\\\.)*\"|'([^'\\\\]|\\\\.)*'"),
        RegEx(Identifier, "[a-zA-Z_]([a-zA-Z0-9_]*)"),
        RegEx(Whitespace, @"\s+"),
        RegEx(MultilineComment, @"#:[\s\S]*?:#"),
        RegEx(Comment, @"##[^\n]*"),
    ];

    /// <summary>
    /// Checked BEFORE standard rules. These must take priority because the
    /// standard Int pattern would otherwise greedily consume the numeric prefix
    /// (e.g. the '0' in '0x'), preventing the error rule from ever firing.
    /// </summary>
    public static readonly IReadOnlyList<LexerDiagnosticRule> PriorityDiagnostic =
    [
        DiagnosticRule(
            "0[xX](?![a-fA-F0-9_])",
            InternalCodes.MalformedNumber,
            m => $"Malformed hexadecimal literal '{m}': expected at least one hex digit after '0x'."
        ),
        DiagnosticRule(
            "0[bB](?![01_])",
            InternalCodes.MalformedNumber,
            m => $"Malformed binary literal '{m}': expected at least one binary digit after '0b'."
        ),
        DiagnosticRule(
            "0[oO](?![0-7_])",
            InternalCodes.MalformedNumber,
            m => $"Malformed octal literal '{m}': expected at least one octal digit after '0o'."
        ),
        DiagnosticRule(
            @"\d[\d_]*(?:\.\d[\d_]*)?[eE](?:-(?![\d_])|(?!-?[\d_]))",
            InternalCodes.MalformedNumber,
            m => $"Malformed scientific notation '{m}': expected one or more digits after the exponent."
        ),
        DiagnosticRule(
            @"\d[\d_]*\.(?![\d_.])",
            InternalCodes.MalformedNumber,
            m => $"Malformed float literal '{m}': expected one or more digits after the decimal point."
        ),
    ];

    /// <summary>
    /// Checked AFTER standard rules fail. Safe as fallbacks because their
    /// opening characters ('\"', '\'', '#') only reach here when no valid
    /// token matched.
    /// </summary>
    public static readonly IReadOnlyList<LexerDiagnosticRule> Diagnostic =
    [
        DiagnosticRule(
            "\"",
            InternalCodes.UnterminatedString,
            _ => "Unterminated string literal: expected closing '\"'."
        ),
        DiagnosticRule(
            "'",
            InternalCodes.UnterminatedString,
            _ => "Unterminated string literal: expected closing \"'\"."
        ),
        DiagnosticRule(
            @"#:[\s\S]*",
            InternalCodes.UnterminatedComment,
            _ => "Unterminated block comment: expected closing ':#'."
        ),
    ];

    private static LexerDiagnosticRule DiagnosticRule(string pattern, string code, Func<string, string> message) =>
        new(new Regex(pattern, RegexOptions.Compiled), code, message);
}