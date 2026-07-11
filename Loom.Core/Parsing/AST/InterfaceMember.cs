using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public abstract class InterfaceMember(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Statement(theseTokens, children);