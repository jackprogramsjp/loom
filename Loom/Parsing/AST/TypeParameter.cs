using Loom.Syntax;

namespace Loom.Parsing.AST;

// TODO: extends constraint, etc.
public class TypeParameter(Token name, EqualsTypeClause? equalsTypeClause) : Node([name], [equalsTypeClause])
{
    public Token Name { get; } = name;
    public EqualsTypeClause? EqualsTypeClause { get; } = equalsTypeClause;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitTypeParameter(this);
}