namespace Loom.Lexing;

public enum LexerRuleKind : byte
{
    SingleCharacter = 0xF,
    RegEx,
    MultiCharacter,
}