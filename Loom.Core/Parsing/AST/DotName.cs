using Loom.Text;

namespace Loom.Parsing.AST;

public class DotName(Token dot, Token name)
{
    public Token Dot { get; } = dot;
    public Token Name { get; } = name;
    public List<Token> Tokens { get; } = [dot, name];
}