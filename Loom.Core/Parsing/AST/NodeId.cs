namespace Loom.Core.Parsing.AST;

public readonly record struct NodeId(int Value)
{
    public static readonly Dictionary<NodeId, Node> Map = [];

    public override string ToString() => $"#{Value}";
}