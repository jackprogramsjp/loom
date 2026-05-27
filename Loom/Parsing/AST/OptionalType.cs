using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class OptionalType(Token question, TypeExpression requiredType) : TypeExpression([requiredType])
{
    public Token Question { get; } = question;
    public TypeExpression RequiredType { get; } = requiredType;

    public override string ToString() => RequiredType + Question.Text;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitOptionalType(this);
}