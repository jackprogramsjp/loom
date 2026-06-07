using Loom.Diagnostics;
using Loom.Parsing.AST;

namespace Loom.TypeChecking.Types;

public class InstantiatedType(GenericType generic, List<Type> arguments, TypeChecker typeChecker, Node node) : Type
{
    public Node Node { get; } = node;
    public TypeChecker Checker { get; } = typeChecker;
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
        {
            var parameter = Generic.Parameters[i];
            var argument = i < Arguments.Count ? Arguments[i] : parameter.DefaultType;
            if (argument == null)
            {
                Checker.ReportCannotInfer(Node, parameter);
                return PrimitiveType.Never;
            }
        
            substituted[parameter] = argument;
        }
        
        return TypeSimplifier.Simplify(SubstituteParameters(Generic.Underlying, substituted));
    }

    private static Type SubstituteParameters(Type type, Dictionary<TypeParameter, Type> substituted) =>
        type switch
        {
            TypeParameter parameter when substituted.TryGetValue(parameter, out var replacement) => replacement,
            _ => TypeSolver.Transform(type, t => SubstituteParameters(t, substituted))
        };
}