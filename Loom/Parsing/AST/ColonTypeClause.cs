using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class ColonTypeClause(Token colonToken, TypeExpression type) : ASTNode([type])
{
    public Token ColonToken { get; } = colonToken;
    public TypeExpression Type { get; } = type;
    
    public override string ToString() => $"{ColonToken.Text} {Type}";
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitColonTypeClause(this);
}