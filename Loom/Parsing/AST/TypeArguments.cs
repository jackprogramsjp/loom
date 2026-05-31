using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeParameters(Token leftArrow, Token rightArrow, List<TypeParameter> parameters)
    : Node([leftArrow, ..parameters.SelectMany(p => p.Tokens), rightArrow], parameters)
{
    public Token LeftArrow { get; } = leftArrow;
    public Token RightArrow { get; } = rightArrow;
    public List<TypeParameter> Parameters { get; } = parameters;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeParameters(this);
}