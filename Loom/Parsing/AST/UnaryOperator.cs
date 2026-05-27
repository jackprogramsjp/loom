using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class UnaryOperator(Token @operator, Expression operand) : Expression([operand])
{
    public Token Operator { get; } = @operator;
    public Expression Operand { get; } = operand;

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryOperator(this);
    public override string ToString() => Operator.Text + Operand;
}