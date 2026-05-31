using Loom.Parsing.AST;

namespace Loom.TypeChecking.Types;

public class GenericType(TypeDeclaration declaration, List<TypeParameter> parameters, Type underlying) : Type
{
    public TypeDeclaration Declaration { get; } = declaration;
    public List<TypeParameter> Parameters { get; } = parameters;
    public Type Underlying { get; } = underlying;

    public override bool Equals(Type? other) =>
        other is GenericType generic
        && Declaration.Id == generic.Declaration.Id
        && Parameters.Count == generic.Parameters.Count
        && Parameters.All(t => generic.Parameters.Any(u => u.Equals(t)))
        && Underlying.Equals(generic.Underlying);

    public override string ToString() => $"{Declaration.Name.Text}<{string.Join(", ", Parameters.ConvertAll(p => p.ToString()))}>";
}