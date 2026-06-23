using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class Name(Token token, List<Token?>? extraTokens = null, List<Node?>? extraChildren = null)
    : AssignmentTarget([token, ..extraTokens ?? []], extraChildren ?? [])
{
    public Token Token { get; } = token;
}