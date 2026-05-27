namespace Loom.Parsing.AST;

public abstract class ASTNode
{
    public List<ASTNode> Children { get; }
    public ASTNode Parent { get; protected set; } = null!;

    protected ASTNode(IEnumerable<ASTNode> children)
    {
        Children = children.ToList();
        foreach (var child in Children)
            child.Parent = this;
    }
    
    public abstract T Accept<T>(IVisitor<T> visitor);
}