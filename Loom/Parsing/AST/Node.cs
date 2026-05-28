using Loom.Syntax;

namespace Loom.Parsing.AST;

public readonly record struct NodeId(int Value)
{
    public override string ToString() => $"#{Value}";
}

public abstract class Node
{
    private static int _nextId;
    
    public NodeId Id { get; } = new(Interlocked.Increment(ref _nextId));
    public List<Node> Children { get; }
    public List<Token> Tokens { get; }
    public LocationSpan Span { get; }
    public Node Parent { get; protected set; } = null!;

    protected Node(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children, LocationSpan? span = null)
    {
        Children = children.Where(node => node != null).Cast<Node>().ToList();
        Tokens = theseTokens.Where(token => token != null).Cast<Token>().ToList();
        Span = span ?? new LocationSpan(Tokens.First().Span.Start, Tokens.Last().Span.End);
        foreach (var child in Children)
            child.Parent = this;
    }
    
    public abstract T Accept<T>(IVisitor<T> visitor);

    public override string ToString() => Span.GetText();
}