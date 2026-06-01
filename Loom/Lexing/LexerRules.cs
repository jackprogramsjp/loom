using Loom.Syntax;

namespace Loom.Lexing;

using static LexerRule;
using static SyntaxKind;

public static class LexerRules
{
    private const string FloatScientific = @"((\d+\.\d+|\.\d+|\d+\.\d+)e\d+)";
    private const string Float = @"(\d+\.\d+|\.\d+|\d+\.\d+)";
    private const string IntScientific = @"(\d+e\d+)";
    private const string HexInt = "(0[xX][a-fA-F0-9]+)";
    private const string BinaryInt = "(0[bB][01]+)";
    private const string OctalInt = "(0[oO][0-7]+)";
        
    public static readonly List<LexerRule> Standard =
    [
        ..SyntaxFacts.OperatorMap.Select(
            pair => pair.Key.Length == 1
                ? SingleCharacter(pair.Value, pair.Key[0])
                : MultiCharacter(pair.Value, pair.Key)
        ),
        ..SyntaxFacts.KeywordMap.Select(pair => MultiCharacter(pair.Value, pair.Key)),
        RegEx(NumberLiteral, @$"{FloatScientific}|{IntScientific}|{Float}|{HexInt}|{BinaryInt}|{OctalInt}|\d+"),
        RegEx(StringLiteral, "\"([^\"]*)\"|'([^']*)'"),
        RegEx(Identifier, "[a-zA-Z_]([a-zA-Z0-9_]*)"),
        RegEx(Whitespace, @"\s+")
    ];
}