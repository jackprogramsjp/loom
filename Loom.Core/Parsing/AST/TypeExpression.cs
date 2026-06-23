using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class TypeExpression(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Node(theseTokens, children);