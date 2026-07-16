namespace Loom.Core.Text;

public sealed record Token
{
    public Token(SyntaxKind kind, LocationSpan span, string? text = null)
    {
        Kind = kind;
        Span = span;
        Text = text ?? Span.GetText();
    }

    public SyntaxKind Kind { get; }
    public LocationSpan Span { get; }
    public string Text { get; }
}