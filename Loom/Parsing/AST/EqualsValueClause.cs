using Loom.Syntax;

namespace Loom.Parsing.AST;

public class EqualsValueClause(Token equalsToken, Expression value)
{
    public Token EqualsToken { get; } = equalsToken;
    public Expression Value { get; } = value;
    
    public override string ToString() => $" {EqualsToken.Text} {Value}";
}