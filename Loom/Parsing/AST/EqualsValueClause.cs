using Loom.Syntax;

namespace Loom.Parsing.AST;

public class EqualsValueClause(Token equalsToken, Expression value) : Node([equalsToken, ..value.Tokens], [value])
{
    public Token EqualsToken { get; } = equalsToken;
    public Expression Value { get; } = value;
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitEqualsValueClause(this);
}