namespace Loom.SemanticAnalysis;

public enum ResolverContext : byte
{
    None,
    Function,
    Loop,
    Declaration
}