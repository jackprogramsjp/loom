using Loom.Core.Text;

namespace Loom.Core.Lexing;

public sealed record LexerRule(LexerRuleKind Kind, SyntaxKind Syntax, string Pattern)
{
    public static LexerRule SingleCharacter(SyntaxKind syntax, char character) => new(LexerRuleKind.SingleCharacter, syntax, character.ToString());
    public static LexerRule MultiCharacter(SyntaxKind syntax, string pattern) => new(LexerRuleKind.MultiCharacter, syntax, pattern);
    public static LexerRule RegEx(SyntaxKind syntax, string pattern) => new(LexerRuleKind.RegEx, syntax, pattern);
}