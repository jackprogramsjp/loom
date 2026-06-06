using Loom.Syntax;

namespace Loom.Parsing.AST;

public class FunctionDeclaration(
    Token keyword,
    Token name,
    TypeParameters? typeParameters,
    Parameters? parameters,
    ColonTypeClause? returnType,
    Statement body
)
    : GenericNamedDeclaration(
        keyword,
        name,
        typeParameters,
        parameters,
        returnType,
        body
    )
{
    public Parameters? Parameters { get; } = parameters;
    public ColonTypeClause? ReturnType { get; } = returnType;
    public Statement Body { get; } = body;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitFunctionDeclaration(this);
}