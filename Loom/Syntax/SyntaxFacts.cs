namespace Loom.Syntax;

public static class SyntaxFacts
{
    public static readonly HashSet<SyntaxKind> _triviaSyntaxes = [SyntaxKind.Whitespace, SyntaxKind.Comment];

    public static bool IsTrivia(SyntaxKind kind) => _triviaSyntaxes.Contains(kind);
}