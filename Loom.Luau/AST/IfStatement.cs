namespace Loom.Luau.AST;

public record ElseIfBranch(LuauExpression Condition, Chunk Branch);

public class IfStatement(LuauExpression condition, Chunk thenBranch, List<ElseIfBranch> elseIfBranches, Chunk? elseBranch) : LuauStatement
{
    public LuauExpression Condition { get; } = condition;
    public Chunk ThenBranch { get; } = thenBranch;
    public List<ElseIfBranch> ElseIfBranches { get; } = elseIfBranches;
    public Chunk? ElseBranch { get; } = elseBranch;

    public override string Render(RenderState state) =>
        $"if {Condition.Render(state)} then\n"
        + state.Indented(ThenBranch.Render(state))
        + string.Join("", ElseIfBranches.ConvertAll(elseIf => $"elseif {elseIf.Condition.Render(state)} then\n" +state.Indented(elseIf.Branch.Render(state))))
        + (ElseBranch != null ? "else\n" + state.Indented(ElseBranch.Render(state)) : "")
        + "end";
}