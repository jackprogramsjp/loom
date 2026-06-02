using Loom.Syntax;

namespace Loom.Parsing.AST;

public class BinaryOperator : Expression
{
    public void Setup()
    {
        SetTokens([..Left.Tokens, Operator, ..Right.Tokens]);
        SetChildren([Left, Right]);
    }

    public Token Operator { get; init; } = null!;
    public Expression Left { get; init; } = null!;
    public Expression Right { get; init; } = null!;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitBinaryOperator(this);
}