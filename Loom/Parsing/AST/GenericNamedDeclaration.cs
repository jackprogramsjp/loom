using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class GenericNamedDeclaration(Token keyword, Token name, TypeParameters? typeParameters, params Node?[] extraChildren)
    : NamedDeclaration([keyword], name, [typeParameters, ..extraChildren])
{
    public Token Keyword { get; } = keyword;
    public TypeParameters? TypeParameters { get; } = typeParameters;
}