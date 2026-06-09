namespace Loom.Luau.AST;

public class Function(string name, TypeParameters? typeParameters, List<Parameter> parameters, LuauType? returnType, List<LuauStatement> statements) : LuauStatement
{
    public string Name { get; } = name;
    public TypeParameters? TypeParameters { get; } = typeParameters;
    public List<Parameter> Parameters { get; } = parameters;
    public LuauType? ReturnType { get; } = returnType;
    public List<LuauStatement> Statements { get; } = statements;

    public override string Render(RenderState state) =>
        "const function " + Name
        + (TypeParameters?.Render(state) ?? "")
        + '('
        + string.Join(", ", Parameters.ConvertAll(p => p.Render(state)))
        + ')'
        + (ReturnType != null ? ": " + ReturnType.Render(state) : "")
        + '\n'
        + state.Block(() => string.Join('\n', Statements.ConvertAll(statement => state.IndentedLine(statement.Render(state)))))
        + "end";
}