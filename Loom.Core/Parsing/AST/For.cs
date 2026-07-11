using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public sealed class For(Token keyword, List<Identifier> names, Token colon, Expression collectionExpression, Statement body)
    : Statement([keyword, ..names.SelectMany(n => n.Tokens), colon, ..collectionExpression.Tokens, ..body.Tokens], [collectionExpression, body])
{
    public Token Keyword { get; } = keyword;
    public List<Identifier> Names { get; } = names;
    public Token Colon { get; } = colon;
    public Expression CollectionExpression { get; } = collectionExpression;
    public Statement Body { get; } = body;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitFor(this);
}