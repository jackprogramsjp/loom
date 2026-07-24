global using SymbolTable = System.Collections.Generic.Dictionary<Loom.Core.Parsing.AST.NodeId, System.Collections.Generic.List<Loom.Core.Resolving.Symbol>>;
using System.Diagnostics.CodeAnalysis;
using Loom.Core.Diagnostics;
using Loom.Core.Parsing.AST;
using Loom.Core.TypeChecking;
using Loom.Core.TypeChecking.Types;
using LiteralType = Loom.Core.TypeChecking.Types.LiteralType;
using Type = Loom.Core.TypeChecking.Types.Type;

namespace Loom.Core.Resolving;

public sealed record SemanticModel(Tree Tree, DiagnosticBag Diagnostics, SymbolTable Declarations, SymbolTable References)
    : DiagnosedResult(Diagnostics)
{
    internal int RuntimeReferences = 0;

    /// <summary>
    ///     Node IDs of reference-site nodes that originate from a non-intrinsic source file.
    ///     Populated by the resolver as references are recorded, so
    ///     <see cref="MustImportRuntimeLibrary" /> can filter references by the file of the
    ///     referencing node without holding on to the node instances themselves.
    /// </summary>
    internal HashSet<NodeId> NonIntrinsicReferenceNodes { get; } = [];

    public bool DisableRuntimeLibraryImport { get; set; }
    public bool MustImportRuntimeLibrary =>
        !DisableRuntimeLibraryImport
        && !Tree.File.IsIntrinsic
        && (
            RuntimeReferences > 0
            || References.Any(pair => NonIntrinsicReferenceNodes.Contains(pair.Key)
                && pair.Value.Any(s => s is { File.Name: "runtime.loom", IsIntrinsic: true, IsTypeSymbol: true })
            )
        );

    internal TypeSolver TypeSolver { get; } = new(new DiagnosticBag());

    public bool IsCompileTimeConstant(Expression expression) =>
        expression is Literal or NameOf
        || expression is QualifiedName name && GetDeclaringSymbol(name.Identifier) is { Declaration: EnumDeclaration }
        || expression is PropertyAccess access && GetDeclaringSymbol(access.Expression) is { Declaration: EnumDeclaration }
        || expression is ElementAccess elementAccess && GetDeclaringSymbol(elementAccess.Expression) is { Declaration: EnumDeclaration };

    public object? GetConstantValue(Expression expression) =>
        expression switch
        {
            QualifiedName qn when GetType(qn.Identifier) is ObjectType objectType
                && objectType.GetProperty(qn.Names.First().Name.Text) is { ValueType: LiteralType literalType } =>
                literalType.Value,
            _ when GetType(expression) is LiteralType literalType => literalType.Value,
            _ => null
        };

    public List<Symbol> GetDeclarationSymbols(Node node)
    {
        while (true)
        {
            if (node is not Declare declare)
                return Declarations.GetValueOrDefault(node.Id, []);

            node = declare.Signature;
        }
    }

    public Symbol? GetSymbol(Node node, SymbolKind? kind = null) => FindSymbol(node, kind, References);

    public Symbol? GetDeclarationSymbol(Node node, SymbolKind? kind = null)
    {
        while (true)
        {
            if (node is not Declare declare)
                return FindSymbol(node, kind, Declarations);

            node = declare.Signature;
        }
    }

    public Symbol? GetDeclaringSymbol(Node node, SymbolKind? kind = null)
    {
        var referenceSymbol = GetSymbol(node, kind);
        return referenceSymbol == null ? null : GetDeclarationSymbol(referenceSymbol.Declaration, kind);
    }

    public bool TryGetIntrinsicAttribute(Expression expression, string name, [MaybeNullWhen(false)] out AttributeSymbol attribute)
    {
        attribute = null;
        var property = GetPropertySymbol(expression);
        return property != null && property.TryGetIntrinsicAttribute(name, out attribute);
    }

    public PropertySymbol? GetPropertySymbol(Expression expression)
    {
        var (objectExpression, names) = expression switch
        {
            QualifiedName qualified => (qualified.Identifier as Expression, qualified.Names.Select(d => d.Name.Text).ToArray()),
            PropertyAccess propertyAccess => (propertyAccess.Expression, propertyAccess.Names.Select(d => d.Name.Text).ToArray()),
            ElementAccess { IndexExpression: Literal { Value: string propertyName } } elementAccess => (elementAccess.Expression, [propertyName]),
            _ => (expression, [])
        };

        if (names.Length == 0 || GetType(objectExpression) is not InterfaceType interfaceType)
            return null;

        var interfaceSymbol = FindDeclarationSymbol<InterfaceSymbol>(interfaceType.Name);
        return interfaceSymbol?.GetPropertyAtPath(names);
    }

    public Type GetType(Node node) => TypeSolver.GetType(node);
    public Type? GetDeclarationType(Node node) => GetSymbol(node) is { } symbol ? TypeSolver.GetType(symbol.Declaration) : null;
    public T? FindIntrinsicDeclarationSymbol<T>(string name) where T : Symbol => FindDeclarationSymbol<T>(s => s.IsIntrinsic && s.Name == name);

    private T? FindDeclarationSymbol<T>(string name) where T : Symbol => FindDeclarationSymbol<T>(s => s.Name == name);
    private T? FindDeclarationSymbol<T>(Func<T, bool> predicate) where T : Symbol => Declarations.Values.SelectMany(s => s).OfType<T>().FirstOrDefault(predicate);

    private static Symbol? FindSymbol(Node node, SymbolKind? kind, SymbolTable table)
    {
        var symbols = table.GetValueOrDefault(node.Id, []);
        return kind != null ? symbols.Find(s => s.Kind == kind) : symbols.FirstOrDefault();
    }
}