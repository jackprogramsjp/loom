using Loom.Diagnostics.Debug;

namespace Loom.Syntax;

public class Token : IExaminable
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

    public string Examine()
    {
        return $"Token {{ kind: {Kind}, span: {Span.Examine()}, text: \"{Text}\" }}";
    }
}