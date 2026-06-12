namespace Loom.SemanticAnalysis;

internal record InitializationState(HashSet<Symbol> Definite, HashSet<Symbol> Maybe)
{
    public InitializationState(InitializationState from)
    {
        Definite = new HashSet<Symbol>(from.Definite);
        Maybe = new HashSet<Symbol>(from.Maybe);
    }
}