namespace Loom.TypeChecking.Types;

public class PrimitiveType(PrimitiveTypeKind kind) : Type
{
    public static readonly PrimitiveType Number = new(PrimitiveTypeKind.Number);
    public static readonly PrimitiveType String = new(PrimitiveTypeKind.String);
    public static readonly PrimitiveType Bool = new(PrimitiveTypeKind.Bool);
    public static readonly PrimitiveType Void = new(PrimitiveTypeKind.Void);
    public static readonly PrimitiveType None = new(PrimitiveTypeKind.None);
    public static readonly PrimitiveType Unknown = new(PrimitiveTypeKind.Unknown);
    public static readonly PrimitiveType Never = new(PrimitiveTypeKind.Never);

    public PrimitiveTypeKind Kind { get; } = kind;

    public override bool IsAssignableTo(Type other)
    {
        // never is assignable to everything, nothing is assignable to it
        if (Kind is PrimitiveTypeKind.Unknown or PrimitiveTypeKind.Never)
            return true;
        
        if (other is PrimitiveType primitiveType)
            return primitiveType.Kind == PrimitiveTypeKind.Unknown ||  Kind == primitiveType.Kind;

        return false;
    }

    public override string ToString() => Kind.ToString().ToLower();

    private bool IsKind(PrimitiveTypeKind kind)
    {
        if (Kind is PrimitiveTypeKind.None or PrimitiveTypeKind.Void)
            return kind is PrimitiveTypeKind.None or PrimitiveTypeKind.Void;

        return Kind == kind;
    }
}