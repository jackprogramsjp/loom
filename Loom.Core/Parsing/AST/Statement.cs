using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class Statement(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Node(theseTokens, children);