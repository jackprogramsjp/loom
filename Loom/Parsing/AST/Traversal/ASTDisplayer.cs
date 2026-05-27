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
        var type = Indented($"type: {(variableDeclaration.ColonTypeClause != null ? Visit(variableDeclaration.ColonTypeClause) : "none")}\n");
        var initializer = Indented($"initializer: {(variableDeclaration.EqualsValueClause != null ? Visit(variableDeclaration.EqualsValueClause) : "none")}\n");
        
        _indent--;
        return "VariableDeclaration(\n" + name + type + initializer + Indented(")");
    }

    public string VisitLiteral(Literal literal) => $"Literal({literal})";

    public string VisitIdentifier(Identifier identifier) => $"Identifier({identifier})";
    

    public string VisitParenthesized(Parenthesized parenthesized)
    {
        _indent++;
        
        var expression = Indented($"expression: {Visit(parenthesized.Expression)}\n");
        
        _indent--;
        return "Parenthesized(\n" + expression + Indented(")");
    }

    public string VisitBinaryOperator(BinaryOperator binaryOperator)
        {
        _indent++;
        
        var op = Indented($"operator: {binaryOperator.Operator.Text}\n");
        var left = Indented($"left: {Visit(binaryOperator.Left)}\n");
        var right = Indented($"right: {Visit(binaryOperator.Right)}\n");
        
        _indent--;
        return "BinaryOperator(\n" + op + left + right + Indented(")");
    }

    public string VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        _indent++;
        
        var op = Indented($"operator: {unaryOperator.Operator.Text}\n");
        var operand = Indented($"operand: {Visit(unaryOperator.Operand)}\n");
        
        _indent--;
        return "UnaryOperator(\n" + op + operand + Indented(")");
    }

    public string VisitColonTypeClause(ColonTypeClause colonTypeClause)
    {
        _indent++;
        
        var type = Indented($"type: {Visit(colonTypeClause.Type)}\n");
        
        _indent--;
        return "ColonTypeClause(\n" + type + Indented(")");
    }

    public string VisitEqualsValueClause(EqualsValueClause equalsValueClause)
    {
        _indent++;
        
        var value = Indented($"value: {Visit(equalsValueClause.Value)}\n");
        
        _indent--;
        return "EqualsValueClause(\n" + value + Indented(")");
    }

    public string VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        _indent++;
        
        var expression = Indented($"expression: {Visit(expressionStatement.Expression)}\n");
        
        _indent--;
        return "ExpressionStatement(\n" + expression + Indented(")");
    }
    
    private string Indented(string content) => new string(' ', 2 * _indent) + content;
}