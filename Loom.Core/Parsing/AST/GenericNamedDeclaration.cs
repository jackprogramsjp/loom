using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class GenericNamedDeclaration(List<Token> otherTokens, Token keyword, Token name, TypeParameters? typeParameters, params Node?[] extraChildren)
    : DeclareSignature([..otherTokens, keyword], name, [typeParameters, ..extraChildren])
{
    public Token Keyword { get; } = keyword;
    public TypeParameters? TypeParameters { get; } = typeParameters;
}