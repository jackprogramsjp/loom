using Loom.Core.Lexing;
using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class Tree(LexerResult lexerResult, List<Statement> statements)
    : Node(
        lexerResult.TokensWithTrivia,
        statements,
        statements.Count == 0 ? LocationSpan.Empty(lexerResult.File) : new LocationSpan(statements.First().Span.Start, statements.Last().Span.End)
    )
{
    public List<Statement> Statements { get; } = statements;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTree(this);
}