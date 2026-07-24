using System.Diagnostics.CodeAnalysis;
using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public abstract class Node
{
    private static int _nextId;

    public NodeId Id { get; }
    public IReadOnlyList<Node> Children { get; }
    public IReadOnlyList<Token> Tokens { get; }
    public TextSpan Span { get; }
    public LocationSpan LocationSpan => new(new Location(File, Span.Position), new Location(File, Span.End));
    public SourceFile File => field ??= Tokens.Count == 0 ? SourceFile.Empty : Tokens[0].File;
    [MaybeNull] public Node Parent { get; private set; }

    protected Node(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    {
        Id = new NodeId(Interlocked.Increment(ref _nextId));

        Children = children.OfType<Node>().OrderBy(n => n.Span.Position).ToArray();
        Tokens = theseTokens.OfType<Token>().OrderBy(t => t.Span.Position).ToArray();
        Span = DeriveSpan();
        foreach (var child in Children)
            child.Parent = this;
    }

    public abstract T Accept<T>(Visitor<T> visitor);
    public override string ToString() => LocationSpan.GetText().ToString();
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

    private TextSpan DeriveSpan() =>
        Tokens.Count == 0
            ? TextSpan.Empty
            : TextSpan.FromStartEnd(Tokens[0].Span.Position, Tokens[^1].Span.End);
}