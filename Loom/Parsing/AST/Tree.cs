using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Tree(SourceFile file, List<Statement> statements)
    : Node(
        [],
        statements,
        statements.Count == 0 ? LocationSpan.Empty(file) : new LocationSpan(statements.First().Span.Start, statements.Last().Span.End)
    )
{
    public SourceFile File { get; } = file;
    public List<Statement> Statements { get; } = statements;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTree(this);
}