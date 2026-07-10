using Tomlyn.Serialization;

namespace Loom.Config;

// ReSharper disable file ClassNeverInstantiated.Global
public sealed class FilesConfig
{
    public string SourceDirectory { get; set; } = "src";
    public string OutputDirectory { get; set; } = "dist";
}

public sealed class LoomConfig
{
    [TomlIgnore] public string ProjectDirectory { get; set; } = "?";

    public bool NoEmit { get; set; }
    public FilesConfig Files { get; init; } = new();
}