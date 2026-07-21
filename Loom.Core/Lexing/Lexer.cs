using Loom.Core.Diagnostics;
using Loom.Core.Text;

namespace Loom.Core.Lexing;

public sealed class Lexer(SourceFile file)
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly int _sourceLength = file.SourceText.Length;
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
            var start = CurrentLocation;
            var current = Current();
            if (char.IsLetter(current) || current == '_')
            {
                yield return LexIdentifier(start);
                continue;
            }

            if (char.IsWhiteSpace(current))
            {
                yield return LexWhitespace(start);
                continue;
            }
            
            if (current is '"' or '\'')
            {
                yield return LexString(start, current);
                continue;
            }

            if (AtNumber())
            {
                yield return LexNumber(start);
                continue;
            }
            
            if (LexWithRule() is { } rule)
            {
                yield return CreateToken(rule.Syntax, GetSpan(start));
            }
            else
            {
                RecoverUnexpectedCharacterDiagnostic(start);
                break;
            }
        }

        yield return CreateToken(SyntaxKind.Eof, GetSpan(CurrentLocation));
    }

    private Token LexString(Location start, char terminator)
    {
        Advance();
        while (!IsEof() && Current() != terminator)
        {
            if (Current() == '\\' && !IsEof(1))
                Advance();
            Advance();
        }

        if (IsEof() || Current() == '\n')
        {
            var quotedQuote = terminator == '"' ? "'\"'" : "\"'\"";
            _diagnostics.Error(GetSpan(start), InternalCodes.UnterminatedString, $"Unterminated string literal: expected closing {quotedQuote}.");
        }
        else
        {
            Advance();
        }

        return CreateToken(SyntaxKind.StringLiteral, GetSpan(start));
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
                scoreDance(rule, content.Length);
            }
        }

        if (LexerRules.RegexRulesByFirstCharacter.TryGetValue(Current(), out var regexCandidates))
        {
            foreach (var (rule, compiledRegex) in regexCandidates)
            {
                var match = compiledRegex.Match(file.SourceText, _position);
                if (!match.Success || match.Index != _position || match.Length == 0) continue;
                scoreDance(rule, match.Length);
            }
        }

        if (bestRule == null)
            return null;

        Advance(advanceLength);
        return bestRule;

        void scoreDance(LexerRule rule, int length)
        {
            var score = length * (int)rule.Kind;
            if (score <= bestScore) return;

            bestScore = score;
            bestRule = rule;
            advanceLength = length;
        }
    }

    private Token LexWhitespace(Location start)
    {
        AdvanceWhile(char.IsWhiteSpace);
        return CreateToken(SyntaxKind.Whitespace, GetSpan(start));
    }

    private Token LexIdentifier(Location start)
    {
        Advance();
        AdvanceWhile(ch => char.IsLetterOrDigit(ch) || ch == '_');
        var span = GetSpan(start);
        var identifierText = span.GetText().ToString();
        var kind = SyntaxFacts.KeywordMap.GetValueOrDefault(identifierText, SyntaxKind.Identifier);
        return CreateToken(kind, span);
    }

    private Token LexNumber(Location start)
    {
        if (AtRadixLiteral())
            return LexRadixNumber(start);

        if (LexDecimal(start) is { } decimalError)
            return decimalError;

        if (TryLexExponent(start) is { } exponentError)
            return exponentError;

        LexUnitSuffix();
        return CreateToken(SyntaxKind.NumberLiteral, GetSpan(start));
    }

    private Token LexRadixNumber(Location start)
    {
        Advance();
        switch (char.ToLowerInvariant(Current()))
        {
            case 'x':
                Advance();
                if (!ReadHexDigits())
                    return ReportMalformedHex(start);

                break;

            case 'b':
                Advance();
                if (!ReadBinaryDigits())
                    return ReportMalformedBinary(start);

                break;

            case 'o':
                Advance();
                if (!ReadOctalDigits())
                    return ReportMalformedOctal(start);

                break;
        }

        LexUnitSuffix();
        return CreateToken(SyntaxKind.NumberLiteral, GetSpan(start));
    }

    private void LexUnitSuffix()
    {
        if (IsEof()) return;
        switch (char.ToLowerInvariant(Current()))
        {
            case 'h':
                Advance();
                Match('z');
                break;

            case 'm':
                Advance();
                Match('s');
                break;

            case 's':
                Advance();
                break;
        }
    }

    private Token? LexDecimal(Location start)
    {
        if (Current() == '.')
        {
            Advance();
            ReadDigits();
            return null;
        }

        ReadDigits();
        return LexFraction(start);
    }

    private Token? LexFraction(Location start)
    {
        if (IsEof() || Current() != '.')
            return null;
        
        if (!IsEof(1) && Peek(1) == '.')
            return null;

        Advance();
        if (!IsEof() && char.IsDigit(Current()))
        {
            ReadDigits();
            return null;
        }

        _diagnostics.Error(
            GetSpan(start),
            InternalCodes.MalformedNumber,
            $"Malformed float literal '{GetSpan(start).GetText()}': expected one or more digits after the decimal point."
        );

        return CreateToken(SyntaxKind.NumberLiteral, GetSpan(start));
    }

    private Token? TryLexExponent(Location start)
    {
        if (IsEof() || Current() is not ('e' or 'E'))
            return null;

        Advance();

        if (!IsEof() && Current() == '-')
            Advance();

        if (!IsEof() && char.IsDigit(Current()))
        {
            ReadDigits();
            return null;
        }

        _diagnostics.Error(
            GetSpan(start),
            InternalCodes.MalformedNumber,
            $"Malformed scientific notation '{GetSpan(start).GetText()}': expected one or more digits after the exponent."
        );

        return CreateToken(SyntaxKind.NumberLiteral, GetSpan(start));
    }

    private bool ReadHexDigits() => AdvanceWhile(static c => char.IsDigit(c) || (uint)(char.ToLowerInvariant(c) - 'a') <= 5 || c == '_');
    private bool ReadBinaryDigits() => AdvanceWhile(static c => c is '0' or '1' or '_');
    private bool ReadOctalDigits() => AdvanceWhile(static c => (uint)(c - '0') <= 7 || c == '_');
    private void ReadDigits() => AdvanceWhile(static c => char.IsDigit(c) || c == '_');
    private bool AtNumber() => char.IsDigit(Current()) || Current() == '.' && !IsEof(1) && char.IsDigit(Peek(1));
    private bool AtRadixLiteral() => Current() == '0' && !IsEof(1) && char.ToLowerInvariant(Peek(1)) is 'x' or 'b' or 'o';

    private Token ReportMalformedHex(Location start)
    {
        // Currently positioned after "0x"
        _diagnostics.Error(
            GetSpan(start),
            InternalCodes.MalformedNumber,
            $"Malformed hexadecimal literal '{GetSpan(start).GetText()}': expected at least one hex digit after '0x'."
        );

        return CreateToken(SyntaxKind.NumberLiteral, GetSpan(start));
    }

    private Token ReportMalformedBinary(Location start)
    {
        _diagnostics.Error(
            GetSpan(start),
            InternalCodes.MalformedNumber,
            $"Malformed binary literal '{GetSpan(start).GetText()}': expected at least one binary digit after '0b'."
        );

        return CreateToken(SyntaxKind.NumberLiteral, GetSpan(start));
    }

    private Token ReportMalformedOctal(Location start)
    {
        _diagnostics.Error(
            GetSpan(start),
            InternalCodes.MalformedNumber,
            $"Malformed octal literal '{GetSpan(start).GetText()}': expected at least one octal digit after '0o'."
        );

        return CreateToken(SyntaxKind.NumberLiteral, GetSpan(start));
    }

    private void RecoverUnexpectedCharacterDiagnostic(Location start)
    {
        if (TryDiagnosticRule(start, LexerRules.Diagnostic)) return;

        var character = Current();
        var display = char.IsControl(character) ? $"U+{(int)character:X4}" : $"'{character}'";
        _diagnostics.Error(GetSpan(start), InternalCodes.UnexpectedCharacter, $"Unexpected character {display}.");
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

    private string? GetLiteralMatch(LexerRule rule) =>
        rule.Kind switch
        {
            LexerRuleKind.SingleCharacter when rule.Pattern.Length == 1 && Current() == rule.Pattern[0] =>
                rule.Pattern,

            LexerRuleKind.MultiCharacter when !IsEof(rule.Pattern.Length - 1) && MatchesPatternAt(rule.Pattern) =>
                rule.Pattern,

            _ => null
        };

    private void Match(char c)
    {
        if (IsEof() || char.ToLowerInvariant(Current()) != c) return;
        Advance();
    }

    private bool AdvanceWhile(Func<char, bool> predicate)
    {
        var any = false;
        while (!IsEof() && predicate(Current()))
        {
            any = true;
            Advance();
        }

        return any;
    }

    private static Token CreateToken(SyntaxKind kind, LocationSpan span) => new(kind, span);
    private void Advance(int offset = 1) => _position += offset;
    private bool MatchesPatternAt(string pattern) => file.SourceText.AsSpan(_position, pattern.Length).SequenceEqual(pattern);
    private char Current() => Peek(0);
    private char Peek(int offset) => file.SourceText[_position + offset];
    private bool IsEof(int offset = 0) => _position + offset >= _sourceLength;
    private LocationSpan GetSpan(Location start) => new(start, CurrentLocation);
    private Location CurrentLocation => new(file, _position);
}