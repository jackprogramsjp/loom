using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeArguments(Token leftArrow, Token rightArrow, List<TypeExpression> arguments)
    : Node([leftArrow, ..arguments.SelectMany(p => p.Tokens), rightArrow], arguments)
{
    public Token LeftArrow { get; } = leftArrow;
    public Token RightArrow { get; } = rightArrow;
    public List<TypeExpression> Arguments { get; } = arguments;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeArguments(this);
}