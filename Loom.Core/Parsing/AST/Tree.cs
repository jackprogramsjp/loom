using Loom.Core.Lexing;

namespace Loom.Core.Parsing.AST;

public class Tree(LexerResult lexerResult, List<Statement> statements)
    : Node(lexerResult.TokensWithTrivia, statements)
{
    public List<Statement> Statements { get; } = statements;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTree(this);
}