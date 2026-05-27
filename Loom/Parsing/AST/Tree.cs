namespace Loom.Parsing.AST;

public class Tree
    : ASTNode
{
    public Tree(IEnumerable<ASTNode> statements)
        : base(statements)
    {
        Statements = Children;
    }

    public IEnumerable<ASTNode> Statements { get; }
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitTree(this);
    public override string ToString() => string.Join('\n', Statements.Select(s => s.ToString()));
}