using Loom.Syntax;

namespace Loom.Parsing.AST;

public class Identifier(Token name) : Expression([])
{
    public Token Name { get; } = name;

    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitIdentifier(this);

    public override string ToString() => Name.Text;
}