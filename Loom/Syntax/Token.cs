namespace Loom.Syntax;

public class Token
{
    public SyntaxKind Kind { get; }
    public LocationSpan Span { get; }
    public string Text { get; }
    
    public Token(SyntaxKind kind, LocationSpan span)
    {
        Kind = kind;
        Span = span;
        Text = Span.GetText();
    }

    public override string ToString() => $"Token {{ kind: {Kind}, span: {Span}, text: \"{Text}\" }}";
}