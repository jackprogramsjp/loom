using Loom.Syntax;

namespace Loom.Parsing.AST;

public class ColonTypeClause(Token colonToken, TypeExpression type)
{
    public Token ColonToken { get; } = colonToken;
    public TypeExpression Type { get; } = type;
    
    public override string ToString() => $"{ColonToken.Text} {Type}";
}