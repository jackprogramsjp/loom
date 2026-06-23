using Loom.Text;

namespace Loom.Parsing.AST;

public class Arguments(Token leftParen, Token rightParen, List<Expression> arguments)
    : Expression([leftParen, rightParen, ..arguments.SelectMany(p => p.Tokens)], arguments)
{
    public Token LeftParen { get; } = leftParen;
    public Token RightParen { get; } = rightParen;
    public List<Expression> ArgumentList { get; } = arguments;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitArguments(this);
}