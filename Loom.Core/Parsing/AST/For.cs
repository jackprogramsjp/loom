using Loom.Text;

namespace Loom.Parsing.AST;

public sealed class For(Token keyword, DeclareVariableSignature declaration, Token inKeyword, Expression collectionExpression, Statement body)
    : Statement([keyword, ..declaration.Tokens, inKeyword, ..collectionExpression.Tokens, ..body.Tokens], [declaration, collectionExpression, body])
{
    public Token Keyword { get; } = keyword;
    public DeclareVariableSignature Declaration { get; } = declaration;
    public Token InKeyword { get; } = inKeyword;
    public Expression CollectionExpression { get; } = collectionExpression;
    public Statement Body { get; } = body;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitFor(this);
}