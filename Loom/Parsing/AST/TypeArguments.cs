using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeArguments(Token colonColonLeftArrow, Token rightArrow, List<TypeExpression> argumentsList)
    : Node([colonColonLeftArrow, ..argumentsList.SelectMany(p => p.Tokens), rightArrow], argumentsList)
{
    public Token ColonColonLeftArrow { get; } = colonColonLeftArrow;
    public Token RightArrow { get; } = rightArrow;
    public List<TypeExpression> ArgumentsList { get; } = argumentsList;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeArguments(this);
}