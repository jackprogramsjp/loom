using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class TypeDeclaration(Token keyword, Token name, TypeParameters? typeParameters, params Node[] extraChildren)
    : Statement(
        [keyword, name, ..typeParameters?.Tokens ?? [], ..extraChildren.SelectMany(c => c.Tokens)],
        typeParameters?.Parameters.Concat(extraChildren) ?? extraChildren
    )
{
    public Token Keyword { get; } = keyword;
    public Token Name { get; } = name;
    public TypeParameters? TypeParameters { get; } = typeParameters;
}