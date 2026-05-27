using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

// TODO: track tokens, create spans
public abstract class ASTNode
{
    public List<ASTNode> Children { get; }
    public List<Token> Tokens { get; }
    public LocationSpan Span { get; }
    public ASTNode Parent { get; protected set; } = null!;

    protected ASTNode(IEnumerable<Token?> theseTokens, IEnumerable<ASTNode?> children, LocationSpan? span = null)
    {
        Children = children.Where(node => node != null).Cast<ASTNode>().ToList();
        Tokens = theseTokens.Where(token => token != null).Cast<Token>().ToList();
        Span = span ?? new LocationSpan(Tokens.First().Span.Start, Tokens.Last().Span.End);
        foreach (var child in Children)
            child.Parent = this;
    }
    
    public abstract T Accept<T>(IVisitor<T> visitor);

    public override string ToString() => Span.GetText();
}