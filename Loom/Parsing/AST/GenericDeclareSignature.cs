using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class GenericDeclareSignature(Token keyword, Token name, TypeParameters? typeParameters, params Node?[] extraChildren)
    : DeclareSignature([keyword], name, [typeParameters, ..extraChildren])
{
    public Token Keyword { get; } = keyword;
    public TypeParameters? TypeParameters { get; } = typeParameters;
}