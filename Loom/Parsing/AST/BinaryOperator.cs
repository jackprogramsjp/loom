using Loom.Syntax;

namespace Loom.Parsing.AST;

public class BinaryOperator(Token @operator, Expression left, Expression right) : Expression
{
    public Token Operator { get; } = @operator;
    public Expression Left { get; } = left;
    public Expression Right { get; } = right;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitBinaryOperator(this);
}