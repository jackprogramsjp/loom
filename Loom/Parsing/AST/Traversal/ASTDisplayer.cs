namespace Loom.Parsing.AST.Traversal;

public class ASTDisplayer : IVisitor<string>
{
    private int _indent;

    public void Display(Tree tree)
    {
        var content = VisitTree(tree);
        Console.WriteLine(content);
    }

    public string Visit(ASTNode node) => node.Accept(this);
    public string VisitTree(Tree tree) => tree.Statements.Aggregate("", (current, node) => current + Visit(node));

    public string VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        _indent++;
        
        var name = Indented($"name: \"{variableDeclaration.Name.Text}\"\n");
        var type = Indented($"type: {(variableDeclaration.ColonTypeClause != null ? Visit(variableDeclaration.ColonTypeClause.Type) : "none")}\n");
        var initializer = Indented($"initializer: {(variableDeclaration.EqualsValueClause != null ? Visit(variableDeclaration.EqualsValueClause.Value) : "none")}\n");
        
        _indent--;
        return "VariableDeclaration(\n" + name + type + initializer + Indented(")");
    }

    public string VisitLiteral(Literal literal) => $"Literal({literal})";

    public string VisitIdentifier(Identifier identifier) => $"Identifier({identifier})";

    public string VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        _indent++;
        
        var statement = Indented($"expression: {Visit(expressionStatement.Expression)}");
        
        _indent--;
        return "ExpressionStatement(\n" + statement + Indented(")");
    }
    
    private string Indented(string content) => new string(' ', 2 * _indent) + content;
}