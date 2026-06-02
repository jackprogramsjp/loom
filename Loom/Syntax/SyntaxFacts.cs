namespace Loom.Syntax;

public static class SyntaxFacts
{
    public static readonly Dictionary<string, SyntaxKind> KeywordMap = new(
        [
            new("true", SyntaxKind.TrueLiteral),
            new("false", SyntaxKind.FalseLiteral),
            new("none", SyntaxKind.NoneLiteral),
            new("let", SyntaxKind.LetKeyword),
            new("mut", SyntaxKind.MutKeyword),
            new("type", SyntaxKind.TypeKeyword),
            new("interface", SyntaxKind.InterfaceKeyword),
            new("fn", SyntaxKind.FnKeyword),
            new("event", SyntaxKind.EventKeyword),
            new("enum", SyntaxKind.EnumKeyword),
            new("every", SyntaxKind.EveryKeyword),
            new("after", SyntaxKind.AfterKeyword),
            new("if", SyntaxKind.IfKeyword),
            new("else", SyntaxKind.ElseKeyword),
            new("while", SyntaxKind.WhileKeyword),
            new("match", SyntaxKind.MatchKeyword),
            new("nameof", SyntaxKind.NameofKeyword),
        ]
    );
    public static readonly Dictionary<string, SyntaxKind> OperatorMap = new(
        [
            new("+", SyntaxKind.Plus),
            new("+=", SyntaxKind.PlusEquals),
            new("-", SyntaxKind.Minus),
            new("-=", SyntaxKind.MinusEquals),
            new("*", SyntaxKind.Star),
            new("*=", SyntaxKind.StarEquals),
            new("/", SyntaxKind.Slash),
            new("/=", SyntaxKind.SlashEquals),
            new("//", SyntaxKind.SlashSlash),
            new("//=", SyntaxKind.SlashSlashEquals),
            new("^", SyntaxKind.Caret),
            new("^=", SyntaxKind.CaretEquals),
            new("%", SyntaxKind.Percent),
            new("%=", SyntaxKind.PercentEquals),
            new("&", SyntaxKind.Ampersand),
            new("&=", SyntaxKind.AmpersandEquals),
            new("|", SyntaxKind.Pipe),
            new("|=", SyntaxKind.PipeEquals),
            new("~", SyntaxKind.Tilde),
            new("~=", SyntaxKind.TildeEquals),
            new(">>", SyntaxKind.RArrowRArrow),
            new(">>=", SyntaxKind.RArrowRArrowEquals),
            new(">>>", SyntaxKind.RArrowRArrowRArrow),
            new(">>>=", SyntaxKind.RArrowRArrowRArrowEquals),
            new("<<", SyntaxKind.LArrowLArrow),
            new("<<=", SyntaxKind.LArrowLArrowEquals),
            new("&&", SyntaxKind.AmpersandAmpersand),
            new("&&=", SyntaxKind.AmpersandAmpersandEquals),
            new("||", SyntaxKind.PipePipe),
            new("||=", SyntaxKind.PipePipeEquals),
            new("=", SyntaxKind.Equals),
            new("==", SyntaxKind.EqualsEquals),
            new("!", SyntaxKind.Bang),
            new("!=", SyntaxKind.BangEquals),
            new(">", SyntaxKind.RArrow),
            new(">=", SyntaxKind.RArrowEquals),
            new("<", SyntaxKind.LArrow),
            new("<=", SyntaxKind.LArrowEquals),
            new("?", SyntaxKind.Question),
            new("??", SyntaxKind.QuestionQuestion),
            new("??=", SyntaxKind.QuestionQuestionEquals),
            new("(", SyntaxKind.LParen),
            new(")", SyntaxKind.RParen),
            new("[", SyntaxKind.LBracket),
            new("]", SyntaxKind.RBracket),
            new("{", SyntaxKind.LBrace),
            new("}", SyntaxKind.RBrace),
            new(":", SyntaxKind.Colon),
            new(",", SyntaxKind.Comma),
            new(".", SyntaxKind.Dot),
            new("..", SyntaxKind.DotDot),
            new(";", SyntaxKind.Semicolon)
        ]
    );

    private static readonly HashSet<string> _primitiveTypeNames =
    [
        "number",
        "string",
        "bool",
        "never",
        "unknown",
        "none",
        "void"
    ];
    private static readonly HashSet<SyntaxKind> _triviaSyntaxes = [SyntaxKind.Whitespace, SyntaxKind.Semicolon, SyntaxKind.Comment];
    private static readonly HashSet<SyntaxKind> _literalSyntaxes =
    [
        SyntaxKind.NumberLiteral, SyntaxKind.StringLiteral, SyntaxKind.TrueLiteral, SyntaxKind.FalseLiteral, SyntaxKind.NoneLiteral
    ];
    private static readonly HashSet<SyntaxKind> _assignmentOperators =
    [
        SyntaxKind.Equals,
        SyntaxKind.PlusEquals,
        SyntaxKind.MinusEquals,
        SyntaxKind.StarEquals,
        SyntaxKind.SlashEquals,
        SyntaxKind.SlashSlashEquals,
        SyntaxKind.PercentEquals,
        SyntaxKind.CaretEquals,
        SyntaxKind.AmpersandEquals,
        SyntaxKind.PipeEquals,
        SyntaxKind.TildeEquals,
        SyntaxKind.LArrowLArrowEquals,
        SyntaxKind.RArrowRArrowEquals,
        SyntaxKind.RArrowRArrowRArrowEquals,
        SyntaxKind.AmpersandAmpersandEquals,
        SyntaxKind.PipePipeEquals,
        SyntaxKind.QuestionQuestionEquals
    ];
    private static readonly HashSet<SyntaxKind> _bitwiseOperators =
    [
        SyntaxKind.Ampersand,
        SyntaxKind.AmpersandEquals,
        SyntaxKind.Pipe,
        SyntaxKind.PipeEquals,
        SyntaxKind.Tilde,
        SyntaxKind.TildeEquals,
        SyntaxKind.RArrowRArrow,
        SyntaxKind.RArrowRArrowEquals,
        SyntaxKind.RArrowRArrowRArrow,
        SyntaxKind.RArrowRArrowRArrowEquals,
        SyntaxKind.LArrowLArrow,
        SyntaxKind.LArrowLArrowEquals
    ];
    private static readonly HashSet<SyntaxKind> _unaryOperators = [SyntaxKind.Minus, SyntaxKind.Tilde, SyntaxKind.Bang];

    public static bool IsNotTrivia(SyntaxKind kind) => !IsTrivia(kind);
    public static bool IsTrivia(SyntaxKind kind) => _triviaSyntaxes.Contains(kind);
    public static bool IsLiteral(SyntaxKind kind) => _literalSyntaxes.Contains(kind);
    public static SyntaxKind? GetOperatorSyntax(string op) => OperatorMap.TryGetValue(op, out var syntax) ? syntax : null;
    public static string? GetOperatorText(SyntaxKind syntax) => OperatorMap.Keys.ElementAtOrDefault(OperatorMap.Values.ToList().IndexOf(syntax));
    public static SyntaxKind? GetKeywordSyntax(string text) => KeywordMap.TryGetValue(text, out var syntax) ? syntax : null;
    public static string? GetKeywordText(SyntaxKind syntax) => KeywordMap.Keys.ElementAtOrDefault(KeywordMap.Values.ToList().IndexOf(syntax));
    public static SyntaxKind? GetSyntax(string text) => GetKeywordSyntax(text) ?? GetOperatorSyntax(text);
    public static string? GetText(SyntaxKind syntax) => GetKeywordText(syntax) ?? GetOperatorText(syntax);

    public static bool IsAssignmentOperator(SyntaxKind kind) => _assignmentOperators.Contains(kind);
    public static bool IsBitwiseOperator(SyntaxKind kind) => _bitwiseOperators.Contains(kind);
    public static bool IsUnaryOperator(SyntaxKind kind) => _unaryOperators.Contains(kind);
    public static bool IsPrimitiveType(string name) => _primitiveTypeNames.Contains(name);
}