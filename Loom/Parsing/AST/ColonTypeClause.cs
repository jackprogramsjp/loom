using Loom.Syntax;

namespace Loom.Parsing.AST;

public class ColonTypeClause(Token colonToken, TypeExpression type) : ASTNode([colonToken], [type])
{
    public Token ColonToken { get; } = colonToken;
    public TypeExpression Type { get; } = type;
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitColonTypeClause(this);
}