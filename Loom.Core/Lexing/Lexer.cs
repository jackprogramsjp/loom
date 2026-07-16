using System.Text.RegularExpressions;
using Loom.Core.Diagnostics;
using Loom.Core.Text;

namespace Loom.Core.Lexing;

public sealed class Lexer(SourceFile file)
{
    private static readonly IReadOnlyList<(LexerRule Rule, Regex CompiledRegex)> _compiledRegexRules =
        LexerRules.RegExRules
            .Select(r => (r, new Regex($@"\G(?:{r.Pattern})", RegexOptions.Compiled)))
            .ToArray();

    private readonly DiagnosticBag _diagnostics = new();
    private int _character, _position;
    private int _line = 1;

    public LexerResult Tokenize()
    {
        var allTokens = new List<Token>();
        var significantTokens = new List<Token>();
        foreach (var token in GetTokens())
        {
            allTokens.Add(token);
            
            if (SyntaxFacts.IsTrivia(token.Kind)) continue;
            significantTokens.Add(token);
        }

        return new LexerResult(file, significantTokens, allTokens, _diagnostics);
    }

    private IEnumerable<Token> GetTokens()
    {
        var sourceLength = file.SourceText.Length;
        while (_position < sourceLength)
        {
            var start = GetLocation();
            if (TryDiagnosticRule(start, LexerRules.PriorityDiagnostic)) continue;

            var lexResult = LexWithRule();
            if (lexResult is not { } result)
            {
                if (!TryDiagnosticRule(start, LexerRules.Diagnostic))
                {
                    var character = Current();
                    var display = char.IsControl(character) ? $"U+{(int)character:X4}" : $"'{character}'";
                    _diagnostics.Error(
                        new LocationSpan(start, start + 1),
                        InternalCodes.UnexpectedCharacter,
                        $"Unexpected character {display}."
                    );
                }

                break;
            }

            var span = GetSpan(start);
            if (result.Rule.Syntax == SyntaxKind.Whitespace)
            {
                var tabCount = 0;
                for (var i = start.Position; i < span.End.Position; i++)
                    if (file.SourceText[i] == '\t')
                        tabCount++;

                _character += tabCount * 2;
            }

            yield return new Token(result.Rule.Syntax, span, result.Content);
        }

        yield return new Token(SyntaxKind.Eof, GetSpan(GetLocation()));
    }

    private bool TryDiagnosticRule(Location start, IReadOnlyList<LexerDiagnosticRule> rules)
    {
        foreach (var rule in rules)
        {
            var match = rule.Pattern.Match(file.SourceText, _position);
            if (!match.Success || match.Index != _position) continue;

            Advance(match.Value);
            _diagnostics.Report(
                GetSpan(start),
                rule.Severity,
                rule.DiagnosticCode,
                rule.MessageFactory(match.Value),
                rule.Hint
            );

            return true;
        }

        return false;
    }

    private (LexerRule Rule, string Content)? LexWithRule()
    {
        LexerRule? bestRule = null;
        var bestContent = "";
        var bestScore = -1;

        if (!IsEof() && LexerRules.LiteralRulesByFirstCharacter.TryGetValue(Current(), out var candidates))
        {
            foreach (var rule in candidates)
            {
                if (GetLiteralMatch(rule) is not { } content) continue;

                var score = content.Length * (int)rule.Kind;
                if (score <= bestScore) continue;

                bestScore = score;
                bestRule = rule;
                bestContent = content;
            }
        }

        foreach (var (rule, compiledRegex) in _compiledRegexRules)
        {
            var match = compiledRegex.Match(file.SourceText, _position);
            if (!match.Success || match.Index != _position || match.Value.Length == 0) continue;

            var score = match.Value.Length * (int)rule.Kind;
            if (score <= bestScore) continue;

            bestScore = score;
            bestRule = rule;
            bestContent = match.Value;
        }

        if (bestRule == null)
            return null;

        Advance(bestContent);
        return (bestRule.Value, bestContent);
    }

    private string? GetLiteralMatch(LexerRule rule) =>
        rule.Kind switch
        {
            LexerRuleKind.SingleCharacter when rule.Pattern.Length == 1 && Current() == rule.Pattern[0] =>
                rule.Pattern,

            LexerRuleKind.MultiCharacter when !IsEof(rule.Pattern.Length - 1) && MatchesPatternAt(rule.Pattern) =>
                rule.Pattern,

            _ => null
        };

    /// <summary>
    /// Advances the position, line, and character counters by the length of
    /// <paramref name="content"/>, accounting for embedded newlines.
    /// </summary>
    private void Advance(string content)
    {
        var lines = content.Count(c => c == '\n');
        var length = content.Length;

        _position += length;
        if (lines > 0)
        {
            _line += lines;
            _character = length - content.LastIndexOf('\n') - 1;
        }
        else
        {
            _character += length;
        }
    }

    private bool MatchesPatternAt(string pattern) => file.SourceText.AsSpan(_position, pattern.Length).SequenceEqual(pattern);
    private char Current() => file.SourceText[_position];
    private bool IsEof(int offset = 0) => _position + offset >= file.SourceText.Length;
    private LocationSpan GetSpan(Location start) => new(start, GetLocation());
    private Location GetLocation() => new(file, _character, _line, _position);
}