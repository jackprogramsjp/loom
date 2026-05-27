using Loom.Syntax;

namespace Loom.Lexing;

public static class LexerRules
{
    public static readonly List<LexerRule> Standard =
    [
        LexerRule.SingleCharacter(SyntaxKind.Plus, '+'),
        LexerRule.MultiCharacter(SyntaxKind.PlusEquals, "+="),
        LexerRule.SingleCharacter(SyntaxKind.Minus, '-'),
        LexerRule.SingleCharacter(SyntaxKind.Star, '*'),
        LexerRule.SingleCharacter(SyntaxKind.Slash, '/'),
        LexerRule.SingleCharacter(SyntaxKind.Percent, '%'),
        LexerRule.SingleCharacter(SyntaxKind.Carat, '^'),
        LexerRule.SingleCharacter(SyntaxKind.Ampersand, '&'),
        LexerRule.SingleCharacter(SyntaxKind.Pipe, '|'),
        LexerRule.SingleCharacter(SyntaxKind.Colon, ':'),
        LexerRule.SingleCharacter(SyntaxKind.Bang, '!'),
        LexerRule.SingleCharacter(SyntaxKind.Equals, '='),
        LexerRule.SingleCharacter(SyntaxKind.LArrow, '<'),
        LexerRule.SingleCharacter(SyntaxKind.RArrow, '>'),
        LexerRule.SingleCharacter(SyntaxKind.LParen, '('),
        LexerRule.SingleCharacter(SyntaxKind.RParen, ')'),
        LexerRule.SingleCharacter(SyntaxKind.LBracket, '['),
        LexerRule.SingleCharacter(SyntaxKind.LBracket, ']'),
        LexerRule.SingleCharacter(SyntaxKind.LBrace, '{'),
        LexerRule.SingleCharacter(SyntaxKind.LBrace, '}'),
        
        LexerRule.SingleCharacter(SyntaxKind.Semicolon, ';'),
        LexerRule.RegEx(SyntaxKind.IntegerLiteral, @"\d+"),
        LexerRule.RegEx(SyntaxKind.Whitespace, @"\s+")
    ];
}