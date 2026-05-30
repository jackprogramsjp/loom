using Loom.Syntax;
using Loom.TypeChecking.Types;

namespace Loom.Parsing.AST;

public class PrimitiveType(Token name)
    : TypeName(name)
{
    public PrimitiveTypeKind Kind { get; } = name.Text switch
    {
        "number" => PrimitiveTypeKind.Number,
        "string" => PrimitiveTypeKind.String,
        "bool" => PrimitiveTypeKind.Bool,
        "never" => PrimitiveTypeKind.Never,
        "unknown" => PrimitiveTypeKind.Unknown,
        "void" => PrimitiveTypeKind.Void,
        "none" => PrimitiveTypeKind.None,
        _ => PrimitiveTypeKind.Unknown
    };

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitPrimitiveType(this);
}