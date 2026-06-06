using Loom.Diagnostics;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using BinaryOperator = Loom.Parsing.AST.BinaryOperator;
using ExpressionStatement = Loom.Parsing.AST.ExpressionStatement;
using Identifier = Loom.Parsing.AST.Identifier;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using Parameter = Loom.Parsing.AST.Parameter;
using Parenthesized = Loom.Parsing.AST.Parenthesized;
using ParenthesizedType = Loom.Parsing.AST.ParenthesizedType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Return = Loom.Parsing.AST.Return;
using TypeAlias = Loom.Parsing.AST.TypeAlias;
using TypeName = Loom.Parsing.AST.TypeName;
using TypeParameter = Loom.Parsing.AST.TypeParameter;
using TypeParameters = Loom.Parsing.AST.TypeParameters;
using UnaryOperator = Loom.Parsing.AST.UnaryOperator;
using UnionType = Loom.Parsing.AST.UnionType;

namespace Loom.Generation;

internal class LuauScope(LuauScope? parent = null)
{
    private readonly Dictionary<string, int> _temporaryIds = [];

    public LuauScope? Parent { get; } = parent;
    public List<LuauStatement> PrereqStatements { get; } = [];

    public string AddIdentifier(string name)
    {
        if (TryGetId(name, out var temporaryId))
            return name + "_" + (_temporaryIds[name] = temporaryId + 1);

        _temporaryIds.Add(name, 0);
        return name;
    }

    private bool TryGetId(string name, out int temporaryId) =>
        _temporaryIds.TryGetValue(name, out temporaryId) || Parent != null && Parent.TryGetId(name, out temporaryId);
}

public class LuauGenerator(SemanticModel semanticModel) : Visitor<LuauNode>
{
    private readonly DiagnosticBag _diagnostics = new();
    private LuauScope _scope = new();

    public LuauGeneratorResult Generate()
    {
        var luauTree = VisitTree(semanticModel.Tree);
        return new LuauGeneratorResult(luauTree, _diagnostics);
    }

    public override LuauNode Visit(Node node) => node.Accept(this);
    public override LuauTree VisitTree(Tree tree) => new(GenerateStatements(tree.Statements));
    public override LuauNode VisitReturn(Return @return) => new Luau.AST.Return(Visit(@return.Expression));

    public override LuauNode VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var typeParameters = MaybeVisit<Luau.AST.TypeParameters>(functionDeclaration.TypeParameters);
        var parameters = functionDeclaration.Parameters?.ParameterList.ConvertAll(Visit<Luau.AST.Parameter>) ?? [];
        var returnType = MaybeVisit<LuauType>(functionDeclaration.ReturnType);
        var statements = functionDeclaration.Body switch
        {
            ExpressionBody body => [new Luau.AST.Return(Visit(body.Expression))],
            Block block => GenerateStatements(block.Statements),
            _ => []
        };

        return new Function(functionDeclaration.Name.Text, typeParameters, parameters, returnType, statements);
    }

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

    public override LuauNode VisitParameter(Parameter parameter) => new Luau.AST.Parameter(parameter.Name.Text, MaybeVisit<LuauType>(parameter.ColonTypeClause));

    public override LuauNode VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        var expression = Visit(expressionStatement.Expression);
        return IsUnorphanableExpression(expression)
            ? new Luau.AST.ExpressionStatement(expression)
            : new ConstVariable("_", null, expression);
    }

    public override LuauNode VisitInvocation(Invocation invocation) => new Call(Visit(invocation.Expression), invocation.Arguments.ArgumentList.ConvertAll(Visit));

    public override LuauNode VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Parent is ExpressionStatement)
            return VisitBinaryOperator(assignmentOperator);

        var binary = (Luau.AST.BinaryOperator)VisitBinaryOperator(assignmentOperator);
        var assignmentStatement = new Luau.AST.ExpressionStatement(binary);
        Prereq(assignmentStatement);

        return binary.Left;
    }

    public override LuauNode VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var operand = Visit(unaryOperator.Operand);
        return SyntaxFacts.IsBitwiseOperator(unaryOperator.Operator.Kind)
            ? LuauFactory.Bit32Call("bnot", [operand])
            : new Luau.AST.UnaryOperator(MapLuau.UnaryOperator(unaryOperator.Operator.Text), operand);
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

            var name = MapLuau.BitwiseOperator(op);
            var arguments = new List<LuauExpression>();
            var leftUpdated = AddBit32Arguments(left, name, arguments);
            var rightUpdated = AddBit32Arguments(right, name, arguments);
            if (!leftUpdated)
                arguments.Add(left);

            if (!rightUpdated)
                arguments.Add(right);

            return LuauFactory.Bit32Call(name, arguments);
        }

        var leftType = semanticModel.GetType(binaryOperator.Left);
        var rightType = semanticModel.GetType(binaryOperator.Right);
        var @string = TypeChecking.Types.PrimitiveType.String;
        var isConcatenation = op.StartsWith('+') && leftType.IsAssignableTo(@string) && rightType.IsAssignableTo(@string);
        var mappedOperator = isConcatenation ? op.Replace("+", "..") : MapLuau.BinaryOperator(op);
        return new Luau.AST.BinaryOperator(left, mappedOperator, right);
    }

    public override LuauNode VisitParenthesized(Parenthesized parenthesized) => new Luau.AST.Parenthesized(Visit(parenthesized.Expression));

    public override LuauNode VisitLiteral(Literal literal) =>
        literal.Value switch
        {
            long l => new NumberLiteral(l),
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
    public override LuauNode VisitParenthesizedType(ParenthesizedType parenthesized) => new Luau.AST.ParenthesizedType(Visit(parenthesized.Type));

    public override LuauNode VisitTypeName(TypeName typeName)
    {
        var typeArguments = typeName.TypeArguments?.ArgumentsList.ConvertAll(Visit);
        return new Luau.AST.TypeName(typeName.Name.Text, typeArguments);
    }

    public override LuauNode VisitTypeParameters(TypeParameters typeParameters) =>
        new Luau.AST.TypeParameters(typeParameters.ParameterList.ConvertAll(Visit).Cast<Luau.AST.TypeParameter>().ToList());

    public override LuauNode VisitTypeParameter(TypeParameter typeParameter) =>
        new Luau.AST.TypeParameter(typeParameter.Name.Text, typeParameter.EqualsTypeClause != null ? Visit(typeParameter.EqualsTypeClause.Type) : null);

    public override LuauNode VisitPrimitiveType(PrimitiveType primitiveType) => new Luau.AST.PrimitiveType(MapLuau.PrimitiveTypeKind(primitiveType.Kind));

    public override LuauNode VisitLiteralType(LiteralType literalType) =>
        literalType.Value switch
        {
            long or int or double => Luau.AST.PrimitiveType.Number,
            bool b => new BooleanLiteralType(b),
            string s => new StringLiteralType(s),
            _ => Luau.AST.PrimitiveType.Nil
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

    private List<LuauStatement> GenerateStatements(List<Statement> statements)
    {
        var result = new List<LuauStatement>();
        foreach (var statement in statements)
        {
            var (luauStatement, scope) = Capture(() => Visit(statement));
            result.AddRange(scope.PrereqStatements);
            result.Add(luauStatement);
        }

        return result;
    }

    private (T, LuauScope) Capture<T>(Func<T> callback)
    {
        T value = default!;
        var scope = CaptureScope(() => value = callback());
        return (value, scope);
    }

    private LuauScope CaptureScope(Action callback)
    {
        var captured = new LuauScope(_scope);
        _scope = captured;
        callback();
        _scope = _scope.Parent!;

        return captured;
    }

    private Luau.AST.Identifier PushToVariable(LuauExpression expression, string name)
    {
        if (expression is Luau.AST.Identifier identifier)
            return identifier;

        var id = _scope.AddIdentifier(name);
        Prereq(new LocalVariable(id, null, expression));
        return new Luau.AST.Identifier(id);
    }

    private void Prereq(params LuauStatement[] statements) => _scope.PrereqStatements.AddRange(statements);

    private static bool IsUnorphanableExpression(LuauExpression expression) =>
        expression is Call
        || expression is Luau.AST.BinaryOperator binaryOperator && binaryOperator.Operator.EndsWith('=');

    private LuauType Visit(TypeExpression node) => (LuauType)node.Accept(this);
    private LuauExpression Visit(Expression node) => (LuauExpression)node.Accept(this);
    private LuauStatement Visit(Statement node) => (LuauStatement)node.Accept(this);
}