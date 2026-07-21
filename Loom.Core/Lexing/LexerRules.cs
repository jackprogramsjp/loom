using System.Text.RegularExpressions;
using Loom.Core.Diagnostics;
using Loom.Core.Text;

namespace Loom.Core.Lexing;

using static LexerRule;
using static SyntaxKind;

public static class LexerRules
{
    private static readonly (LexerRule Rule, IReadOnlyList<char> LeadChars)[] _regexRuleSpecs =
    [
        (RegEx(StringLiteral, "\"([^\"\\\\]|\\\\.)*\"|'([^'\\\\]|\\\\.)*'"), ['"', '\'']),
        (RegEx(BlockComment, @"#:[\s\S]*?:#"), ['#']),
        (RegEx(Comment, @"##[^\n]*"), ['#'])
    ];

    private static readonly IReadOnlyList<LexerRule> _standardRules =
    [
        ..SyntaxFacts.OperatorMap.Select(pair => pair.Key.Length == 1
            ? SingleCharacter(pair.Value, pair.Key[0])
            : MultiCharacter(pair.Value, pair.Key)
        ),
        .._regexRuleSpecs.Select(s => s.Rule)
    ];

    public static readonly IReadOnlyDictionary<char, (LexerRule Rule, Regex CompiledRegex)[]> RegexRulesByFirstCharacter = _regexRuleSpecs
        .SelectMany(spec => spec.LeadChars.Select(c => (Char: c, spec.Rule)))
        .GroupBy(x => x.Char)
        .ToDictionary(
            g => g.Key,
            g => g.Select(x => (x.Rule, new Regex($@"\G(?:{x.Rule.Pattern})", RegexOptions.Compiled))).ToArray()
        );

    /// <summary>
    /// Literal (non-regex) rules from <see cref="_standardRules"/>, bucketed by first
    /// character and sorted longest-pattern-first within each bucket, so the
    /// lexer can look up only the handful of candidates that could possibly
    /// match at a given position instead of scanning every literal rule.
    /// </summary>
    public static readonly IReadOnlyDictionary<char, LexerRule[]> LiteralRulesByFirstCharacter = _standardRules
        .Where(r => r.Kind is LexerRuleKind.SingleCharacter or LexerRuleKind.MultiCharacter)
        .GroupBy(r => r.Pattern[0])
        .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.Pattern.Length).ToArray());
    
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
        )
    ];

    private static LexerDiagnosticRule DiagnosticRule(string pattern, string code, Func<string, string> message) =>
        new(new Regex($@"\G(?:{pattern})", RegexOptions.Compiled), code, message);
}