using System.Text.Json;

namespace Loom.Config;

public sealed class RojoProject
{
    public required RojoNode Tree { get; init; }

    public static RojoProject? Read(string projectFilePath)
    {
        if (!File.Exists(projectFilePath))
            return  null;

        using var document = JsonDocument.Parse(File.ReadAllText(projectFilePath));
        if (!document.RootElement.TryGetProperty("tree", out var treeElement))
            return null;
        
        return new RojoProject { Tree = RojoNode.Parse(treeElement) };
    }
}