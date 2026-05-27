using System.Text.RegularExpressions;
using Loom.Diagnostics;
using Loom.Syntax;

namespace Loom.Lexing;

public class Lexer : Diagnosable
{
    private readonly SourceFile _file;
    private int _line = 1;
    private int _character, _position;

    public Lexer(SourceFile file)
    {
        _file = file;
        LexerRules.Standard.Sort((a, b) => b.Kind - a.Kind);
    }

    public LexerResult Tokenize()
    {
        var tokens = GetTokens();
        return new LexerResult(tokens.ToList(), Diagnostics);
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
                Diagnostics.Error(span, InternalCodes.UnexpectedCharacter, "Unexpected character."); // TODO: more detailed lexer errors
                break;
            }

            if (SyntaxFacts.IsTrivia(rule.Syntax)) continue;

            Diagnostics.Info(span, $"Lexed token {rule.Syntax}");
            yield return new Token(rule.Syntax, span);
        }
    }

    private LexerRule? Lex()
    {    
        string? bestMatch = null;
        LexerRule? bestRule = null;
        foreach (var rule in LexerRules.Standard)
        {
            var match = GetMatch(rule);
            if (match == null) continue;

            var (content, position) = match.Value;
            if (position != _position) continue;
            if (bestMatch != null && content.Length <= bestMatch.Length) continue;
            bestMatch = content;
            bestRule = rule;
        }

        if (bestMatch == null)
            return null;

        var lines = bestMatch.Split('\n').Length - 1;
        var length = bestMatch.Length;
        _position += length;
        if (lines > 0)
        {
            _line += lines;
            _character = length - bestMatch.LastIndexOf('\n') - 1;
        }
        else
        {
            _character += length;
        }

        return bestRule;
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
            LexerRuleKind.MultiCharacter => !IsEof(rule.Pattern.Length) && PeekNext(rule.Pattern.Length) == rule.Pattern,
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