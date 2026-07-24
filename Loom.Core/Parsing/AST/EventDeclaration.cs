using System.Diagnostics.CodeAnalysis;
using Loom.Core.Resolving;
using Loom.Core.Text;

namespace Loom.Core.Parsing.AST;

public class EventDeclaration(Token keyword, Token name, TypeParameters? typeParameters, Parameters? parameters, Attributes? attributes = null)
    : GenericNamedDeclaration([], keyword, name, typeParameters, attributes),
      IWithAttributes
{
    public Parameters? Parameters { get; } = parameters;
    public Attributes? Attributes { get; } = attributes;

    public bool TryGetIntrinsicAttribute(SemanticModel semanticModel, string name, [MaybeNullWhen(false)] out AttributeSymbol attribute)
    {
        // Interface-member events are declared twice against this same node: once as a plain
        // Symbol (via the Resolver's generic body traversal) and once as the real PropertySymbol
        // that carries attributes (via ResolveInterfaceBody). GetDeclarationSymbol returns the
        // first match by kind, which is the plain Symbol, so we must search all symbols
        // registered for this node rather than assume there's only one. Global events only ever
        // get the plain Symbol, so this correctly yields no attribute for them.
        var eventSymbol = semanticModel.GetDeclarationSymbols(this).OfType<PropertySymbol>().FirstOrDefault();
        attribute = eventSymbol?.Attributes.Find(a => a is { IsIntrinsic: true } && a.Name == name);
        return attribute != null;
    }

    public override T Accept<T>(Visitor<T> visitor) => visitor.VisitEventDeclaration(this);
}
