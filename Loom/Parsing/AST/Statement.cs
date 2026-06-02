using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class Statement(IEnumerable<Token?>? theseTokens = null, IEnumerable<Node?>? children = null)
    : Node(theseTokens ?? [], children ?? []);