namespace Loom.Parsing.AST;

public class AssignmentOperator : BinaryOperator
{
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitAssignmentOperator(this);
}