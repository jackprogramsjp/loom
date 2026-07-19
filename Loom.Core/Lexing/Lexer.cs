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
    private int _position;

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
        while (!IsEof())
        {
            var start = GetLocation();
            var current = Current();
            if (char.IsLetter(current) || current == '_')
            {
                _position++;
                while (!IsEof() && (char.IsLetterOrDigit(Current()) || Current() == '_'))
                    _position++;

                var span = GetSpan(start);
                var kind = SyntaxFacts.KeywordMap.GetValueOrDefault(span.GetText().ToString(), SyntaxKind.Identifier);
                yield return new Token(kind, span);
                continue;
            }
            
            if (char.IsWhiteSpace(current))
            {
                while (!IsEof() && char.IsWhiteSpace(Current()))
                    _position++;
                
                yield return new Token(SyntaxKind.Whitespace, GetSpan(start));
                continue;
            }
            
            if (TryDiagnosticRule(start, LexerRules.PriorityDiagnostic)) continue;
            if (LexWithRule() is not { } rule)
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

            yield return new Token(rule.Syntax, GetSpan(start));
        }

        yield return new Token(SyntaxKind.Eof, GetSpan(GetLocation()));
    }

    private bool TryDiagnosticRule(Location start, IReadOnlyList<LexerDiagnosticRule> rules)
    {
        foreach (var rule in rules)
        {
            var match = rule.Pattern.Match(file.SourceText, _position);
            if (!match.Success || match.Index != _position) continue;

            Advance(match.Length);
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

    private LexerRule? LexWithRule()
    {
        LexerRule? bestRule = null;
        var advanceLength = 0;
        var bestScore = -1;

        if (LexerRules.LiteralRulesByFirstCharacter.TryGetValue(Current(), out var candidates))
        {
            foreach (var rule in candidates)
            {
                if (GetLiteralMatch(rule) is not { } content) continue;

                var score = content.Length * (int)rule.Kind;
                if (score <= bestScore) continue;

                bestScore = score;
                bestRule = rule;
                advanceLength = content.Length;
            }
        }

        foreach (var (rule, compiledRegex) in _compiledRegexRules)
        {
            var match = compiledRegex.Match(file.SourceText, _position);
            if (!match.Success || match.Index != _position || match.Length == 0) continue;

            var score = match.Length * (int)rule.Kind;
            if (score <= bestScore) continue;

            bestScore = score;
            bestRule = rule;
            advanceLength = match.Length;
        }

        if (bestRule == null)
            return null;

        Advance(advanceLength);
        return bestRule;
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
    
    private void Advance(int offset) => _position += offset;
    private bool MatchesPatternAt(string pattern) => file.SourceText.AsSpan(_position, pattern.Length).SequenceEqual(pattern);
    private char Current() => file.SourceText[_position];
    private bool IsEof(int offset = 0) => _position + offset >= file.SourceText.Length;
    private LocationSpan GetSpan(Location start) => new(start, GetLocation());
    private Location GetLocation() => new(file, _position);
}