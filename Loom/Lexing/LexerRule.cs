using Loom.Syntax;

namespace Loom.Lexing;

public class LexerRule(LexerRuleKind kind, SyntaxKind syntax, string pattern)
{
    public LexerRuleKind Kind { get; } = kind;
    public SyntaxKind Syntax { get; } = syntax;
    public string Pattern { get; } = pattern;

    public static LexerRule SingleCharacter(SyntaxKind syntax, char character) => new(LexerRuleKind.SingleCharacter, syntax, character.ToString());
    public static LexerRule MultiCharacter(SyntaxKind syntax, string pattern) => new(LexerRuleKind.MultiCharacter, syntax, pattern);
    public static LexerRule RegEx(SyntaxKind syntax, string pattern) => new(LexerRuleKind.RegEx, syntax, pattern);
}