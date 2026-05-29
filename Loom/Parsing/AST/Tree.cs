using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Tree
    : Node
{
    public Tree(SourceFile file, List<Node> statements)
        : base([],
               statements,
               statements.Count == 0 ? LocationSpan.Empty(file) : new LocationSpan(statements.First().Span.Start, statements.Last().Span.End))
    {
        File = file;
        Statements = Children;
    }

    public SourceFile File { get; }
    public List<Node> Statements { get; }

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTree(this);
}