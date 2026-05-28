using Loom.Syntax;

namespace Loom.Parsing.AST;

public enum PrimitiveTypeKind
{
    Number,
    String,
    Bool,
    None,
    Void
}

public class PrimitiveType(Token name) : TypeName(name)
{
    public PrimitiveTypeKind Kind { get; } = name.Text switch
    {
        "number" => PrimitiveTypeKind.Number,
        "string" => PrimitiveTypeKind.String,
        "bool" => PrimitiveTypeKind.Bool,
        "void" => PrimitiveTypeKind.Void,
        _ => PrimitiveTypeKind.None
    };
    
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitPrimitiveType(this);
}