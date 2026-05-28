using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Tree
    : Node
{
    public Tree(List<Node> statements)
        : base([], statements, new LocationSpan(statements.First().Span.Start, statements.Last().Span.End))
    {
        Statements = Children;
    }

    public List<Node> Statements { get; }
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitTree(this);
}