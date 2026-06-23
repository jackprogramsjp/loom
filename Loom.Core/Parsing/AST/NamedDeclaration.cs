using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class NamedDeclaration(List<Token> otherTokens, Token name, params Node?[] children)
    : Statement(
        [..otherTokens, name, ..children.Where(c => c != null).Cast<Node>().SelectMany(c => c.Tokens)],
        children
    )
{
    public Token Name { get; } = name;
}