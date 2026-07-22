namespace Loom.Core.Parsing.AST;

public readonly record struct NodeId(int Value)
{
    public override string ToString() => $"#{Value}";
}