using Loom.Syntax;

namespace Loom.Lexing;

public static class LexerRules
{
    public static readonly List<LexerRule> Standard =
    [
        
        LexerRule.SingleCharacter(SyntaxKind.Plus, '+'),
        LexerRule.MultiCharacter(SyntaxKind.PlusEquals, "+="),
        LexerRule.SingleCharacter(SyntaxKind.Minus, '-'),
        LexerRule.MultiCharacter(SyntaxKind.MinusEquals, "-="),
        LexerRule.SingleCharacter(SyntaxKind.Star, '*'),
        LexerRule.MultiCharacter(SyntaxKind.StarEquals, "*="),
        LexerRule.SingleCharacter(SyntaxKind.Slash, '/'),
        LexerRule.MultiCharacter(SyntaxKind.SlashEquals, "/="),
        LexerRule.SingleCharacter(SyntaxKind.Percent, '%'),
        LexerRule.MultiCharacter(SyntaxKind.PercentEquals, "%="),
        LexerRule.SingleCharacter(SyntaxKind.Carat, '^'),
        LexerRule.MultiCharacter(SyntaxKind.CaratEquals, "^="),
        LexerRule.SingleCharacter(SyntaxKind.Ampersand, '&'),
        LexerRule.MultiCharacter(SyntaxKind.AmpersandEquals, "&="),
        LexerRule.MultiCharacter(SyntaxKind.AmpersandAmpersand, "&&"),
        LexerRule.MultiCharacter(SyntaxKind.AmpersandAmpersandEquals, "&&="),
        LexerRule.SingleCharacter(SyntaxKind.Pipe, '|'),
        LexerRule.MultiCharacter(SyntaxKind.PipeEquals, "|="),
        LexerRule.MultiCharacter(SyntaxKind.PipePipe, "||"),
        LexerRule.MultiCharacter(SyntaxKind.PipePipeEquals, "||="),
        LexerRule.SingleCharacter(SyntaxKind.Colon, ':'),
        LexerRule.SingleCharacter(SyntaxKind.Bang, '!'),
        LexerRule.MultiCharacter(SyntaxKind.BangEquals, "!="),
        LexerRule.SingleCharacter(SyntaxKind.Equals, '='),
        LexerRule.MultiCharacter(SyntaxKind.EqualsEquals, "=="),
        LexerRule.SingleCharacter(SyntaxKind.LArrow, '<'),
        LexerRule.MultiCharacter(SyntaxKind.LArrowLArrow, "<<"),
        LexerRule.MultiCharacter(SyntaxKind.LArrowLArrowEquals, "<<="),
        LexerRule.MultiCharacter(SyntaxKind.LArrowEquals, "<="),
        LexerRule.SingleCharacter(SyntaxKind.RArrow, '>'),
        LexerRule.MultiCharacter(SyntaxKind.RArrowRArrow, ">>"),
        LexerRule.MultiCharacter(SyntaxKind.RArrowRArrowEquals, ">>="),
        LexerRule.MultiCharacter(SyntaxKind.RArrowRArrowRArrow, ">>>"),
        LexerRule.MultiCharacter(SyntaxKind.RArrowRArrowRArrowEquals, ">>>="),
        LexerRule.MultiCharacter(SyntaxKind.RArrowEquals, ">="),
        LexerRule.SingleCharacter(SyntaxKind.LParen, '('),
        LexerRule.SingleCharacter(SyntaxKind.RParen, ')'),
        LexerRule.SingleCharacter(SyntaxKind.LBracket, '['),
        LexerRule.SingleCharacter(SyntaxKind.RBracket, ']'),
        LexerRule.SingleCharacter(SyntaxKind.LBrace, '{'),
        LexerRule.SingleCharacter(SyntaxKind.RBrace, '}'),
        
        LexerRule.RegEx(SyntaxKind.IntegerLiteral, @"\d+"),
        LexerRule.RegEx(SyntaxKind.FloatLiteral, @"(\d+\.\d+|\.\d+|\d+\.\d+)"),
        LexerRule.RegEx(SyntaxKind.StringLiteral, "\".*\"|'.*'"),
        LexerRule.MultiCharacter(SyntaxKind.TrueLiteral, "true"),
        LexerRule.MultiCharacter(SyntaxKind.FalseLiteral, "false"),
        LexerRule.MultiCharacter(SyntaxKind.NoneLiteral, "none"),
        
        LexerRule.MultiCharacter(SyntaxKind.LetKeyword, "let"),
        LexerRule.MultiCharacter(SyntaxKind.MutKeyword, "mut"),
        LexerRule.MultiCharacter(SyntaxKind.FnKeyword, "fn"),
        
        LexerRule.RegEx(SyntaxKind.Identifier, "[a-zA-Z_]([a-zA-Z0-9_]+)"),
        
        LexerRule.SingleCharacter(SyntaxKind.Semicolon, ';'),
        LexerRule.RegEx(SyntaxKind.Whitespace, @"\s+")
    ];
}
