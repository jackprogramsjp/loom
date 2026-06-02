using Loom.Syntax;

namespace Loom.Parsing.AST;

public readonly record struct NodeId(int Value)
{
    public static readonly Dictionary<NodeId, Node> Map = [];

    public override string ToString() => $"#{Value}";
}

public abstract class Node
{
    private static int _nextId;

    protected Node(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children, LocationSpan? span = null)
    {
        Id = new NodeId(Interlocked.Increment(ref _nextId));
        NodeId.Map.Add(Id, this);

        Children = FilterChildren(children);
        Tokens = SortTokens(theseTokens);
        Span = span ?? DeriveSpan();
        SetChildrenParents();
    }

    private LocationSpan DeriveSpan() =>
        Tokens.Count == 0
            ? LocationSpan.Empty(SourceFile.Empty)
            : new LocationSpan(Tokens.First().Span.Start, Tokens.Last().Span.End);

    private static List<Node> FilterChildren(IEnumerable<Node?> children) => children.Where(node => node != null).Cast<Node>().ToList();

    public NodeId Id { get; }
    public List<Node> Children { get; private set; }
    public List<Token> Tokens { get; private set; }
    public LocationSpan Span { get; private set; }
    public Node Parent { get; protected set; } = null!;

    public abstract T Accept<T>(Visitor<T> visitor);
    public override string ToString() => Span.GetText();

    protected void SetChildren(IEnumerable<Node?> children)
    {
        Children = FilterChildren(children);
        SetChildrenParents();
    }

    protected void SetTokens(IEnumerable<Token?> tokens)
    {
        Tokens = SortTokens(tokens);
        Span = DeriveSpan();
    }

    private void SetChildrenParents()
    {
        foreach (var child in Children)
            child.Parent = this;
    }

    private static List<Token> SortTokens(IEnumerable<Token?> tokens) =>
        tokens.Where(token => token != null).Cast<Token>().OrderBy(token => token.Span.Start.Position).ToList();
}