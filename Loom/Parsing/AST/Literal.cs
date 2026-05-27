using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Literal(Token token) : Expression([])
{
    public Token Token { get; } = token;

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteral(this);

    public override string ToString() => Token.Text;
}