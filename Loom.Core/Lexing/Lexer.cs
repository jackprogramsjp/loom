using System.Text.RegularExpressions;
using Loom.Core.Diagnostics;
using Loom.Core.Text;

namespace Loom.Core.Lexing;

public sealed class Lexer(SourceFile file)
{
    private static readonly IReadOnlyList<(LexerRule Rule, Regex? CompiledRegex)> _compiledRules =
        LexerRules.Standard
            .Select(r => (
                r,
                r.Kind == LexerRuleKind.RegEx
                    ? new Regex($@"\G(?:{r.Pattern})", RegexOptions.Compiled)
                    : null
            ))
            .ToList();
    
    private readonly DiagnosticBag _diagnostics = new();
    private int _character, _position;
    private int _line = 1;

    public LexerResult Tokenize()
    {
        var tokens = GetTokens().ToList();
        return new LexerResult(file, tokens.FindAll(t => SyntaxFacts.IsNotTrivia(t.Kind)), tokens, _diagnostics);
    }

    private IEnumerable<Token> GetTokens()
    {
        var sourceLength = file.SourceText.Length;
        while (_position < sourceLength)
        {
            var start = GetLocation();
            if (TryDiagnosticRule(start, LexerRules.PriorityDiagnostic)) continue;

            var rule = Lex();
            if (rule == null)
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
            if (rule.Syntax == SyntaxKind.Whitespace)
            {
                var text = span.GetText();
                var tabCount = text.Count(c => c == '\t');
                _character += tabCount * 3;
            }
            
            yield return new Token(rule.Syntax, span);
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

    private LexerRule? Lex()
    {
        var matches = _compiledRules
            .Select(entry => (entry.Rule, Match: GetMatch(entry.Rule, entry.CompiledRegex)))
            .Where(pair => pair.Match.HasValue && pair.Match.Value.Index == _position)
            .Select(pair => (pair.Rule, pair.Match!.Value))
            .OrderByDescending(pair => pair.Value.Content.Length * (int)pair.Rule.Kind)
            .ToList();

        if (matches.Count == 0 || matches.All(pair => pair.Value.Content.Length == 0))
            return null;

        var (rule, (content, _)) = matches.First();
        Advance(content);
        return rule;
    }
    
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

    private (string Content, int Index)? GetMatch(LexerRule rule, Regex? compiledRegex)
    {
        if (rule.Kind == LexerRuleKind.RegEx && compiledRegex != null)
        {
            var match = compiledRegex.Match(file.SourceText, _position);
            return match.Success ? (match.Value, match.Index) : null;
        }

        var isMatch = rule.Kind switch
        {
            LexerRuleKind.SingleCharacter =>
                !IsEof() && Current().ToString() == rule.Pattern,

            LexerRuleKind.MultiCharacter =>
                !IsEof(rule.Pattern.Length - 1) && PeekNext(rule.Pattern.Length) == rule.Pattern,

            _ => false
        };

        return isMatch ? (rule.Pattern, _position) : null;
    }

    private string PeekNext(int characters) => file.SourceText[_position..(_position + characters)];
    private char Current() => file.SourceText[_position];
    private bool IsEof(int offset = 0) => _position + offset >= file.SourceText.Length;
    private LocationSpan GetSpan(Location start) => new(start, GetLocation());
    private Location GetLocation() => new(file, _character, _line, _position);
}