using System.Diagnostics.CodeAnalysis;
using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public abstract class Node
{
    private static int _nextId;

    public NodeId Id { get; }
    public List<Node> Children { get; }
    public List<Token> Tokens { get; }
    public LocationSpan Span { get; }
    public SourceFile File { get; }
    [MaybeNull] public Node Parent { get; private set; } = null!;

    protected Node(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children, LocationSpan? span = null)
    {
        Id = new NodeId(Interlocked.Increment(ref _nextId));

        Children = SortChildren(children);
        Tokens = SortTokens(theseTokens);
        Span = span ?? DeriveSpan();
        File = Span.File;
        foreach (var child in Children)
            child.Parent = this;
    }

    public abstract T Accept<T>(Visitor<T> visitor);
    public override string ToString() => Span.GetText().ToString();

    public List<T> GetDescendants<T>()
        where T : Node =>
        GetDescendants().OfType<T>().ToList();

    public List<Node> GetDescendants() => Children.SelectMany(c => c.GetDescendants()).Concat(Children).ToList();

    public bool IsDescendantOf<T>()
        where T : Node =>
        FirstAncestorOfType<T>() != null;

    public T? FirstAncestorOfType<T>()
        where T : Node
    {
        if (this is Tree)
            return null;

        if (Parent is T node)
            return node;

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        return Parent?.FirstAncestorOfType<T>();
    }

    private static List<Node> SortChildren(IEnumerable<Node?> children)
    {
        var result = new List<Node>();
        foreach (var child in children)
            if (child != null)
                result.Add(child);

        result.Sort(static (a, b) => a.Span.Start.Position.CompareTo(b.Span.Start.Position));
        return result;
    }

    private static List<Token> SortTokens(IEnumerable<Token?> tokens)
    {
        var result = new List<Token>();
        foreach (var token in tokens)
            if (token != null)
                result.Add(token);

        result.Sort(static (a, b) => a.Span.Position.CompareTo(b.Span.Position));
        return result;
    }

    private LocationSpan DeriveSpan() =>
        Tokens.Count == 0
            ? LocationSpan.Empty(SourceFile.Empty)
            : new LocationSpan(Tokens.First().GetLocation().Start, Tokens.Last().GetLocation().End);
}