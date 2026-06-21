using Loom.Parsing.AST;

namespace Loom.TypeChecking.Types;

public sealed class GenericType(GenericNamedDeclaration declaration, List<TypeParameter> parameters, Type underlyingType) : Type
{
    public GenericNamedDeclaration Declaration { get; } = declaration;
    public List<TypeParameter> Parameters { get; } = parameters;
    public Type UnderlyingType { get; } = underlyingType;

    public override bool Equals(Type? other) =>
        other is GenericType generic
        && Declaration.Id == generic.Declaration.Id
        && Parameters.Count == generic.Parameters.Count
        && Parameters.All(t => generic.Parameters.Any(u => u.Equals(t)))
        && UnderlyingType.Equals(generic.UnderlyingType);

    public override string ToString() => $"{Declaration.Name.Text}<{string.Join(", ", Parameters.ConvertAll(p => p.ToString()))}>";
}