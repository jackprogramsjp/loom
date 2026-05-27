using Loom.Parsing.AST.Traversal;

namespace Loom.Parsing.AST;

public abstract class ASTNode
{
    public List<ASTNode> Children { get; }
    public ASTNode Parent { get; protected set; } = null!;

    protected ASTNode(IEnumerable<ASTNode?> children)
    {
        Children = children.Where(node => node != null).Cast<ASTNode>().ToList();
        foreach (var child in Children)
            child.Parent = this;
    }
    
    public abstract T Accept<T>(IVisitor<T> visitor);
}