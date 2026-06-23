using Loom.Text;

namespace Loom.Parsing.AST;

public class Block(Token leftBrace, Token rightBrace, List<Statement> statements)
    : Statement([leftBrace, ..statements.SelectMany(s => s.Tokens), rightBrace], statements)
{
    public Token LeftBrace { get; } = leftBrace;
    public Token RightBrace { get; } = rightBrace;
    public List<Statement> Statements { get; } = statements;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitBlock(this);
}