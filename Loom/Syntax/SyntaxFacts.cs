namespace Loom.Syntax;

internal static class SyntaxFacts
{
    private static readonly HashSet<SyntaxKind> _triviaSyntaxes = [SyntaxKind.Whitespace, SyntaxKind.Semicolon, SyntaxKind.Comment];
    private static readonly HashSet<SyntaxKind> _assignmentOperators = [
        SyntaxKind.Equals,
        SyntaxKind.PlusEquals, 
        SyntaxKind.MinusEquals, 
        SyntaxKind.StarEquals, 
        SyntaxKind.SlashEquals, 
        SyntaxKind.SlashSlashEquals, 
        SyntaxKind.PercentEquals, 
        SyntaxKind.CaratEquals, 
        SyntaxKind.AmpersandEquals, 
        SyntaxKind.PipeEquals, 
        SyntaxKind.TildeEquals, 
        SyntaxKind.LArrowLArrowEquals, 
        SyntaxKind.RArrowRArrowEquals, 
        SyntaxKind.RArrowRArrowRArrowEquals,
        SyntaxKind.AmpersandAmpersandEquals,
        SyntaxKind.PipePipeEquals
    ];
    private static readonly HashSet<SyntaxKind> _unaryOperators = [
        SyntaxKind.Minus, 
        SyntaxKind.Tilde, 
        SyntaxKind.Bang
    ];

    public static bool IsTrivia(SyntaxKind kind) => _triviaSyntaxes.Contains(kind);
    public static bool IsAssignmentOperator(SyntaxKind kind) => _assignmentOperators.Contains(kind);
    public static bool IsUnaryOperator(SyntaxKind kind) => _unaryOperators.Contains(kind);
}