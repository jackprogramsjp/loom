using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class GenericNamedDeclaration(List<Token> otherTokens, Token keyword, Token name, TypeParameters? typeParameters, params Node?[] extraChildren)
    : NamedDeclaration([..otherTokens, keyword], name, [typeParameters, ..extraChildren])
{
    public Token Keyword { get; } = keyword;
    public TypeParameters? TypeParameters { get; } = typeParameters;
}