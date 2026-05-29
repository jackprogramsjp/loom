using System.Text.RegularExpressions;
using Loom.Diagnostics;
using Loom.Syntax;

namespace Loom.Lexing;

public class Lexer
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly SourceFile _file;
    private int _character, _position;
    private int _line = 1;

    public Lexer(SourceFile file)
    {
        _file = file;
        LexerRules.Standard.Sort((a, b) => b.Kind  - a.Kind);
    }

    public LexerResult Tokenize()
    {
        var tokens = GetTokens();
        return new LexerResult(tokens.ToList(), _diagnostics);
    }

    private IEnumerable<Token> GetTokens()
    {
        var sourceLength = _file.SourceText.Length;
        while (_position < sourceLength)
        {
            var start = GetLocation();
            var rule = Lex();
            var span = GetSpan(start);
            if (rule == null)
            {
                _diagnostics.Error(span, InternalCodes.UnexpectedCharacter, "Unexpected character."); // TODO: more detailed lexer errors
                break;
            }

            if (SyntaxFacts.IsTrivia(rule.Syntax)) continue;

            yield return new Token(rule.Syntax, span);
        }
    }

    private LexerRule? Lex()
    {
        var matches = LexerRules.Standard
                                .ConvertAll(r => (r, GetMatch(r)))
                                .FindAll(pair => pair.Item2.HasValue && pair.Item2.Value.Item2 == _position) // silly
                                .ConvertAll(pair => (pair.Item1, pair.Item2!.Value))
                                .OrderByDescending(pair => pair.Item2.Item1.Length)
                                .ToList();

        if (matches.Count == 0)
            return null;

        foreach (var (_, (c, _)) in matches)
        {
            Console.WriteLine(c);
        }

        var (rule, (content, _)) = matches.First();
        var lines = content.Split('\n').Length - 1;
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

        return rule;
    }

    private (string, int)? GetMatch(LexerRule rule)
    {
        if (rule.Kind == LexerRuleKind.RegEx)
        {
            var regEx = new Regex(rule.Pattern, RegexOptions.Compiled);
            var match = regEx.Match(_file.SourceText, _position);
            return match.Success ? (match.Value, match.Index) : null;
        }

        var isMatch = rule.Kind switch
        {
            LexerRuleKind.SingleCharacter => !IsEof() && Current().ToString() == rule.Pattern,
            LexerRuleKind.MultiCharacter => !IsEof(rule.Pattern.Length - 1) && PeekNext(rule.Pattern.Length) == rule.Pattern,
            _ => false
        };

        return isMatch ? (rule.Pattern, _position) : null;
    }

    private string PeekNext(int characters) => _file.SourceText[_position..(_position + characters)];
    private char Current() => _file.SourceText[_position];
    private bool IsEof(int offset = 0) => _position + offset >= _file.SourceText.Length;
    private LocationSpan GetSpan(Location start) => new(start, GetLocation());
    private Location GetLocation() => new(_file, _character, _line, _position);
}