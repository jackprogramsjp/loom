using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class TypeArguments(Token leftArrow, Token rightArrow, List<TypeExpression> argumentsList)
    : Node([leftArrow, ..argumentsList.SelectMany(p => p.Tokens), rightArrow], argumentsList)
{
    public Token LeftArrow { get; } = leftArrow;
    public Token RightArrow { get; } = rightArrow;
    public List<TypeExpression> ArgumentsList { get; } = argumentsList;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeArguments(this);
}