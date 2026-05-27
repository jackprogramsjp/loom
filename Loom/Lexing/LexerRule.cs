using Loom.Syntax;

namespace Loom.Lexing;

public class LexerRule(LexerRuleKind kind, SyntaxKind syntax, string pattern, bool skip = false)
{
    public LexerRuleKind Kind { get; } = kind;
    public SyntaxKind Syntax { get; } = syntax;
    public string Pattern { get; } = pattern;
    public bool Skip { get; } = skip;

    public static LexerRule SingleCharacter(SyntaxKind syntax, char character, bool skip = false) => new(LexerRuleKind.SingleCharacter, syntax, character.ToString(), skip);
    public static LexerRule MultiCharacter(SyntaxKind syntax, string pattern, bool skip = false) => new(LexerRuleKind.MultiCharacter, syntax, pattern, skip);
    public static LexerRule RegEx(SyntaxKind syntax, string pattern, bool skip = false) => new(LexerRuleKind.RegEx, syntax, pattern, skip);
}