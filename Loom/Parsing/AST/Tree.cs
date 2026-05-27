using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Tree
    : ASTNode
{
    public Tree(List<ASTNode> statements)
        : base([], statements, new LocationSpan(statements.First().Span.Start, statements.Last().Span.End))
    {
        Statements = Children;
    }

    public List<ASTNode> Statements { get; }
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitTree(this);
}