using Loom.Text;

namespace Loom.Parsing.AST;

public class DeclareFunctionSignature(
    Token keyword,
    Token name,
    TypeParameters? typeParameters,
    Parameters? parameters,
    ColonTypeClause returnType,
    params Node?[] extraChildren
)
    : GenericNamedDeclaration([], keyword, name, typeParameters, [parameters, returnType, ..extraChildren])
{
    public Parameters? Parameters { get; } = parameters;
    public ColonTypeClause ReturnType { get; } = returnType;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitDeclareFunctionSignature(this);
}