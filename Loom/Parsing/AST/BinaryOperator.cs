using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class BinaryOperator(Token @operator, Expression left, Expression right) : Expression([..left.Tokens, @operator, ..right.Tokens], [left, right])
{
    public Token Operator { get; } = @operator;
    public Expression Left { get; } = left;
    public Expression Right { get; } = right;

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryOperator(this);
}