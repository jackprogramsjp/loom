using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class TypeExpression(IEnumerable<Token?> theseTokens, IEnumerable<ASTNode?> children) : ASTNode(theseTokens, children);