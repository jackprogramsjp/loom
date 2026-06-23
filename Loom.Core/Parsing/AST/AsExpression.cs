using Loom.Text;

namespace Loom.Parsing.AST;

public class AsExpression(Token keyword, Expression expression, TypeExpression type)
    : Expression([..expression.Tokens, keyword, ..type.Tokens], [expression, type])
{
    public Token Keyword { get; } = keyword;
    public Expression Expression { get; } = expression;
    public TypeExpression Type { get; } = type;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitAsExpression(this);
}