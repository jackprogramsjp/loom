namespace Loom.SemanticAnalysis;

internal record InitializationState(HashSet<string> Definite, HashSet<string> Maybe)
{
    public InitializationState(InitializationState from)
    {
        Definite = new HashSet<string>(from.Definite);
        Maybe = new HashSet<string>(from.Maybe);
    }
}