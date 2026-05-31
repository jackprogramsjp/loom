using Loom.Diagnostics;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.Syntax;
using BinaryOperator = Loom.Parsing.AST.BinaryOperator;
using ExpressionStatement = Loom.Parsing.AST.ExpressionStatement;
using Identifier = Loom.Parsing.AST.Identifier;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using Parenthesized = Loom.Parsing.AST.Parenthesized;
using ParenthesizedType = Loom.Parsing.AST.ParenthesizedType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using PrimitiveTypeKind = Loom.TypeChecking.Types.PrimitiveTypeKind;
using TypeAlias = Loom.Parsing.AST.TypeAlias;
using TypeName = Loom.Parsing.AST.TypeName;
using TypeParameter = Loom.Parsing.AST.TypeParameter;
using TypeParameters = Loom.Parsing.AST.TypeParameters;
using UnaryOperator = Loom.Parsing.AST.UnaryOperator;
using UnionType = Loom.Parsing.AST.UnionType;

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

    public override LuauTree VisitTree(Tree t) => new(t.Statements.ConvertAll(Visit));

    public override LuauNode VisitTypeAlias(TypeAlias typeAlias)
    {
        var typeParameters = typeAlias.TypeParameters != null 
            ? Visit<Luau.AST.TypeParameters>(typeAlias.TypeParameters) 
            : new Luau.AST.TypeParameters();

        var type = Visit(typeAlias.EqualsTypeClause.Type);
        return new Luau.AST.TypeAlias(typeAlias.Name.Text, typeParameters, type);
    }

    public override LuauNode VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var isConst = variableDeclaration.Keyword is { Kind: SyntaxKind.LetKeyword };
        var name = variableDeclaration.Name.Text;
        var type = variableDeclaration.ColonTypeClause != null ? Visit(variableDeclaration.ColonTypeClause.Type) : null;
        var initializer = variableDeclaration.EqualsValueClause != null ? Visit(variableDeclaration.EqualsValueClause.Value) : null;
        return isConst
            ? new ConstVariable(name, type, initializer!)
            : new LocalVariable(name, type, initializer);
    }

    public override LuauNode VisitExpressionStatement(ExpressionStatement expressionStatement) =>
        new Luau.AST.ExpressionStatement(Visit(expressionStatement.Expression));

    public override LuauNode VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var operand = Visit(unaryOperator.Operand);
        if (SyntaxFacts.IsBitwiseOperator(unaryOperator.Operator.Kind))
        {
            return LuauFactory.Bit32Call("bnot", [operand]);
        }

        var mappedOperator = MapUnaryOperator(unaryOperator.Operator.Text);
        return new Luau.AST.UnaryOperator(mappedOperator, operand);
    }

    public override LuauNode VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        var left = Visit(binaryOperator.Left);
        var right = Visit(binaryOperator.Right);
        var op = binaryOperator.Operator.Text;
        if (SyntaxFacts.IsBitwiseOperator(binaryOperator.Operator.Kind))
        {
            if (op.EndsWith('='))
            {
                _diagnostics.NotImplemented(binaryOperator, "Luau generation for bitwise assignment operators is not yet supported.");
                return new Luau.AST.BinaryOperator(left, "???", right);
            }

            var name = MapBitwiseOperator(op);
            var arguments = new List<LuauExpression>();
            var leftUpdated = AddBit32Arguments(left, name, arguments);
            var rightUpdated = AddBit32Arguments(right, name, arguments);
            if (!leftUpdated)
                arguments.Add(left);

            if (!rightUpdated)
                arguments.Add(right);

            return LuauFactory.Bit32Call(name, arguments);
        }

        var mappedOperator = MapBinaryOperator(op);
        return new Luau.AST.BinaryOperator(left, mappedOperator, right);
    }

    public override LuauNode VisitParenthesized(Parenthesized parenthesized) => new Luau.AST.Parenthesized(Visit(parenthesized.Expression));

    public override LuauNode VisitLiteral(Literal literal) =>
        literal.Value switch
        {
            int i => new NumberLiteral(i),
            double d => new NumberLiteral(d),
            string s => new StringLiteral(s),
            bool b => new BooleanLiteral(b),
            _ => new NilLiteral()
        };

    public override LuauNode VisitIdentifier(Identifier identifier) => new Luau.AST.Identifier(identifier.Name.Text);

    public override LuauNode VisitIntersectionType(IntersectionType intersectionType) => new Luau.AST.IntersectionType(intersectionType.Types.ConvertAll(Visit));

    public override LuauNode VisitUnionType(UnionType unionType) => new Luau.AST.UnionType(unionType.Types.ConvertAll(Visit));

    public override LuauNode VisitOptionalType(OptionalType optionalType) => new Luau.AST.OptionalType(Visit(optionalType.NonNullableType));

    public override LuauNode VisitTypeName(TypeName typeName)
    {
        var typeArguments = typeName.TypeArguments?.Arguments.ConvertAll(Visit);
        return new Luau.AST.TypeName(typeName.Name.Text, typeArguments);
    }

    public override LuauNode VisitTypeParameters(TypeParameters typeParameters) =>
        new Luau.AST.TypeParameters(typeParameters.Parameters.ConvertAll(Visit).Cast<Luau.AST.TypeParameter>().ToList());

    public override LuauNode VisitTypeParameter(TypeParameter typeParameter) =>
        new Luau.AST.TypeParameter(typeParameter.Name.Text, typeParameter.EqualsTypeClause != null ? Visit(typeParameter.EqualsTypeClause.Type) : null);

    public override LuauNode VisitParenthesizedType(ParenthesizedType parenthesized) => new Luau.AST.ParenthesizedType(Visit(parenthesized.Type));

    public override LuauNode VisitPrimitiveType(PrimitiveType primitiveType) => new Luau.AST.PrimitiveType(MapPrimitiveTypeKind(primitiveType.Kind));

    private static Luau.AST.PrimitiveTypeKind MapPrimitiveTypeKind(PrimitiveTypeKind kind) =>
        kind switch
        {
            PrimitiveTypeKind.Number => Luau.AST.PrimitiveTypeKind.Number,
            PrimitiveTypeKind.String => Luau.AST.PrimitiveTypeKind.String,
            PrimitiveTypeKind.Bool => Luau.AST.PrimitiveTypeKind.Boolean,
            PrimitiveTypeKind.Unknown => Luau.AST.PrimitiveTypeKind.Unknown,
            PrimitiveTypeKind.Never => Luau.AST.PrimitiveTypeKind.Never,
            _ => Luau.AST.PrimitiveTypeKind.Nil,
        };

    private static string MapBitwiseOperator(string op) =>
        op switch
        {
            "&" => "band",
            "|" => "bor",
            "~" => "bxor",
            "<<" => "lshift",
            ">>>" => "rshift",
            ">>" => "arshift",
            _ => op
        };

    private static string MapBinaryOperator(string op) =>
        op switch
        {
            "&&" => "and",
            "||" or "??" => "or",
            _ => op
        };

    private static string MapUnaryOperator(string op) =>
        op switch
        {
            "!" => "not ",
            _ => op
        };

    private static bool AddBit32Arguments(LuauExpression expression, string name, List<LuauExpression> arguments)
    {
        if (expression is not Call
            {
                Callee: Luau.AST.PropertyAccess { Target: Luau.AST.Identifier { Name: "bit32" }, Names: [{ } fnName] }, Arguments: { } fnArguments
            }
            || fnName != name)
        {
            return false;
        }

        // shift methods dont accept varargs
        if (fnName.EndsWith("shift"))
            return false;

        foreach (var argument in fnArguments)
        {
            arguments.Add(argument);
            AddBit32Arguments(argument, name, arguments);
        }

        return true;
    }
    
    private T Visit<T>(Node node) where T : LuauNode => (T)Visit(node);
    private LuauType Visit(TypeExpression node) => (LuauType)node.Accept(this);
    private LuauExpression Visit(Expression node) => (LuauExpression)node.Accept(this);
    private LuauStatement Visit(Statement node) => (LuauStatement)node.Accept(this);
}