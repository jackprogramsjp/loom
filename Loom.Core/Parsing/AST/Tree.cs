using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class Tree(SourceFile file, List<Statement> statements)
    : Node(
        statements.SelectMany(s => s.Tokens),
        statements,
        statements.Count == 0 ? LocationSpan.Empty(file) : new LocationSpan(statements.First().Span.Start, statements.Last().Span.End)
    )
{
    public List<Statement> Statements { get; } = statements;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTree(this);
}