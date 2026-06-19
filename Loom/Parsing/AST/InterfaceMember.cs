using Loom.Syntax;

namespace Loom.Parsing.AST;

public abstract class InterfaceMember(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Statement(theseTokens, children);