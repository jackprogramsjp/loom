using System.Diagnostics.CodeAnalysis;
using Loom.Diagnostics;
using Loom.Luau;
using Loom.Luau.AST;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using ArrayType = Loom.Parsing.AST.ArrayType;
using BinaryOperator = Loom.Parsing.AST.BinaryOperator;
using Break = Loom.Parsing.AST.Break;
using Continue = Loom.Parsing.AST.Continue;
using ElementAccess = Loom.Parsing.AST.ElementAccess;
using Expression = Loom.Parsing.AST.Expression;
using ExpressionStatement = Loom.Parsing.AST.ExpressionStatement;
using FunctionType = Loom.Parsing.AST.FunctionType;
using Identifier = Loom.Parsing.AST.Identifier;
using IntersectionType = Loom.Parsing.AST.IntersectionType;
using LiteralType = Loom.Parsing.AST.LiteralType;
using OptionalType = Loom.Parsing.AST.OptionalType;
using Parameter = Loom.Parsing.AST.Parameter;
using Parenthesized = Loom.Parsing.AST.Parenthesized;
using ParenthesizedType = Loom.Parsing.AST.ParenthesizedType;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using PrimitiveTypeKind = Loom.TypeChecking.Types.PrimitiveTypeKind;
using PropertyAccess = Loom.Parsing.AST.PropertyAccess;
using Return = Loom.Parsing.AST.Return;
using TypeAlias = Loom.Parsing.AST.TypeAlias;
using TypeName = Loom.Parsing.AST.TypeName;
using TypeParameter = Loom.Parsing.AST.TypeParameter;
using TypeParameters = Loom.Parsing.AST.TypeParameters;
using UnaryOperator = Loom.Parsing.AST.UnaryOperator;
using UnionType = Loom.Parsing.AST.UnionType;

namespace Loom.Generation;

public sealed class LuauGenerator(SemanticModel semanticModel)
    : Visitor<LuauNode>(new NoOpStatement())
{
    private readonly DiagnosticBag _diagnostics = new();
    private LuauScope _scope = new();

    public LuauGeneratorResult Generate()
    {
        var luauTree = VisitTree(semanticModel.Tree);
        return new LuauGeneratorResult(luauTree, _diagnostics);
    }

    protected override LuauNode Visit(Node node) => node.Accept(this);
    public override LuauTree VisitTree(Tree tree) => new(GenerateStatements(tree.Statements));
    public override LuauNode VisitBreak(Break @break) => new Luau.AST.Break();
    public override LuauNode VisitContinue(Continue @continue) => new Luau.AST.Continue();
    public override LuauNode VisitWhile(While @while) => new WhileStatement(Visit(@while.Condition), GenerateChunk(@while.Body));

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

    public override Chunk VisitBlock(Block block) => new(GenerateStatements(block.Statements));
    public override LuauNode VisitParameter(Parameter parameter) => new Luau.AST.Parameter(parameter.Name.Text, MaybeVisit<LuauType>(parameter.ColonTypeClause));
    public override LuauNode VisitReturn(Return @return) => new Luau.AST.Return(Visit(@return.Expression));

    public override LuauNode VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var typeParameters = MaybeVisit<Luau.AST.TypeParameters>(functionDeclaration.TypeParameters);
        if (typeParameters != null)
            foreach (var typeParameter in typeParameters.Parameters)
                typeParameter.OfFunction = true;

        var parameters = functionDeclaration.Parameters?.ParameterList.ConvertAll(Visit<Luau.AST.Parameter>) ?? [];
        var returnType = MaybeVisit<LuauType>(functionDeclaration.ReturnType);
        var statements = functionDeclaration.Body switch
        {
            ExpressionBody body => [new Luau.AST.Return(Visit(body.Expression))],
            Block block => GenerateStatements(block.Statements),
            _ => []
        };

        return new Function(functionDeclaration.Name.Text, typeParameters, parameters, returnType, new Chunk(statements));
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

    public override LuauNode VisitDeclare(Declare declare) => new NoOpStatement();

    public override LuauNode VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        var expression = Visit(expressionStatement.Expression);
        return IsUnorphanableExpression(expression)
            ? new Luau.AST.ExpressionStatement(expression)
            : new ConstVariable("_", null, expression);
    }

    public override LuauNode VisitNameOf(NameOf nameOf) => new StringLiteral(nameOf.Name.ToString());

    public override LuauNode VisitInvocation(Invocation invocation) => new Call(Visit(invocation.Expression), invocation.Arguments.ArgumentList.ConvertAll(Visit));

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
        if (!indexType.Equals(Intrinsics.RangeType))
        {
            if (TryGetEnumConstant(elementAccess, out var enumValue))
                return enumValue;

            if (!indexType.IsAssignableTo(TypeChecking.Types.PrimitiveType.Number))
                return new Luau.AST.ElementAccess(target, Visit(elementAccess.IndexExpression));

            var index = Visit(elementAccess.IndexExpression);
            return LuauFactory.StringCall("sub", [target, index, index]);
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

    public override LuauNode VisitAsExpression(AsExpression asExpression) => new TypeCast(Visit(asExpression.Expression), Visit(asExpression.Type));

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

    public override LuauNode VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var operand = Visit(unaryOperator.Operand);
        return SyntaxFacts.IsBitwiseOperator(unaryOperator.Operator.Kind)
            ? LuauFactory.Bit32Call("bnot", [operand])
            : new Luau.AST.UnaryOperator(MapLuau.UnaryOperator(unaryOperator.Operator.Text), operand);
    }

    public override LuauNode VisitParenthesized(Parenthesized parenthesized) => new Luau.AST.Parenthesized(Visit(parenthesized.Expression));

    public override LuauNode VisitInterfaceInvocation(InterfaceInvocation interfaceInvocation) => Visit(interfaceInvocation.Body);

    public override LuauNode VisitInterfaceInvocationBody(InterfaceInvocationBody interfaceInvocationBody) =>
        new Table(interfaceInvocationBody.Initializers.ConvertAll(Visit<TableInitializer>));

    public override LuauNode VisitInterfaceInvocationPropertyInitializer(InterfaceInvocationPropertyInitializer propertyInitializer) =>
        new PropertyTableInitializer(propertyInitializer.Name.Text, Visit(propertyInitializer.Expression));

    public override LuauNode VisitInterfaceInvocationIndexInitializer(InterfaceInvocationIndexInitializer indexInitializer) =>
        new ComputedPropertyTableInitializer(Visit(indexInitializer.IndexExpression), Visit(indexInitializer.Expression));

    public override LuauNode VisitRangeLiteral(RangeLiteral rangeLiteral) =>
        new Table([new PropertyTableInitializer("minimum", Visit(rangeLiteral.Minimum)), new PropertyTableInitializer("maximum", Visit(rangeLiteral.Maximum))]);

    public override LuauNode VisitArrayLiteral(ArrayLiteral arrayLiteral) => new Table(arrayLiteral.Expressions.ConvertAll(e => new TableInitializer(Visit(e))));

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

    public override LuauNode VisitFunctionType(FunctionType functionType) =>
        new Luau.AST.FunctionType(
            MaybeVisit<Luau.AST.TypeParameters>(functionType.TypeParameters),
            functionType.Parameters?.ParameterList.ConvertAll(p => Visit(p.ColonTypeClause!)) ?? [],
            Visit(functionType.ReturnType)
        );

    public override LuauNode VisitIntersectionType(IntersectionType intersectionType) => new Luau.AST.IntersectionType(intersectionType.Types.ConvertAll(Visit));
    public override LuauNode VisitUnionType(UnionType unionType) => new Luau.AST.UnionType(unionType.Types.ConvertAll(Visit));
    public override LuauNode VisitArrayType(ArrayType arrayType) => new TableType(new TableTypeIndexer(null, null, Visit(arrayType.ElementType)), []);
    public override LuauNode VisitOptionalType(OptionalType optionalType) => new Luau.AST.OptionalType(Visit(optionalType.NonNullableType));
    public override LuauNode VisitParenthesizedType(ParenthesizedType parenthesized) => new Luau.AST.ParenthesizedType(Visit(parenthesized.Type));
    public override LuauNode VisitIndexedType(IndexedType indexedType) => new Luau.AST.TypeName("index", [Visit(indexedType.Type), Visit(indexedType.IndexType)]);

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

    public override LuauNode VisitTypeParameters(TypeParameters typeParameters) =>
        new Luau.AST.TypeParameters(typeParameters.ParameterList.ConvertAll(VisitTypeParameter));

    public override Luau.AST.TypeParameter VisitTypeParameter(TypeParameter typeParameter) =>
        new(typeParameter.Name.Text, typeParameter.EqualsTypeClause != null ? Visit(typeParameter.EqualsTypeClause.Type) : null);

    public override LuauNode VisitPrimitiveType(PrimitiveType primitiveType) =>
        primitiveType is { Kind: PrimitiveTypeKind.Void or PrimitiveTypeKind.None, Parent: ColonTypeClause { Parent: DeclareFunctionSignature or FunctionType } }
            ? new UnitType()
            : new Luau.AST.PrimitiveType(MapLuau.PrimitiveTypeKind(primitiveType.Kind));

    public override LuauNode VisitLiteralType(LiteralType literalType) =>
        literalType.Value switch
        {
            long or int or double => Luau.AST.PrimitiveType.Number,
            bool b => new BooleanLiteralType(b),
            string s => new StringLiteralType(s),
            _ when literalType.Parent is ColonTypeClause { Parent: DeclareFunctionSignature or FunctionType } => new UnitType(),
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

    private bool TryGetEnumConstant(Node node, [MaybeNullWhen(false)] out LuauNode constantType)
    {
        constantType = null;
        var type = semanticModel.GetType(node);
        if (type is not TypeChecking.Types.LiteralType { Value: long or int or double or string } literal)
            return false;

        constantType = literal.Value is string s ? new StringLiteral(s) : new NumberLiteral(Convert.ToDouble(literal.Value));
        return true;
    }

    private Chunk GenerateChunk(Statement statement) => statement is Block block ? VisitBlock(block) : new Chunk([Visit(statement)]);

    private List<LuauStatement> GenerateStatements(List<Statement> statements)
    {
        var result = new List<LuauStatement>();
        foreach (var statement in statements)
        {
            var (luauStatement, scope) = Capture(() => Visit(statement));
            result.AddRange(scope.PrereqStatements);
            result.Add(luauStatement);
            result.AddRange(scope.PostreqStatements);
        }

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

    private static bool IsUnorphanableExpression(LuauExpression expression) =>
        expression is Call
        || expression is Luau.AST.BinaryOperator binaryOperator && binaryOperator.Operator.EndsWith('=');

    private LuauType Visit(TypeExpression node) => (LuauType)node.Accept(this);
    private LuauExpression Visit(Expression node) => (LuauExpression)node.Accept(this);
    private LuauStatement Visit(Statement node) => (LuauStatement)node.Accept(this);
}