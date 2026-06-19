namespace Loom.Parsing.AST;

public class InterfaceInvocation(Name name, TypeArguments? typeArguments, InterfaceInvocationBody body)
    : Expression([..name.Tokens, ..typeArguments?.Tokens ?? [], ..body.Tokens], [name, typeArguments, body])
{
    public Name Name { get; } = name;
    public TypeArguments? TypeArguments { get; } = typeArguments;
    public InterfaceInvocationBody Body { get; } = body;
    
    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitInterfaceInvocation(this);
}