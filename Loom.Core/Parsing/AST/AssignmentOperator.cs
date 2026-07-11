using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class AssignmentOperator(Token @operator, AssignmentTarget left, Expression right)
    : BinaryOperator(@operator, left, right)
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitAssignmentOperator(this);
}