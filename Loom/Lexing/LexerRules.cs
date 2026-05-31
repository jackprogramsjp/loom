using Loom.Syntax;

namespace Loom.Lexing;

using static LexerRule;
using static SyntaxKind;

public static class LexerRules
{
    public static readonly List<LexerRule> Standard =
    [
        ..SyntaxFacts.OperatorMap.Select(
            pair => pair.Key.Length == 1
                ? SingleCharacter(pair.Value, pair.Key[0])
                : MultiCharacter(pair.Value, pair.Key)
        ),
        ..SyntaxFacts.KeywordMap.Select(pair => MultiCharacter(pair.Value, pair.Key)),
        RegEx(IntegerLiteral, @"\d+"),
        RegEx(FloatLiteral, @"(\d+\.\d+|\.\d+|\d+\.\d+)"),
        RegEx(StringLiteral, "\"([^\"]*)\"|'([^']*)'"),
        RegEx(Identifier, "[a-zA-Z_]([a-zA-Z0-9_]*)"),
        RegEx(Whitespace, @"\s+")
    ];
}