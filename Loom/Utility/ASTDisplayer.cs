using Loom.Parsing.AST;

namespace Loom.Utility;

public class ASTDisplayer(Tree ast) : Visitor<string>
{
    private int _indent;
    
    public void Display()
    {
        var content = VisitTree(ast);
        Console.WriteLine(content);
    }
    
    public override string Visit(Node node) => node.Accept(this);

    public new string VisitTree(Tree tree) => string.Join('\n', tree.Statements.Select(Visit));

    public override string VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        _indent++;

        var name = Indented($"name: \"{variableDeclaration.Name.Text}\"\n");
        var type = Indented($"type: {(variableDeclaration.ColonTypeClause != null ? Visit(variableDeclaration.ColonTypeClause) : "none")}\n");
        var initializer = Indented($"initializer: {(variableDeclaration.EqualsValueClause != null ? Visit(variableDeclaration.EqualsValueClause) : "none")}\n");

        _indent--;
        return "VariableDeclaration(\n" + name + type + initializer + Indented(")");
    }

    public override string VisitLiteral(Literal literal) => $"Literal({literal})";

    public override string VisitIdentifier(Identifier identifier) => $"Identifier({identifier})";

    public override string VisitParenthesized(Parenthesized parenthesized)
    {
        _indent++;

        var expression = Indented($"expression: {Visit(parenthesized.Expression)}\n");

        _indent--;
        return "Parenthesized(\n" + expression + Indented(")");
    }

    public override string VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        _indent++;

        var op = Indented($"operator: {binaryOperator.Operator.Text}\n");
        var left = Indented($"left: {Visit(binaryOperator.Left)}\n");
        var right = Indented($"right: {Visit(binaryOperator.Right)}\n");

        _indent--;
        return "BinaryOperator(\n" + op + left + right + Indented(")");
    }

    public override string VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        _indent++;

        var op = Indented($"operator: {unaryOperator.Operator.Text}\n");
        var operand = Indented($"operand: {Visit(unaryOperator.Operand)}\n");

        _indent--;
        return "UnaryOperator(\n" + op + operand + Indented(")");
    }

    public override string VisitTypeName(TypeName typeName) => $"TypeName({typeName})";

    public override string VisitPrimitiveType(PrimitiveType primitiveType) => $"PrimitiveType({primitiveType})";

    public override string VisitOptionalType(OptionalType optionalType)
    {
        _indent++;

        var type = Indented($"requiredType: {Visit(optionalType.NonNullableType)}\n");

        _indent--;
        return "OptionalType(\n" + type + Indented(")");
    }

    public override string VisitColonTypeClause(ColonTypeClause colonTypeClause)
    {
        _indent++;

        var type = Indented($"type: {Visit(colonTypeClause.Type)}\n");

        _indent--;
        return "ColonTypeClause(\n" + type + Indented(")");
    }

    public override string VisitEqualsValueClause(EqualsValueClause equalsValueClause)
    {
        _indent++;

        var value = Indented($"value: {Visit(equalsValueClause.Value)}\n");

        _indent--;
        return "EqualsValueClause(\n" + value + Indented(")");
    }

    public override string VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        _indent++;

        var expression = Indented($"expression: {Visit(expressionStatement.Expression)}\n");

        _indent--;
        return "ExpressionStatement(\n" + expression + Indented(")");
    }

    private string Indented(string content) => new string(' ', 2 * _indent) + content;
}