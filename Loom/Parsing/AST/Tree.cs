namespace Loom.Parsing.AST;

public class Tree(IEnumerable<ASTNode> children)
    : ASTNode(children)
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitTree(this);
}