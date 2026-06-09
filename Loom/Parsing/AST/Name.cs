using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class Name(Token token, params Node?[] extraChildren)
    : AssignmentTarget([token, ..extraChildren.SelectMany(c => c?.Tokens!)], extraChildren)
{
    public Token Token { get; } = token;
}