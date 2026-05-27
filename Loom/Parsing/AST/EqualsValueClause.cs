using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class EqualsValueClause(Token equalsToken, Expression value) : ASTNode([value])
{
    public Token EqualsToken { get; } = equalsToken;
    public Expression Value { get; } = value;
    
    public override string ToString() => $" {EqualsToken.Text} {Value}";
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitEqualsValueClause(this);
}