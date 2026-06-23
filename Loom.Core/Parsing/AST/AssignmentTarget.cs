using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class AssignmentTarget(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Expression(theseTokens, children);