using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class Expression(IEnumerable<Token?>? theseTokens = null, IEnumerable<Node?>? children = null)
    : Node(theseTokens ?? [], children ?? []);