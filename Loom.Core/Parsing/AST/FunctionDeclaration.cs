using Loom.Text;

namespace Loom.Parsing.AST;

public class FunctionDeclaration(
    Token keyword,
    Token name,
    TypeParameters? typeParameters,
    Parameters? parameters,
    ColonTypeClause? returnType,
    Statement body
)
    : DeclareFunctionSignature(
        keyword,
        name,
        typeParameters,
        parameters,
        returnType!,
        body
    )
{
    public new ColonTypeClause? ReturnType { get; } = returnType;
    public Statement Body { get; } = body;

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitFunctionDeclaration(this);
}