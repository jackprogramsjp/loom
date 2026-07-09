namespace Loom.Resolving;

public enum ResolverContext : byte
{
    None,
    Function,
    Loop,
    Scheduler,
    Declaration
}