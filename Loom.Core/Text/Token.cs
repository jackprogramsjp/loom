namespace Loom.Core.Text;

public sealed class Token
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

    public override string ToString() => $"Token {{ kind: {Kind}, span: {Span}, text: \"{Text}\" }}";
}