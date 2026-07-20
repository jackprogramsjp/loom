namespace Loom.Core.Resolving;

public enum SymbolKind : byte
{
    Variable,
    Function,
    Parameter,
    Property,
    InjectedPropertyVariable,
    Type,
    EnumType,
    Interface,
    Trait
}