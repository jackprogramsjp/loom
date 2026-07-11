using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public abstract class AssignmentTarget(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Expression(theseTokens, children);