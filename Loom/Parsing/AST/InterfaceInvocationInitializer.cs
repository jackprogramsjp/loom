using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class InterfaceInvocationInitializer(Expression expression, IEnumerable<Token?> otherTokens, IEnumerable<Node?>? extraChildren = null)
    : Expression([..otherTokens, ..expression.Tokens], [..extraChildren ?? [], expression])
{
    public Expression Expression { get; } = expression;
}