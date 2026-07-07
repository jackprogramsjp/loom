using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.TypeChecking.Types;
using Type = Loom.TypeChecking.Types.Type;

namespace Loom.TypeChecking;

public sealed partial class TypeChecker
{
    public override Type VisitInterfaceDeclaration(InterfaceDeclaration interfaceDeclaration)
    {
        var name = interfaceDeclaration.Name.Text;
        var constraints = interfaceDeclaration.ColonTypeListClause?.Types.ConvertAll(Visit).OfType<InterfaceType>().ToList() ?? [];
        var typeParameters = interfaceDeclaration.TypeParameters?.ParameterList.ConvertAll(VisitTypeParameter);
        var indexerDeclaration = interfaceDeclaration.Body?.Members.OfType<IndexerDeclaration>().FirstOrDefault();
        var propertyDeclarations = interfaceDeclaration.Body?.Members.OfType<PropertyDeclaration>().ToList() ?? [];
        var indexer = ResolveInterfaceIndexer(constraints, indexerDeclaration);
        var properties = ResolveInterfaceProperties(constraints, propertyDeclarations);
        var objectType = new ObjectType(indexer, properties);
        var interfaceType = new InterfaceType(name, constraints, objectType);
        if (typeParameters == null)
            return BindType(interfaceDeclaration, interfaceType);

        var genericType = new GenericType(interfaceDeclaration, typeParameters, interfaceType);
        return BindType(interfaceDeclaration, genericType);
    }

    public override Type VisitInterfaceInvocation(InterfaceInvocation node)
    {
        var type = Visit(node.Name);
        if (type.Equals(Intrinsics.Range))
            _diagnostics.Warn(node, InternalCodes.SimplifiableCode, "Use range literal.");

        if (type is InterfaceType nonGeneric)
        {
            CheckInterfaceInvocationInitializers(node, nonGeneric);
            return BindType(node, nonGeneric);
        }

        if (type is not GenericType { UnderlyingType: InterfaceType underlying } generic)
        {
            _diagnostics.Error(node, InternalCodes.InvalidInvocation, $"Type '{type}' is not an interface.");
            return BindType(node, Types.PrimitiveType.Never);
        }

        if (!TrySubstituteGenericInterface(node, generic, underlying, out var interfaceType))
            return BindType(node, Types.PrimitiveType.Never);

        CheckInterfaceInvocationInitializers(node, interfaceType);
        return BindType(node, interfaceType);
    }

    private bool TrySubstituteGenericInterface(
        InterfaceInvocation node,
        GenericType generic,
        InterfaceType underlying,
        [MaybeNullWhen(false)] out InterfaceType substituted)
    {
        substituted = null;
        var substitution = node.TypeArguments != null
            ? ResolveExplicitInterfaceTypeArguments(node, generic)
            : _inferrer.InferInterfaceTypeArguments(node, generic, underlying);

        if (substitution == null)
            return false;

        foreach (var tp in generic.Parameters)
        {
            if (tp.Constraint == null || !substitution.TryGetValue(tp, out var arg)) continue;
            if (!CheckTypeParameterConstraints(node, arg, tp))
                return false;
        }

        var substitutedObject = SubstituteObjectType(underlying.ObjectType, substitution);
        substituted = new InterfaceType(underlying.Name, underlying.Constraints, substitutedObject);
        return true;
    }

    private List<ObjectProperty> ResolveInterfaceProperties(List<InterfaceType> constraints, List<PropertyDeclaration> propertyDeclarations)
    {
        var properties = new List<ObjectProperty>();
        foreach (var declaration in propertyDeclarations)
        {
            var name = declaration.Name.Text;
            if (constraints.Find(i => i.GetProperty(name) != null) is { } subclass)
            {
                _diagnostics.Error(declaration, InternalCodes.ConstraintPropertyOverride, $"Property '{name}' is already declared within constraint '{subclass}'.");
                return properties;
            }

            var isMutable = declaration.MutKeyword != null;
            var valueType = Visit(declaration.ColonTypeClause);
            properties.Add(new ObjectProperty(isMutable, name, valueType));
        }

        return properties;
    }

    private ObjectIndexer? ResolveInterfaceIndexer(List<InterfaceType> constraints, IndexerDeclaration? indexerDeclaration)
    {
        if (indexerDeclaration == null)
            return null;

        var isMutable = indexerDeclaration.MutKeyword != null;
        var indexType = Visit(indexerDeclaration.IndexType);
        var valueType = Visit(indexerDeclaration.ColonTypeClause);
        if (constraints.Find(i => i.Indexer != null) is not { } subclass)
            return new ObjectIndexer(isMutable, indexType, valueType);

        _diagnostics.Error(indexerDeclaration, InternalCodes.ConstraintIndexerOverride, $"An indexer is already declared within constraint '{subclass}'.");
        return null;
    }

    private void CheckInterfaceInvocationInitializers(InterfaceInvocation node, InterfaceType interfaceType)
    {
        var objectType = interfaceType.ObjectType;
        var providedProperties = new HashSet<string>();
        foreach (var property in node.Body.Initializers.SelectMany(initializer => CheckInterfaceInvocationInitializer(interfaceType, initializer)))
            providedProperties.Add(property);

        foreach (var property in objectType.Properties.Where(property => !providedProperties.Contains(property.Name)))
            _diagnostics.Error(
                node.Body,
                InternalCodes.IncompleteInterfaceInvocation,
                $"Missing property initializer for '{property.Name}' in interface '{interfaceType.Name}'."
            );
    }

    private HashSet<string> CheckInterfaceInvocationInitializer(InterfaceType interfaceType, InterfaceInvocationInitializer initializer)
    {
        var providedProperties = new HashSet<string>();
        switch (initializer)
        {
            case InterfaceInvocationPropertyInitializer propertyInitializer:
            {
                var propertyName = CheckPropertyInitializer(propertyInitializer, interfaceType);
                if (propertyName != null)
                    providedProperties.Add(propertyName);

                break;
            }
            case InterfaceInvocationIndexInitializer indexInitializer:
            {
                CheckIndexInitializer(indexInitializer, interfaceType);
                break;
            }
        }

        return providedProperties;
    }

    private string? CheckPropertyInitializer(InterfaceInvocationPropertyInitializer propertyInitializer, InterfaceType interfaceType)
    {
        var name = propertyInitializer.Name.Text;
        var property = interfaceType.GetProperty(name);
        if (property == null)
        {
            _diagnostics.Error(
                propertyInitializer,
                InternalCodes.InvalidAccess,
                $"Property '{name}' does not exist on interface '{interfaceType.Name}'."
            );

            return null;
        }

        var valueType = Visit(propertyInitializer.Expression);
        _semanticModel.TypeSolver.AddConstraint(valueType, property.ValueType, propertyInitializer.Expression);
        return name;
    }

    private void CheckIndexInitializer(InterfaceInvocationIndexInitializer initializer, InterfaceType interfaceType)
    {
        var indexer = interfaceType.Indexer;
        if (indexer == null)
        {
            _diagnostics.Error(
                initializer,
                InternalCodes.InvalidAccess,
                $"Interface '{interfaceType.Name}' does not have an indexer."
            );

            return;
        }

        var indexType = Visit(initializer.IndexExpression);
        var valueType = Visit(initializer.Expression);
        _semanticModel.TypeSolver.AddConstraint(indexType, indexer.KeyType, initializer.IndexExpression);
        _semanticModel.TypeSolver.AddConstraint(valueType, indexer.ValueType, initializer.Expression);
    }
}