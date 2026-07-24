namespace Loom.Core;

public enum RuntimeImportStatus
{
    Resolved,
    RojoMissing,
    NotFoundInRojo,
}

public sealed record RuntimeImport(RuntimeImportStatus Status, string Path);