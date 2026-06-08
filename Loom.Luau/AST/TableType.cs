namespace Loom.Luau.AST;

public class TableType(LuauType? keyType, LuauType valueType) : LuauType
{
    public LuauType? KeyType { get; } = keyType;
    public LuauType ValueType { get; } = valueType;
    
    public override string Render(RenderState state) => KeyType != null ? $"{{ [{KeyType}]: {ValueType} }}" : $"{{ {ValueType} }}";
}