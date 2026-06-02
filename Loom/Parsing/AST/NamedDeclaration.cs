using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class NamedDeclaration(List<Token> preTokens, Token name, params Node?[] children)
    : Statement(
        [..preTokens, name, ..children.Where(c => c != null).Cast<Node>().SelectMany(c => c.Tokens)],
        children
    )
{
    public Token Name { get; } = name;
}