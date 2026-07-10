namespace Loom.Luau.AST;

public class QualifiedTypeName(List<string> qualifications, TypeName finalName) : LuauType
{
    public List<string> Qualifications { get; } = qualifications;
    public TypeName FinalName { get; } = finalName;

    public override string Render(RenderState state) => string.Join('.', Qualifications) + '.' + FinalName.Render(state);
}