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

        Children = SortChildren(children);
        Tokens = SortTokens(theseTokens);
        Span = span ?? DeriveSpan();
        foreach (var child in Children)
            child.Parent = this;
    }

    public NodeId Id { get; }
    public List<Node> Children { get; }
    public List<Token> Tokens { get; }
    public LocationSpan Span { get; }
    public Node Parent { get; private set; } = null!;

    public abstract T Accept<T>(Visitor<T> visitor);
    public override string ToString() => Span.GetText();
    public List<Node> GetDescendants() => Children.SelectMany(c => c.Children).ToList();

    private static List<Node> SortChildren(IEnumerable<Node?> children) => children.Where(node => node != null).Cast<Node>().OrderBy(node => node.Span.Start.Position).ToList();
    private static List<Token> SortTokens(IEnumerable<Token?> tokens) =>
        tokens.Where(token => token != null).Cast<Token>().OrderBy(token => token.Span.Start.Position).ToList();
    
    private LocationSpan DeriveSpan() =>
        Tokens.Count == 0
            ? LocationSpan.Empty(SourceFile.Empty)
            : new LocationSpan(Tokens.First().Span.Start, Tokens.Last().Span.End);
}