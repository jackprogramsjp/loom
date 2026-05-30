using Loom.Diagnostics;
using Loom.Luau;
using Loom.Parsing.AST;
using Loom.Syntax;
using BinaryOperator = Loom.Parsing.AST.BinaryOperator;
using ExpressionStatement = Loom.Parsing.AST.ExpressionStatement;
using UnaryOperator = Loom.Parsing.AST.UnaryOperator;

namespace Loom;

public class LuauGenerator(Tree tree) : Visitor<LuauNode>
{
    private readonly DiagnosticBag _diagnostics = new();

    public LuauGeneratorResult Generate()
    {
        var luauTree = VisitTree(tree);
        return new LuauGeneratorResult(luauTree, _diagnostics);
    }

    public override LuauNode Visit(Node node) => node.Accept(this);
    public LuauExpression Visit(Expression node) => (LuauExpression)node.Accept(this);
    public LuauStatement Visit(Statement node) => (LuauStatement)node.Accept(this);

    public override LuauTree VisitTree(Tree t) => new(t.Statements.ConvertAll(Visit));

    public override LuauNode VisitExpressionStatement(ExpressionStatement expressionStatement) =>
        new Luau.ExpressionStatement(Visit(expressionStatement.Expression));

    public override LuauNode VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var operand = Visit(unaryOperator.Operand);
        if (SyntaxFacts.IsBitwiseOperator(unaryOperator.Operator.Kind))
        {
            _diagnostics.NotImplemented(unaryOperator, "Luau generation for bitwise operators is not yet supported.");
            return new Luau.UnaryOperator("???", operand);
        }

        var mappedOperator = MapUnaryOperator(unaryOperator.Operator.Text);
        return new Luau.UnaryOperator(mappedOperator, operand);
    }

    public override LuauNode VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        var left = Visit(binaryOperator.Left);
        var right = Visit(binaryOperator.Right);
        if (SyntaxFacts.IsBitwiseOperator(binaryOperator.Operator.Kind))
        {
            _diagnostics.NotImplemented(binaryOperator, "Luau generation for bitwise operators is not yet supported.");
            return new Luau.BinaryOperator(left, "???", right);
        }

        var mappedOperator = MapBinaryOperator(binaryOperator.Operator.Text);
        return new Luau.BinaryOperator(left, mappedOperator, right);
    }

    public override LuauNode VisitLiteral(Literal literal) =>
        literal.Value switch
        {
            int i => new NumberLiteral(i),
            double d => new NumberLiteral(d),
            string s => new StringLiteral(s),
            bool b => new BooleanLiteral(b),
            _ => new NilLiteral()
        };

    public override LuauNode VisitIdentifier(Identifier identifier) => throw new NotImplementedException();

    public override LuauNode VisitTypeName(TypeName typeName) => throw new NotImplementedException();

    public override LuauNode VisitPrimitiveType(PrimitiveType primitiveType) => throw new NotImplementedException();

    private static string MapBinaryOperator(string op) =>
        op switch
        {
            "&&" => "and",
            "||" => "or",
            _ => op
        };

    private static string MapUnaryOperator(string op) =>
        op switch
        {
            "!" => "not",
            _ => op
        };
}