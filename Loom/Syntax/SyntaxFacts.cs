namespace Loom.Syntax;

internal static class SyntaxFacts
{
    private static readonly HashSet<SyntaxKind> _triviaSyntaxes = [SyntaxKind.Whitespace, SyntaxKind.Semicolon, SyntaxKind.Comment];

    public static bool IsTrivia(SyntaxKind kind) => _triviaSyntaxes.Contains(kind);
}