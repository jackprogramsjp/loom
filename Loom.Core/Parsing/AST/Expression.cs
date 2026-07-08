using Loom.Text;

namespace Loom.Parsing.AST;

public abstract class Expression(IEnumerable<Token?> theseTokens, IEnumerable<Node?> children)
    : Node(theseTokens, children)
{
    public new List<Expression> Children => base.Children.OfType<Expression>().ToList();
}