using Loom.Parsing.AST.Traversal;
using Loom.Syntax;

namespace Loom.Parsing.AST;

public class TypeName(Token name) : TypeExpression([])
{
    public Token Name { get; } = name;

    public override string ToString() => Name.Text;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitTypeName(this);
}