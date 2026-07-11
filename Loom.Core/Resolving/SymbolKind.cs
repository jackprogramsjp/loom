namespace Loom.Core.Resolving;

public enum SymbolKind : byte
{
    Variable,
    Function,
    Parameter,
    Type,
    EnumType,
    Interface,
    Trait
}