namespace Loom.TypeChecking.Types;

public class PrimitiveType(PrimitiveTypeKind kind) : Type
{
    public static readonly PrimitiveType Number = new(PrimitiveTypeKind.Number);
    public static readonly PrimitiveType String = new(PrimitiveTypeKind.String);
    public static readonly PrimitiveType Bool = new(PrimitiveTypeKind.Bool);
    public static readonly PrimitiveType Void = new(PrimitiveTypeKind.Void);
    public static readonly PrimitiveType None = new(PrimitiveTypeKind.None);
    public static readonly PrimitiveType Unknown = new(PrimitiveTypeKind.Unknown);

    public PrimitiveTypeKind Kind { get; } = kind;

    public override bool IsAssignableTo(Type other) => Kind == PrimitiveTypeKind.Unknown || other is PrimitiveType primitive && IsKind(primitive.Kind);

    public override string ToString() => Kind.ToString().ToLower();

    private bool IsKind(PrimitiveTypeKind kind)
    {
        if (Kind is PrimitiveTypeKind.None or PrimitiveTypeKind.Void)
            return kind is PrimitiveTypeKind.None or PrimitiveTypeKind.Void;

        return Kind == kind;
    }
}