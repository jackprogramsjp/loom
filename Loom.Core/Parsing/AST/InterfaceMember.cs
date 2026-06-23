using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class InterfaceMember(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Statement(theseTokens, children);