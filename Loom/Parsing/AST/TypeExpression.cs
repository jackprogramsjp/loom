using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class TypeExpression(IEnumerable<Token?>? theseTokens = null, IEnumerable<Node?>? children = null)
    : Node(theseTokens ?? [], children ?? []);