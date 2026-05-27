namespace Loom.Parsing.AST;

public abstract class Statement(IEnumerable<ASTNode?> children) : ASTNode(children);