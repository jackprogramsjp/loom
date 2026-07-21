using System.Diagnostics.CodeAnalysis;
using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public abstract class Node
{
    private static int _nextId;

    public NodeId Id { get; }
    public IReadOnlyList<Node> Children { get; }
    public IReadOnlyList<Token> Tokens { get; }
    public LocationSpan Span { get; }
    public SourceFile File => field ??= Span.File;
    [MaybeNull] public Node Parent { get; private set; }

    protected Node(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children, LocationSpan? span = null)
    {
        Id = new NodeId(Interlocked.Increment(ref _nextId));
        NodeId.Map.Add(Id, this);

        Children = children.OfType<Node>().ToArray();
        Tokens = theseTokens.OfType<Token>().ToArray();
        Span = span ?? DeriveSpan();
        foreach (var child in Children)
            child.Parent = this;
    }

    public abstract T Accept<T>(Visitor<T> visitor);
    public override string ToString() => Span.GetText().ToString();
    public IReadOnlyList<T> GetDescendants<T>() where T : Node => GetDescendants().OfType<T>().ToArray();
    public IReadOnlyList<Node> GetDescendants() => [..Children, ..Children.SelectMany(c => c.GetDescendants())];
    public bool IsDescendantOf<T>() where T : Node => FirstAncestorOfType<T>() != null;

    public T? FirstAncestorOfType<T>()
        where T : Node
    {
        for (var node = Parent; node != null; node = node.Parent)
        {
            if (node is T typed)
                return typed;
        }

        return null;
    }

    private LocationSpan DeriveSpan() =>
        Tokens.Count == 0
            ? LocationSpan.Empty(SourceFile.Empty)
            : new LocationSpan(Tokens[0].GetLocation().Start, Tokens[^1].GetLocation().End);
}