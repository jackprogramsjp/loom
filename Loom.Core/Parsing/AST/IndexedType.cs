using Loom.Syntax;

namespace Loom.Parsing.AST;

public class IndexedType(Token leftBracket, Token rightBracket, TypeExpression type, TypeExpression indexType)
    : TypeExpression([..type.Tokens, leftBracket, ..indexType.Tokens, rightBracket], [type, indexType])
{
    public Token LeftBracket { get; } = leftBracket;
    public Token RightBracket { get; } = rightBracket;
    public TypeExpression Type { get; } = type;
    public TypeExpression IndexType { get; } = indexType;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitIndexedType(this);
}