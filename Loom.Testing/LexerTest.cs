using Loom.Diagnostics;
using Loom.Syntax;

namespace Loom.Testing;

public class LexerTest
{
    public static readonly List<object[]> Operators = new List<(string, SyntaxKind)>([
        ("+", SyntaxKind.Plus),
        ("+=", SyntaxKind.PlusEquals),
        ("-", SyntaxKind.Minus),
        ("-=", SyntaxKind.MinusEquals),
        ("*", SyntaxKind.Star),
        ("*=", SyntaxKind.StarEquals),
        ("/", SyntaxKind.Slash),
        ("/=", SyntaxKind.SlashEquals),
        ("//", SyntaxKind.SlashSlash),
        ("//=", SyntaxKind.SlashSlashEquals),
        ("^", SyntaxKind.Carat),
        ("^=", SyntaxKind.CaratEquals),
        ("%", SyntaxKind.Percent),
        ("%=", SyntaxKind.PercentEquals),
        ("&", SyntaxKind.Ampersand),
        ("&=", SyntaxKind.AmpersandEquals),
        ("|", SyntaxKind.Pipe),
        ("|=", SyntaxKind.PipeEquals),
        ("~", SyntaxKind.Tilde),
        ("~=", SyntaxKind.TildeEquals),
        (">>", SyntaxKind.RArrowRArrow),
        (">>=", SyntaxKind.RArrowRArrowEquals),
        (">>>", SyntaxKind.RArrowRArrowRArrow),
        (">>>=", SyntaxKind.RArrowRArrowRArrowEquals),
        ("<<", SyntaxKind.LArrowLArrow),
        ("<<=", SyntaxKind.LArrowLArrowEquals),
        ("&&", SyntaxKind.AmpersandAmpersand),
        ("&&=", SyntaxKind.AmpersandAmpersandEquals),
        ("||", SyntaxKind.PipePipe),
        ("||=", SyntaxKind.PipePipeEquals),
        ("=", SyntaxKind.Equals),
        ("==", SyntaxKind.EqualsEquals),
        ("!", SyntaxKind.Bang),
        ("!=", SyntaxKind.BangEquals),
        (">", SyntaxKind.RArrow),
        (">=", SyntaxKind.RArrowEquals),
        ("<", SyntaxKind.LArrow),
        ("<=", SyntaxKind.LArrowEquals),
        ("??", SyntaxKind.QuestionQuestion),
        ("??=", SyntaxKind.QuestionQuestionEquals),
        ("(", SyntaxKind.LParen),
        (")", SyntaxKind.RParen),
        ("[", SyntaxKind.LBracket),
        ("]", SyntaxKind.RBracket),
        ("{", SyntaxKind.LBrace),
        ("}", SyntaxKind.RBrace)
    ]).ConvertAll<object[]>(t => [t.Item1, t.Item2]);
    
    [Theory]
    [InlineData("@")]
    [InlineData("$")]
    [InlineData("\\")]
    [InlineData("`")]
    [InlineData("123.")]
    [InlineData("'abc\"")]
    [InlineData("\"abc'")]
    public void ThrowsFor_UnexpectedCharacters(string source)
    {
        var diagnostics = Utility.GetLexerDiagnostics(source);
        var diagnostic = diagnostics.Find(d => d.Code == InternalCodes.UnexpectedCharacter);
        Assert.NotNull(diagnostic);
        Assert.Equal("Unexpected character.", diagnostic.Message);
    }
    
    [Fact]
    public void Tokenizes_MultipleOperators()
    {
        var lexemes = Operators.ConvertAll(a => a[0]).Cast<string>();
        var expectedSyntaxes = Operators.ConvertAll(a => a[1]).Cast<SyntaxKind>().ToList();
        var source = string.Join(' ', lexemes);
        var tokens = Utility.GetTokens(source);
        Assert.Equal(expectedSyntaxes.Count, tokens.Count);

        for (var i = 0; i < tokens.Count; ++i)
        {
            var actual = tokens[i];
            var expected = expectedSyntaxes[i];
            Assert.Equal(expected, actual.Kind);
        }
    }
    
    [Theory]
    [MemberData(nameof(Operators))]
    public void Tokenizes_Operators(string source, SyntaxKind expected)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(expected, token.Kind);
    }
    
    [Theory]
    [InlineData("420.69")]
    [InlineData(".420")]
    [InlineData("0.234")]
    public void Tokenizes_Floats(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(SyntaxKind.FloatLiteral, token.Kind);
    }
    
    [Theory]
    [InlineData("69")]
    [InlineData("123456")]
    public void Tokenizes_Integers(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(SyntaxKind.IntegerLiteral, token.Kind);
    }
    
    [Theory]
    [InlineData("\"abcd\"")]
    [InlineData("'abc'")]
    public void Tokenizes_Strings(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(SyntaxKind.StringLiteral, token.Kind);
    }
    
    [Fact]
    public void Tokenizes_True()
    {
        var tokens = Utility.GetTokens("true");
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(SyntaxKind.TrueLiteral, token.Kind);
    }
    
    [Fact]
    public void Tokenizes_False()
    {
        var tokens = Utility.GetTokens("false");
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(SyntaxKind.FalseLiteral, token.Kind);
    }
    
    [Fact]
    public void Tokenizes_None()
    {
        var tokens = Utility.GetTokens("none");
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(SyntaxKind.NoneLiteral, token.Kind);
    }
    
    [Theory]
    [InlineData("_abc_")]
    [InlineData("abc123")]
    [InlineData("abc_123")]
    [InlineData("AbC_123_")]
    public void Tokenizes_Identifiers(string source)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(SyntaxKind.Identifier, token.Kind);
    }
    
    [Theory]
    [InlineData("let", SyntaxKind.LetKeyword)]
    [InlineData("mut", SyntaxKind.MutKeyword)]
    [InlineData("fn", SyntaxKind.FnKeyword)]
    public void Tokenizes_Keywords(string source, SyntaxKind expected)
    {
        var tokens = Utility.GetTokens(source);
        Assert.Single(tokens);
        
        var token = tokens.First();
        Assert.Equal(expected, token.Kind);
    }
}