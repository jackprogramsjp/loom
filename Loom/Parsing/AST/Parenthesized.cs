using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Parenthesized(Token leftParen, Token rightParen, Expression expression) : Expression([expression])
{
    public Token LeftParen { get; } = leftParen;
    public Token RightParen { get; } = rightParen;
    public Expression Expression { get; } = expression;
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitParenthesized(this);
    public override string ToString() => LeftParen.Text + Expression + RightParen.Text;
}