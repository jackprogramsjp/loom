using Loom.Syntax;

namespace Loom.Lexing;

using static LexerRule;
using static SyntaxKind;

public static class LexerRules
{
    private const string Int = @"\d[\d_]*\d*_*";
    private const string FloatScientific = $"({Float}[eE]{Int})";
    private const string Float = $@"({Int}\.{Int}|\.{Int}|{Int}\.{Int})";
    private const string IntScientific = $"({Int}[eE]{Int})";
    private const string HexInt = $"(0[xX][a-fA-F0-9_]+)";
    private const string BinaryInt = "(0[bB][01_]+)";
    private const string OctalInt = "(0[oO][0-7_]+)";
    private const string Number = $"({HexInt}|{BinaryInt}|{OctalInt}|{FloatScientific}|{IntScientific}|{Float}|{Int})";
    private const string HzNumber = $"({Number}[hH][zZ])";
    private const string MsNumber = $"({Number}[mM][sS])";
    private const string SecondsNumber = $"({Number}[sS])";
    private const string MinutesNumber = $"({Number}[mM])";
    private const string HoursNumber = $"({Number}[hH])";

    public static readonly List<LexerRule> Standard =
    [
        ..SyntaxFacts.OperatorMap.Select(
            pair => pair.Key.Length == 1
                ? SingleCharacter(pair.Value, pair.Key[0])
                : MultiCharacter(pair.Value, pair.Key)
        ),
        ..SyntaxFacts.KeywordMap.Select(pair => MultiCharacter(pair.Value, pair.Key)),
        RegEx(NumberLiteral, $"{HzNumber}|{MsNumber}|{SecondsNumber}|{MinutesNumber}|{HoursNumber}|{Number}"),
        RegEx(StringLiteral, "\"([^\"]*)\"|'([^']*)'"),
        RegEx(Identifier, "[a-zA-Z_]([a-zA-Z0-9_]*)"),
        RegEx(Whitespace, @"\s+")
    ];
}