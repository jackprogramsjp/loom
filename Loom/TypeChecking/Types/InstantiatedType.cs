namespace Loom.TypeChecking.Types;

public class InstantiatedType(GenericType generic, List<Type> arguments) : Type
{
    public GenericType Generic { get; } = generic;
    public List<Type> Arguments { get; } = arguments;

    public override bool Equals(Type? other) =>
        other is InstantiatedType instantiated
        && Generic.Equals(instantiated.Generic)
        && Arguments.Count == instantiated.Arguments.Count
        && Arguments.All(t => instantiated.Arguments.Any(u => u.Equals(t)));
    
    public override bool IsAssignableTo(Type other) => Expand().IsAssignableTo(other);

    public override string ToString() => Generic.Declaration.Name.Text + "<" + string.Join(", ", Arguments.ConvertAll(p => p.ToString())) + ">";
    
    public Type Expand()
    {
        var substituted = new Dictionary<TypeParameter, Type>();
        for (var i = 0; i < Generic.Parameters.Count; i++)
            substituted[Generic.Parameters[i]] = Arguments[i];
        
        return TypeSimplifier.Simplify(SubstituteParameters(Generic.Underlying, substituted));
    }

    private static Type SubstituteParameters(Type type, Dictionary<TypeParameter, Type> substituted) =>
        type switch
        {
            TypeParameter parameter when substituted.TryGetValue(parameter, out var replacement) => replacement,
            InstantiatedType instantiated => new InstantiatedType(instantiated.Generic, instantiated.Arguments.ConvertAll(a => SubstituteParameters(a, substituted))),
            IntersectionType intersection => new IntersectionType(intersection.Types.ConvertAll(t => SubstituteParameters(t, substituted))),
            UnionType union => new UnionType(union.Types.ConvertAll(t => SubstituteParameters(t, substituted))),
            _ => type
        };
}