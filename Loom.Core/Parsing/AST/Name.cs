using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public abstract class Name(Token token, List<Token?>? extraTokens = null, List<Node?>? extraChildren = null)
    : AssignmentTarget([token, ..extraTokens ?? []], extraChildren ?? [])
{
    public Token Token { get; } = token;
}