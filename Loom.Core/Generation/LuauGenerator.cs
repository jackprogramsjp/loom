using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Text;
using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using ArrayType = Loom.TypeChecking.Types.ArrayType;
using BinaryOperator = Loom.Parsing.AST.BinaryOperator;
using ElementAccess = Loom.Parsing.AST.ElementAccess;
using Expression = Loom.Parsing.AST.Expression;
using ExpressionStatement = Loom.Parsing.AST.ExpressionStatement;
using Identifier = Loom.Parsing.AST.Identifier;
using PropertyAccess = Loom.Parsing.AST.PropertyAccess;
using TypeAlias = Loom.Parsing.AST.TypeAlias;
using TypeName = Loom.Parsing.AST.TypeName;
using TypeParameter = Loom.Parsing.AST.TypeParameter;
using UnaryOperator = Loom.Parsing.AST.UnaryOperator;

namespace Loom.Generation;

public sealed partial class LuauGenerator(SemanticModel semanticModel)
    : Visitor<LuauNode>(_ => new NoOpStatement())
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Macros _macros = new(semanticModel);
    private LuauScope _scope = new();

    public LuauGeneratorResult Generate()
    {
        var luauTree = VisitTree(semanticModel.Tree);
        return new LuauGeneratorResult(luauTree, _diagnostics);
    }

    protected override LuauNode Visit(Node node) => node.Accept(this);

    public override LuauNode VisitFor(For @for)
    {
        var names = @for.Names.ConvertAll(n => n.Token.Text);
        var body = GenerateChunk(@for.Body);
        var collectionType = semanticModel.GetType(@for.CollectionExpression);
        var collectionExpression = Visit(@for.CollectionExpression);
        if (names.Count == 2 && collectionType is ArrayType)
        {
            names.Reverse();
            return new ForStatement(names, collectionExpression, body);
        }

        if (!collectionType.Equals(Intrinsics.Range))
            return collectionType is ObjectType or InterfaceType
                ? new ForStatement(names.Count == 1 ? ["_", names[0]] : names, collectionExpression, body)
                : new ForStatement(names, collectionExpression, body);

        LuauExpression start;
        LuauExpression end;
        LuauExpression? incrementBy;
        var one = new NumberLiteral(1);
        var negativeOne = new Luau.AST.UnaryOperator("-", one);
        if (@for.CollectionExpression is RangeLiteral range)
        {
            start = Visit(range.Minimum);
            end = Visit(range.Maximum);
            incrementBy = start is NumberLiteral { Value: var minimum } && end is NumberLiteral { Value: var maximum }
                ? maximum < minimum ? negativeOne : null
                : new IfExpression(new Luau.AST.BinaryOperator(end, "<", start), negativeOne, [], one);
        }
        else
        {
            var rangeIdentifier = PushToVariable("_range", collectionExpression);
            var minimum = new Luau.AST.PropertyAccess(rangeIdentifier, ["minimum"]);
            var maximum = new Luau.AST.PropertyAccess(rangeIdentifier, ["maximum"]);
            start = minimum;
            end = maximum;
            incrementBy = new IfExpression(new Luau.AST.BinaryOperator(end, "<", start), negativeOne, [], one);
        }

        return new NumericForStatement(names.First(), start, end, incrementBy, body);
    }

    public override IfStatement VisitIf(If @if)
    {
        var condition = Visit(@if.Condition);
        var thenBranch = GenerateChunk(@if.ThenBranch);
        var elseBranch = @if.ElseBranch != null ? GenerateChunk(@if.ElseBranch.Branch) : null;
        var elseIfBranches = new List<ElseIfBranch>();
        if (@if.ElseBranch is not { Branch: If elseIf })
            return new IfStatement(condition, thenBranch, elseIfBranches, elseBranch);

        var luauElseIf = VisitIf(elseIf);
        elseBranch = luauElseIf.ElseBranch;
        elseIfBranches.Add(new ElseIfBranch(luauElseIf.Condition, luauElseIf.ThenBranch));
        elseIfBranches.AddRange(luauElseIf.ElseIfBranches);

        return new IfStatement(condition, thenBranch, elseIfBranches, elseBranch);
    }

    public override LuauNode VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var typeParameters = MaybeVisit<Luau.AST.TypeParameters>(functionDeclaration.TypeParameters);
        if (typeParameters != null)
            foreach (var typeParameter in typeParameters.Parameters)
                typeParameter.OfFunction = true;

        var parameters = functionDeclaration.Parameters?.ParameterList.ConvertAll(Visit<Luau.AST.Parameter>) ?? [];
        var returnType = MaybeVisit<LuauType>(functionDeclaration.ReturnType);
        var statements = functionDeclaration.Body is ExpressionBody expressionBody
            ? new Chunk(GenerateStatements(expressionBody.Expression))
            : GenerateChunk(functionDeclaration.Body);

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

    public override LuauNode VisitEnumDeclaration(EnumDeclaration enumDeclaration)
    {
        if (semanticModel.GetType(enumDeclaration) is not ObjectType objectType)
            return LuauFactory.EmptyVariable();

        var propertyUnion = objectType.PropertyUnion();
        return new Luau.AST.TypeAlias(
            enumDeclaration.Name.Text,
            new Luau.AST.TypeParameters(),
            propertyUnion switch
            {
                TypeChecking.Types.UnionType union =>
                    union.Types.Any(t => t.IsAssignableTo(TypeChecking.Types.PrimitiveType.Number))
                        ? Luau.AST.PrimitiveType.Number
                        : new Luau.AST.UnionType(
                            union.Types.ConvertAll(t => t is TypeChecking.Types.LiteralType { Value: string s }
                                    ? new StringLiteralType(s)
                                    : Luau.AST.PrimitiveType.Number
                                )
                                .OfType<LuauType>()
                                .ToList()
                        ),
                TypeChecking.Types.LiteralType { Value: string s } => new StringLiteralType(s),
                _ => Luau.AST.PrimitiveType.Number
            }
        );
    }

    public override LuauNode VisitInterfaceDeclaration(InterfaceDeclaration interfaceDeclaration)
    {
        var indexer = interfaceDeclaration.Body?.Members.OfType<IndexerDeclaration>().FirstOrDefault();
        var propertyDeclarations = interfaceDeclaration.Body?.Members.OfType<PropertyDeclaration>() ?? [];
        var tableIndexer = indexer != null
            ? new TableTypeIndexer(indexer.MutKeyword == null ? LuauVisibility.Read : null, Visit(indexer.IndexType), Visit(indexer.ColonTypeClause))
            : null;

        var properties = propertyDeclarations.Select(p => new TableTypeProperty(
                    p.MutKeyword == null ? LuauVisibility.Read : null,
                    p.Name.Text,
                    Visit(p.ColonTypeClause)
                )
            )
            .ToList();

        var tableType = new TableType(tableIndexer, properties);
        var typeParameters = MaybeVisit<Luau.AST.TypeParameters>(interfaceDeclaration.TypeParameters) ?? new Luau.AST.TypeParameters();
        return new Luau.AST.TypeAlias(
            interfaceDeclaration.Name.Text,
            typeParameters,
            interfaceDeclaration.ColonTypeListClause != null
                ? new Luau.AST.IntersectionType([..interfaceDeclaration.ColonTypeListClause.Types.ConvertAll(Visit), tableType])
                : tableType
        );
    }

    public override LuauNode VisitQualifiedName(QualifiedName qualifiedName) =>
        TryGetEnumConstant(qualifiedName, out var enumValue)
            ? enumValue
            : new Luau.AST.PropertyAccess(Visit(qualifiedName.Identifier), qualifiedName.Names.ConvertAll(dotName => dotName.Name.Text));

    public override LuauNode VisitPropertyAccess(PropertyAccess propertyAccess) =>
        TryGetEnumConstant(propertyAccess, out var enumValue)
            ? enumValue
            : new Luau.AST.PropertyAccess(Visit(propertyAccess.Expression), propertyAccess.Names.ConvertAll(dotName => dotName.Name.Text));

    public override LuauNode VisitElementAccess(ElementAccess elementAccess)
    {
        var target = Visit(elementAccess.Expression);
        var targetType = semanticModel.GetType(elementAccess.Expression);
        var indexType = semanticModel.GetType(elementAccess.IndexExpression);
        if (!indexType.Equals(Intrinsics.Range))
        {
            if (TryGetEnumConstant(elementAccess, out var enumValue))
                return enumValue;

            var index = Visit(elementAccess.IndexExpression);
            if (targetType.IsAssignableTo(TypeChecking.Types.PrimitiveType.String) && indexType.IsAssignableTo(TypeChecking.Types.PrimitiveType.Number))
                return LuauFactory.StringCall("sub", [target, index, index]);

            return new Luau.AST.ElementAccess(target, index);
        }

        var one = new NumberLiteral(1);
        var length = PushToVariable("_length", new Luau.AST.UnaryOperator("#", target));
        Call? minimum, maximum;

        if (elementAccess.IndexExpression is RangeLiteral literal)
        {
            var rangeTable = Visit<Table>(literal);
            var properties = rangeTable.Initializers.OfType<PropertyTableInitializer>().ToList();
            var minimumLiteral = properties.First(p => p.PropertyName == "minimum").Value;
            var maximumLiteral = properties.First(p => p.PropertyName == "maximum").Value;
            minimum = LuauFactory.MathCall("clamp", [minimumLiteral, one, length]);
            maximum = LuauFactory.MathCall("clamp", [maximumLiteral, one, length]);
        }
        else
        {
            var index = Visit(elementAccess.IndexExpression);
            var range = PushToVariable("_range", index);
            minimum = LuauFactory.MathCall("clamp", [new Luau.AST.PropertyAccess(range, ["minimum"]), one, length]);
            maximum = LuauFactory.MathCall("clamp", [new Luau.AST.PropertyAccess(range, ["maximum"]), one, length]);
        }

        return targetType.IsAssignableTo(TypeChecking.Types.PrimitiveType.String)
            ? LuauFactory.StringCall("sub", [target, minimum, maximum])
            : LuauFactory.TableCall("move", [target, minimum, maximum, one, new Table([])]);
    }

    public override LuauNode VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Parent is ExpressionStatement)
            return VisitBinaryOperator(assignmentOperator);

        if (assignmentOperator.Left is Identifier)
        {
            var binary = (Luau.AST.BinaryOperator)VisitBinaryOperator(assignmentOperator);
            var assignmentStatement = new Luau.AST.ExpressionStatement(binary);
            Prereq(assignmentStatement);
            return binary.Left;
        }

        var left = Visit(assignmentOperator.Left);
        var right = Visit(assignmentOperator.Right);
        if (assignmentOperator.Parent is EqualsValueClause { Parent: NamedDeclaration declaration })
        {
            var identifierAssignment = new Luau.AST.BinaryOperator(left, "=", new Luau.AST.Identifier(declaration.Name.Text));
            Postreq(new Luau.AST.ExpressionStatement(identifierAssignment));
            return right;
        }

        var assigned = PushToVariable("_assigned", right);
        var boundAssignment = new Luau.AST.BinaryOperator(left, "=", assigned);
        Prereq(new Luau.AST.ExpressionStatement(boundAssignment));
        return assigned;
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
                _diagnostics.NotImplemented(
                    binaryOperator,
                    "Luau generation for bitwise assignment operators is not yet supported.",
                    $"use '{binaryOperator.Left} = {binaryOperator.Left} {op.Replace("=", "")} {binaryOperator.Right}'"
                );

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

    public override LuauNode VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var operand = Visit(unaryOperator.Operand);
        return SyntaxFacts.IsBitwiseOperator(unaryOperator.Operator.Kind)
            ? LuauFactory.Bit32Call("bnot", [operand])
            : new Luau.AST.UnaryOperator(MapLuau.UnaryOperator(unaryOperator.Operator.Text), operand);
    }

    public override LuauNode VisitTypeName(TypeName typeName)
    {
        var symbol = semanticModel.GetSymbol(typeName);
        if (symbol == null)
        {
            _diagnostics.Error(typeName, InternalCodes.CannotFindSymbol, $"Cannot find symbol for type '{typeName}'");
            return new NilLiteral();
        }

        var constraint = symbol.Declaration is TypeParameter { ColonTypeClause: { } clause } ? Visit(clause) : null;
        var typeArguments = typeName.TypeArguments?.ArgumentsList.ConvertAll(Visit);
        var luauTypeName = new Luau.AST.TypeName(typeName.Name.Text, typeArguments);
        return constraint != null ? new Luau.AST.IntersectionType([luauTypeName, constraint]) : luauTypeName;
    }

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

    private bool TryGetEnumConstant(Node node, [MaybeNullWhen(false)] out LuauNode constantType)
    {
        constantType = null;
        var type = semanticModel.GetType(node);
        if (type is not TypeChecking.Types.LiteralType { Value: long or int or double or string } literal)
            return false;

        constantType = literal.Value is string s ? new StringLiteral(s) : new NumberLiteral(Convert.ToDouble(literal.Value));
        return true;
    }

    private Chunk GenerateChunk(Statement statement) => statement is Block block ? VisitBlock(block) : new Chunk(GenerateStatements(statement));

    private List<LuauStatement> GenerateStatements(Expression expression)
    {
        var result = new List<LuauStatement>();
        var (luauExpression, scope) = Capture(() => Visit(expression));
        result.AddRange(scope.PrereqStatements);
        result.Add(new Luau.AST.Return(luauExpression));
        result.AddRange(scope.PostreqStatements);

        return result.FindAll(s => s is not NoOpStatement);
    }

    private List<LuauStatement> GenerateStatements(Statement statement)
    {
        var result = new List<LuauStatement>();
        var (luauStatement, scope) = Capture(() => Visit(statement));
        result.AddRange(scope.PrereqStatements);
        result.Add(luauStatement);
        result.AddRange(scope.PostreqStatements);

        return result.FindAll(s => s is not NoOpStatement);
    }

    private List<LuauStatement> GenerateStatements(List<Statement> statements)
    {
        var result = new List<LuauStatement>();
        foreach (var statement in statements)
            result.AddRange(GenerateStatements(statement));

        return result.FindAll(s => s is not NoOpStatement);
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

    private Luau.AST.Identifier PushToVariable(string name, LuauExpression expression, LuauType? type = null, bool isConst = true)
    {
        if (expression is Luau.AST.Identifier identifier)
            return identifier;

        var id = _scope.AddIdentifier(name);
        Prereq(isConst ? new ConstVariable(id, type, expression) : new LocalVariable(id, type, expression));
        return new Luau.AST.Identifier(id);
    }

    private void Prereq(params LuauStatement[] statements) => _scope.PrereqStatements.AddRange(statements);
    private void Postreq(params LuauStatement[] statements) => _scope.PostreqStatements.AddRange(statements);

    private static LuauStatement WrapExpressionAsStatement(LuauExpression expression) =>
        IsUnorphanableExpression(expression)
            ? new Luau.AST.ExpressionStatement(expression)
            : new ConstVariable("_", null, expression);

    private static bool IsUnorphanableExpression(LuauExpression expression) =>
        expression is Call
        || expression is Luau.AST.BinaryOperator binaryOperator && binaryOperator.Operator.EndsWith('=') && binaryOperator.Operator is not ("==" or "~=");

    private LuauType Visit(TypeExpression node) => (LuauType)node.Accept(this);
    private LuauExpression Visit(Expression node) => (LuauExpression)node.Accept(this);
    private LuauStatement Visit(Statement node) => (LuauStatement)node.Accept(this);
}