using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class EventDeclaration(Token keyword, Token name, TypeParameters? typeParameters, Parameters? parameters)
    : GenericNamedDeclaration([], keyword, name, typeParameters)
{
    public Parameters? Parameters { get; } = parameters;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitEventDeclaration(this);
}