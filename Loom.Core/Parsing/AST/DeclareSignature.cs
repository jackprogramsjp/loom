using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class DeclareSignature(List<Token> otherTokens, Token name, params Node?[] children)
    : NamedDeclaration(otherTokens, name, children);