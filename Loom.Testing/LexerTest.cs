using Loom.Diagnostics;
using Loom.Text;

namespace Loom.Testing;

[Collection("Assembly")]
public class LexerTest
{
    public static readonly List<object[]> Operators = SyntaxFacts.OperatorMap.Where(pair => SyntaxFacts.IsNotTrivia(pair.Value))
        .Select(t => new object[] { t.Key, t.Value })
        .ToList();
    public static readonly List<object[]> Keywords = SyntaxFacts.KeywordMap.Select(t => new object[] { t.Key, t.Value }).ToList();

    [Theory]
    [InlineData("@")]
    [InlineData("$")]
    [InlineData("\\")]
    [InlineData("`")]
    public void ThrowsFor_UnexpectedCharacters(string source)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnexpectedCharacter, $"Unexpected character '{source}'.");
    }

    [Theory]
    [InlineData("0x", "0x")]
    [InlineData("0X", "0X")]
    [InlineData("0x.", "0x")]
    [InlineData("0x ", "0x")]
    [InlineData("0xG", "0x")]
    public void ThrowsFor_MalformedHexLiteral(string source, string matched)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.MalformedNumber,
            $"Malformed hexadecimal literal '{matched}': expected at least one hex digit after '0x'."
        );
    }

    [Theory]
    [InlineData("0b", "0b")]
    [InlineData("0B", "0B")]
    [InlineData("0b.", "0b")]
    [InlineData("0b2", "0b")]
    [InlineData("0b ", "0b")]
    public void ThrowsFor_MalformedBinaryLiteral(string source, string matched)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.MalformedNumber,
            $"Malformed binary literal '{matched}': expected at least one binary digit after '0b'."
        );
    }

    [Theory]
    [InlineData("0o", "0o")]
    [InlineData("0O", "0O")]
    [InlineData("0o.", "0o")]
    [InlineData("0o8", "0o")]
    [InlineData("0o ", "0o")]
    public void ThrowsFor_MalformedOctalLiteral(string source, string matched)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.MalformedNumber,
            $"Malformed octal literal '{matched}': expected at least one octal digit after '0o'."
        );
    }

    [Theory]
    [InlineData("1e", "1e")]
    [InlineData("1E", "1E")]
    [InlineData("1e+", "1e")]
    [InlineData("1e-", "1e")]
    [InlineData("1e ", "1e")]
    [InlineData("3.14e", "3.14e")]
    [InlineData("1_0e", "1_0e")]
    [InlineData("1_0.2_3e", "1_0.2_3e")]
    public void ThrowsFor_MalformedScientificNotation(string source, string matched)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.MalformedNumber,
            $"Malformed scientific notation '{matched}': expected one or more digits after the exponent."
        );
    }

    [Theory]
    [InlineData("1.", "1.")]
    [InlineData("42.", "42.")]
    [InlineData("1_0.", "1_0.")]
    [InlineData("1. ", "1.")]
    [InlineData("1.e5", "1.")]
    public void ThrowsFor_MalformedFloatLiteral(string source, string matched)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.MalformedNumber,
            $"Malformed float literal '{matched}': expected one or more digits after the decimal point."
        );
    }

    [Theory]
    [InlineData("'abc\"", true)]
    [InlineData("\"abc'")]
    [InlineData("\"abc")]
    [InlineData("'abc", true)]
    [InlineData("\"")]
    [InlineData("'", true)]
    public void ThrowsFor_UnterminatedString(string source, bool singleQuote = false)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        Utility.AssertDiagnostic(
            diagnostics,
            InternalCodes.UnterminatedString,
            $"Unterminated string literal: expected closing {(singleQuote ? "\"'\"" : "'\"'")}."
        );
    }

    [Fact]
    public void ThrowsFor_UnterminatedBlockComment()
    {
        var diagnostics = Utility.GetLexerDiagnostics("#: hello!");
        Utility.AssertDiagnostic(diagnostics, InternalCodes.UnterminatedComment, $"Unterminated block comment: expected closing ':#'.");
    }
    
    [Theory]
    [InlineData("s")]
    [InlineData("ms")]
    [InlineData("hz")]
    [InlineData("HZ")]
    public void Tokenizes_UnitSuffixesAsIdentifiers_WhenNotPrefixedByNumber(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(SyntaxKind.Identifier, tokens[0].Kind);
    }

    [Theory]
    [InlineData("## this is a comment")]
    [InlineData("##")]
    [InlineData("## 123 !@#$%")]
    public void Tokenizes_LineComments(string source)
    {
        var tokens = Utility.GetTokens(source, true);
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        Assert.Equal(SyntaxKind.Comment, token.Kind);
    }

    [Theory]
    [InlineData("#: this is a multiline comment :#")]
    [InlineData("#::#")]
    [InlineData("#: line one\nline two :#")]
    [InlineData("#: ## nested-looking content :#")]
    public void Tokenizes_BlockComments(string source)
    {
        var tokens = Utility.GetTokens(source, true);
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        Assert.Equal(SyntaxKind.MultilineComment, token.Kind);
    }

    [Fact]
    public void LineComment_DoesNotConsume_Newline()
    {
        var tokens = Utility.GetTokens("## comment\ntrue", true);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(SyntaxKind.Comment, tokens[0].Kind);
        Assert.Equal(SyntaxKind.Whitespace, tokens[1].Kind);
        Assert.Equal(SyntaxKind.TrueLiteral, tokens[2].Kind);
        Assert.Equal(SyntaxKind.Eof, tokens[3].Kind);

        Assert.Equal(1, tokens[0].Span.Start.Line);
        Assert.Equal(1, tokens[0].Span.End.Line);
        Assert.Equal(2, tokens[2].Span.Start.Line);
    }

    [Fact]
    public void BlockComment_Tracks_Lines()
    {
        var tokens = Utility.GetTokens("#: line one\nline two\nline three :#", true);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(SyntaxKind.MultilineComment, tokens[0].Kind);
        Assert.Equal(SyntaxKind.Eof, tokens[1].Kind);

        var comment = tokens[0];
        Assert.Equal(1, comment.Span.Start.Line);
        Assert.Equal(3, comment.Span.End.Line);
    }

    [Fact]
    public void LineComment_AfterCode()
    {
        var tokens = Utility.GetTokens("true ## comment", true);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(SyntaxKind.TrueLiteral, tokens[0].Kind);
        Assert.Equal(SyntaxKind.Whitespace, tokens[1].Kind);
        Assert.Equal(SyntaxKind.Comment, tokens[2].Kind);
        Assert.Equal(SyntaxKind.Eof, tokens[3].Kind);
    }

    [Fact]
    public void BlockComment_BetweenCode()
    {
        var tokens = Utility.GetTokens("true #: ignored :# false", true);
        Assert.Equal(6, tokens.Count);
        Assert.Equal(SyntaxKind.TrueLiteral, tokens[0].Kind);
        Assert.Equal(SyntaxKind.Whitespace, tokens[1].Kind);
        Assert.Equal(SyntaxKind.MultilineComment, tokens[2].Kind);
        Assert.Equal(SyntaxKind.Whitespace, tokens[3].Kind);
        Assert.Equal(SyntaxKind.FalseLiteral, tokens[4].Kind);
        Assert.Equal(SyntaxKind.Eof, tokens[5].Kind);
    }

    [Fact]
    public void BlockComment_DoesNotConsume_TrailingCode_AcrossLines()
    {
        var tokens = Utility.GetTokens("#: comment\nstill comment :# true", true);
        Assert.Equal(4, tokens.Count);
        Assert.Equal(SyntaxKind.MultilineComment, tokens[0].Kind);
        Assert.Equal(SyntaxKind.Whitespace, tokens[1].Kind);
        Assert.Equal(SyntaxKind.TrueLiteral, tokens[2].Kind);
        Assert.Equal(SyntaxKind.Eof, tokens[3].Kind);

        Assert.Equal(2, tokens[1].Span.Start.Line);
    }
    
    [Fact]
    public void Tokenize_WithTriviaFalse_ExcludesWhitespaceAndComments()
    {
        var tokens = Utility.GetTokens("true  ## comment\nfalse", withTrivia: false);
        Assert.Equal(3, tokens.Count);
        Assert.Equal(SyntaxKind.TrueLiteral, tokens[0].Kind);
        Assert.Equal(SyntaxKind.FalseLiteral, tokens[1].Kind);
        Assert.Equal(SyntaxKind.Eof, tokens[2].Kind);
    }

    [Fact]
    public void Tokenize_WithTriviaTrue_IncludesWhitespaceAndComments()
    {
        var tokens = Utility.GetTokens("true  ## comment\nfalse", withTrivia: true);
        Assert.Equal(6, tokens.Count);
        Assert.Equal(SyntaxKind.TrueLiteral, tokens[0].Kind);
        Assert.Equal(SyntaxKind.Whitespace, tokens[1].Kind);
        Assert.Equal(SyntaxKind.Comment, tokens[2].Kind);
        Assert.Equal(SyntaxKind.Whitespace, tokens[3].Kind);
        Assert.Equal(SyntaxKind.FalseLiteral, tokens[4].Kind);
        Assert.Equal(SyntaxKind.Eof, tokens[5].Kind);
    }

    [Fact]
    public void Tokenizes_MultipleOperators()
    {
        var lexemes = Operators.ConvertAll(a => a[0]).Cast<string>();
        var expectedSyntaxes = Operators.ConvertAll(a => a[1]).Cast<SyntaxKind>().ToList();
        var source = string.Join(' ', lexemes);
        var tokens = Utility.GetTokens(source);
        Assert.Equal(1 + expectedSyntaxes.Count, tokens.Count);

        for (var i = 0; i < tokens.Count; ++i)
        {
            var actual = tokens[i];
            var expected = expectedSyntaxes.Count > i ? expectedSyntaxes[i] : SyntaxKind.Eof;
            Assert.Equal(expected, actual.Kind);
        }
    }

    [Fact]
    public void Tokenizes_Eof()
    {
        var tokens = Utility.GetTokens("");
        Assert.Single(tokens);

        var token = tokens[0];
        Assert.Equal(SyntaxKind.Eof, token.Kind);
    }

    [Theory]
    [MemberData(nameof(Operators))]
    public void Tokenizes_Operators(string source, SyntaxKind expected)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        Assert.Equal(expected, token.Kind);
    }

    [Theory]
    [InlineData("69")]
    [InlineData("69_420")]
    [InlineData("123456")]
    [InlineData("1e5")]
    [InlineData("420_69.69_420")]
    [InlineData("420.69")]
    [InlineData(".420")]
    [InlineData("0.234")]
    [InlineData("1.24335e5")]
    [InlineData("1_2.24_3_35e1_1")]
    [InlineData("5s")]
    [InlineData("5ms")]
    [InlineData("5hz")]
    [InlineData("2_0.2_3Hz")]
    [InlineData("0.5s")]
    [InlineData("10m")]
    [InlineData("1M")]
    [InlineData("3H")]
    [InlineData("4h")]
    [InlineData("1_000")]
    [InlineData("1_000.5")]
    [InlineData("1.5e1_0")]
    [InlineData("1_0_0s")]
    [InlineData("1.5_0ms")]
    public void Tokenizes_Numbers(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        Assert.Equal(SyntaxKind.NumberLiteral, token.Kind);
    }

    [Theory]
    [InlineData("\"abcd\"")]
    [InlineData("'abc'")]
    public void Tokenizes_Strings(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        Assert.Equal(SyntaxKind.StringLiteral, token.Kind);
    }

    [Theory]
    [InlineData("_abc_")]
    [InlineData("abc123")]
    [InlineData("abc_123")]
    [InlineData("AbC_123_")]
    public void Tokenizes_Identifiers(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        Assert.Equal(SyntaxKind.Identifier, token.Kind);
    }

    [Theory]
    [MemberData(nameof(Keywords))]
    public void Tokenizes_Keywords(string source, SyntaxKind expected)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        Assert.Equal(expected, token.Kind);
    }
    
    [Fact]
    public void Tokenize_TracksLineAndColumnNumbers()
    {
        var tokens = Utility.GetTokens("abc\n123\nxyz", withTrivia: true);
        Assert.Equal(6, tokens.Count);

        var first = tokens[0];
        Assert.Equal(1, first.Span.Start.Line);
        Assert.Equal(0, first.Span.Start.Character);
        Assert.Equal(3, first.Span.End.Character);

        var firstWhitespace = tokens[1];
        Assert.Equal(1, firstWhitespace.Span.Start.Line);
        Assert.Equal(3, firstWhitespace.Span.Start.Character);
        Assert.Equal(2, firstWhitespace.Span.End.Line);
        Assert.Equal(0, firstWhitespace.Span.End.Character);

        var number = tokens[2];
        Assert.Equal(2, number.Span.Start.Line);
        Assert.Equal(0, number.Span.Start.Character);
        Assert.Equal(3, number.Span.End.Character);
    }

    [Fact]
    public void Tokenizes_ProperSpan_WithWhitespace()
    {
        var tokens = Utility.GetTokens("true false");
        Assert.Equal(3, tokens.Count);

        var first = tokens[0];
        var second = tokens[^2];
        var eof = tokens[^1];
        var firstStart = first.Span.Start;
        var firstEnd = first.Span.End;
        var secondStart = second.Span.Start;
        var secondEnd = second.Span.End;
        var eofStart = eof.Span.Start;
        var eofEnd = eof.Span.End;
        Assert.Equal(firstStart.Line, firstEnd.Line);
        Assert.Equal(firstStart.Character, firstStart.Position);
        Assert.Equal(firstEnd.Character, firstEnd.Position);
        Assert.Equal(0, firstStart.Position);
        Assert.Equal(4, firstEnd.Position);

        Assert.Equal(secondStart.Line, secondEnd.Line);
        Assert.Equal(secondStart.Character, secondStart.Position);
        Assert.Equal(secondEnd.Character, secondEnd.Position);
        Assert.Equal(5, secondStart.Position);
        Assert.Equal(10, secondEnd.Position);

        Assert.Equal(eofStart.Line, eofEnd.Line);
        Assert.Equal(eofStart.Character, eofStart.Position);
        Assert.Equal(eofEnd.Character, eofEnd.Position);
        Assert.Equal(10, eofStart.Position);
        Assert.Equal(10, eofEnd.Position);
    }

    [Fact]
    public void Tokenizes_ProperSpan()
    {
        var tokens = Utility.GetTokens("true");
        Assert.Equal(2, tokens.Count);

        var token = tokens[0];
        var start = token.Span.Start;
        var end = token.Span.End;
        Assert.Equal(start.Line, end.Line);
        Assert.Equal(start.Character, start.Position);
        Assert.Equal(end.Character, end.Position);
        Assert.Equal(0, start.Position);
        Assert.Equal(4, end.Position);
    }
}