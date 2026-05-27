using System.Text.RegularExpressions;
using Loom.Syntax;

namespace Loom;

public class Lexer
{
    private readonly SourceFile _file;
    private int _character = 0;
    private int _line = 1;
    private int _position = 0;
    
    private readonly Regex _regex;
    private readonly List<(SyntaxKind Kind, string Pattern)> _patterns = [
        (SyntaxKind.Whitespace,   @"\s+"),
        
        (SyntaxKind.Plus,   @"\+"),
        (SyntaxKind.Minus,   "-"),
        (SyntaxKind.Star,   @"\*"),
        (SyntaxKind.Slash,   "/"),
        
        (SyntaxKind.FloatLiteral,     @"\d+(\.\d+)?"),
    ];

    public Lexer(SourceFile file)
    {
        var patterns = string.Join("|", _patterns.Select(p => $"(?<{p.Kind}>{p.Pattern})"));
        _regex = new Regex(patterns, RegexOptions.Compiled);
        _file = file;
    }
    
    public IEnumerable<Token> Tokenize()
    {
        var sourceLength = _file.SourceText.Length;
        while (_position < sourceLength)
        {
            foreach (var token in Lex())
                yield return token;
        }
    }

    private IEnumerable<Token> Lex()
    {
        var match = _regex.Match(_file.SourceText, _position);
        if (!match.Success || match.Index != _position)
        {
            throw new Exception($"Unexpected character at line {_line}, col {_character}"); // TODO: real errors
        }

        var start = GetLocation();
        _position  = match.Index + match.Length;
        foreach (var (kind, _) in _patterns)
        {
            if (!match.Groups[kind.ToString()].Success) continue;
                
            var newLines = match.Value.Count(c => c == '\n');
            if (newLines > 0)
            {
                _line += newLines;
                _character = match.Value.Length - match.Value.LastIndexOf('\n');
            }
            else
            {
                _character += match.Value.Length;
            }
                
            if (!SyntaxFacts.IsTrivia(kind))
            {
                var end = GetLocation();
                var span = new LocationSpan(start, end);
                yield return new Token(kind, span);
            }
                
            break;
        }
    }

    private Location GetLocation() => new(_file, _character, _line, _position);
}