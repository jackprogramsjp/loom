namespace Loom.Parsing.AST;

public abstract class Expression(IEnumerable<ASTNode?> children) : ASTNode(children);