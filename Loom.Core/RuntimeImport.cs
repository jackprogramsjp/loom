namespace Loom.Core;

public enum RuntimeImportStatus
{
    Resolved,
    RojoMissing,
    NotFoundInRojo,
}

public sealed record RuntimeImport(RuntimeImportStatus Status, string Path)
{
    public const string PathPrefix = "@game/";
    public const string DefaultPath = PathPrefix + "ReplicatedStorage/include/loom_runtime";

    public static RuntimeImport Default { get; } = new(RuntimeImportStatus.RojoMissing, DefaultPath);
}