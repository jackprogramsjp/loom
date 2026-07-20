using Loom.Core.Diagnostics;
using Loom.Core.Generation.Macros;
using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Core.Text;
using Loom.Core.TypeChecking;
using Loom.Core.TypeChecking.Types;
using Loom.Luau;
using Loom.Luau.AST;
using ArrayType = Loom.Core.TypeChecking.Types.ArrayType;
using BinaryOperator = Loom.Core.Parsing.AST.BinaryOperator;
using ElementAccess = Loom.Core.Parsing.AST.ElementAccess;
using Expression = Loom.Core.Parsing.AST.Expression;
using ExpressionStatement = Loom.Core.Parsing.AST.ExpressionStatement;
using Identifier = Loom.Core.Parsing.AST.Identifier;
using PropertyAccess = Loom.Core.Parsing.AST.PropertyAccess;
using TypeAlias = Loom.Core.Parsing.AST.TypeAlias;
using TypeName = Loom.Core.Parsing.AST.TypeName;
using TypeParameter = Loom.Core.Parsing.AST.TypeParameter;
using TypeParameters = Loom.Core.Parsing.AST.TypeParameters;
using UnaryOperator = Loom.Core.Parsing.AST.UnaryOperator;

namespace Loom.Core.Generation;

public sealed partial class LuauGenerator
    : Visitor<LuauNode>
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly LuauState _state = new();
    private readonly MacroExpander _macroExpander;
    private readonly SemanticModel _semanticModel;

    public LuauGenerator(SemanticModel semanticModel)
        : base(_ => new NoOpStatement())
    {
        _semanticModel = semanticModel;
        _macroExpander = new MacroExpander(semanticModel, _state, _diagnostics);
    }

    public LuauGeneratorResult Generate()
    {
        var luauTree = VisitTree(_semanticModel.Tree);
        if (_semanticModel.MustImportRuntimeLibrary)
            luauTree.Statements.Insert(0, LuauFactory.RuntimeLibraryImport("@game/ReplicatedStorage/include/loom_runtime")); // TODO: rojo resolver; issue #21

        return new LuauGeneratorResult(luauTree, _diagnostics);
    }

    protected override LuauNode Visit(Node node) => node.Accept(this);

    public override LuauNode VisitFor(For @for)
    {
        var names = @for.Names.ConvertAll(n => n.Token.Text);
        var body = GenerateChunk(@for.Body);
        var collectionType = _semanticModel.GetType(@for.CollectionExpression);
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
            incrementBy = MacroContext.TryComputeConstantArithmetic(start, out var minimum) && MacroContext.TryComputeConstantArithmetic(end, out var maximum)
                ? maximum < minimum ? negativeOne : null
                : new IfExpression(new Luau.AST.BinaryOperator(end, "<", start), negativeOne, [], one);
        }
        else
        {
            var rangeIdentifier = _state.PushToVariable("_range", collectionExpression);
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
        var statements = GenerateFunctionBody(functionDeclaration);
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
        if (initializer != null)
            initializer = LuauFactory.UnwrapParentheses(initializer);

        return isConst
            ? new ConstVariable(name, type, initializer!)
            : new LocalVariable(name, type, initializer);
    }

    public override LuauNode VisitEnumDeclaration(EnumDeclaration enumDeclaration)
    {
        if (_semanticModel.GetType(enumDeclaration) is not ObjectType objectType)
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

    public override LuauNode VisitInvocation(Invocation invocation)
    {
        var isMethod = invocation.Expression.Children.FirstOrDefault() is { } child
            && _semanticModel.GetType(child) is InterfaceType interfaceType
            && invocation.Expression.Tokens.LastOrDefault() is { Kind: SyntaxKind.Identifier } name
            && interfaceType.TraitMethodNames.Contains(name.Text);

        var call = new Call(Visit(invocation.Expression), invocation.Arguments.ArgumentList.ConvertAll(Visit), isMethod);
        return _macroExpander.TryGetInvocationMacro(invocation, call, out var replacement) ? replacement : call;
    }

    public override LuauNode VisitQualifiedName(QualifiedName qualifiedName)
    {
        var luauAccess = new Luau.AST.PropertyAccess(Visit(qualifiedName.Identifier), qualifiedName.Names.ConvertAll(dotName => dotName.Name.Text));
        if (_macroExpander.TryGetQualifiedNameMacro(qualifiedName, luauAccess, out var propertyReplacement))
            return propertyReplacement;

        if (_macroExpander.TryGetInvocationMacroReference(qualifiedName, luauAccess, out var referenceReplacement))
            return referenceReplacement;

        return luauAccess;
    }

    public override LuauNode VisitPropertyAccess(PropertyAccess propertyAccess)
    {
        var luauAccess = new Luau.AST.PropertyAccess(Visit(propertyAccess.Expression), propertyAccess.Names.ConvertAll(dotName => dotName.Name.Text));
        if (_macroExpander.TryGetPropertyAccessMacro(propertyAccess, luauAccess, out var propertyReplacement))
            return propertyReplacement;

        if (_macroExpander.TryGetInvocationMacroReference(propertyAccess, luauAccess, out var referenceReplacement))
            return referenceReplacement;

        return luauAccess;
    }

    public override LuauNode VisitElementAccess(ElementAccess elementAccess)
    {
        var luauAccess = new Luau.AST.ElementAccess(Visit(elementAccess.Expression), Visit(elementAccess.IndexExpression));
        return _macroExpander.TryGetElementAccessMacro(elementAccess, luauAccess, out var replacement)
            ? replacement
            : luauAccess;
    }

    public override LuauNode VisitAssignmentOperator(AssignmentOperator assignmentOperator)
    {
        if (assignmentOperator.Parent is ExpressionStatement)
            return VisitBinaryOperator(assignmentOperator);

        if (assignmentOperator.Left is Identifier)
        {
            var binary = (Luau.AST.BinaryOperator)VisitBinaryOperator(assignmentOperator);
            var assignmentStatement = new Luau.AST.ExpressionStatement(binary);
            _state.Prereq(assignmentStatement);

            return binary.Left;
        }

        var left = Visit(assignmentOperator.Left);
        var right = Visit(assignmentOperator.Right);
        if (assignmentOperator.Parent is EqualsValueClause { Parent: NamedDeclaration declaration })
        {
            var identifierAssignment = new Luau.AST.BinaryOperator(left, "=", new Luau.AST.Identifier(declaration.Name.Text));
            _state.Postreq(new Luau.AST.ExpressionStatement(identifierAssignment));

            return right;
        }

        var assigned = _state.PushToVariable("_assigned", right);
        var boundAssignment = new Luau.AST.BinaryOperator(left, "=", assigned);
        _state.Prereq(new Luau.AST.ExpressionStatement(boundAssignment));

        return assigned;
    }

    public override LuauNode VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        if (SyntaxFacts.IsBitwiseOperator(binaryOperator.Operator.Kind))
            return GenerateBitwiseOperator(binaryOperator);

        var op = binaryOperator.Operator.Text;
        var leftType = _semanticModel.GetType(binaryOperator.Left);
        var rightType = _semanticModel.GetType(binaryOperator.Right);
        var @string = TypeChecking.Types.PrimitiveType.String;
        var isConcatenation = op.StartsWith('+') && leftType.IsAssignableTo(@string) && rightType.IsAssignableTo(@string);
        var left = Visit(binaryOperator.Left);
        var right = Visit(binaryOperator.Right);
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
        var symbol = _semanticModel.GetSymbol(typeName);
        if (symbol == null)
        {
            _diagnostics.Error(typeName, InternalCodes.CannotFindSymbol, $"Cannot find symbol for type '{typeName}'");
            return new NilLiteral();
        }

        var typeArguments = typeName.TypeArguments?.ArgumentsList.ConvertAll(Visit);
        var luauTypeName = new Luau.AST.TypeName(typeName.Name.Text, typeArguments);
        if (symbol.IsIntrinsic)
            return new QualifiedTypeName([LuauFactory.RuntimeImportName], luauTypeName);

        var constraint = symbol.Declaration is TypeParameter { ColonTypeClause: { } clause } ? Visit(clause) : null;
        return constraint != null ? new Luau.AST.IntersectionType([luauTypeName, constraint]) : luauTypeName;
    }

    private LuauNode GenerateBitwiseOperator(BinaryOperator binaryOperator)
    {
        var left = Visit(binaryOperator.Left);
        var right = Visit(binaryOperator.Right);
        var op = binaryOperator.Operator.Text;
        if (op.EndsWith('='))
            return GenerateBitwiseAssignmentOperator(op, left, right);

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

    private static Luau.AST.BinaryOperator GenerateBitwiseAssignmentOperator(string op, LuauExpression left, LuauExpression right)
    {
        var name = MapLuau.BitwiseOperator(op);
        var arguments = new List<LuauExpression> { left };
        var rightUpdated = AddBit32Arguments(right, name, arguments);
        if (!rightUpdated)
            arguments.Add(right);

        return new Luau.AST.BinaryOperator(left, "=", LuauFactory.Bit32Call(name, arguments));
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

    private Chunk GenerateFunctionBody(FunctionDeclaration functionDeclaration) =>
        functionDeclaration.Body is ExpressionBody expressionBody
            ? new Chunk(GenerateStatements(expressionBody.Expression))
            : GenerateChunk(functionDeclaration.Body);

    private Luau.AST.TypeParameters GenerateTypeParameters(TypeParameters? typeParameters) =>
        MaybeVisit<Luau.AST.TypeParameters>(typeParameters) ?? new Luau.AST.TypeParameters();

    private Chunk GenerateChunk(Statement statement) =>
        statement is Block block
            ? new Chunk(GenerateStatements(block.Statements))
            : new Chunk(GenerateStatements(statement));

    private List<LuauStatement> GenerateStatements(List<Statement> statements)
    {
        var result = new List<LuauStatement>();
        foreach (var statement in statements)
            result.AddRange(GenerateStatements(statement));

        return result.FindAll(s => s is not NoOpStatement);
    }

    private List<LuauStatement> GenerateStatements(Expression expression)
    {
        var result = new List<LuauStatement>();
        var (luauExpression, scope) = _state.Capture(() => Visit(expression));
        ApplyPrereqAndPostreq(result, scope, new Luau.AST.Return(luauExpression));

        return result.FindAll(s => s is not NoOpStatement);
    }

    private List<LuauStatement> GenerateStatements(Statement statement)
    {
        if (statement is NamedDeclaration { Name.Text: var name })
            _state.Scope.AddIdentifier(name);

        var result = new List<LuauStatement>();
        var (luauStatement, scope) = _state.Capture(() => Visit(statement));
        ApplyPrereqAndPostreq(result, scope, luauStatement);

        return result.FindAll(s => s is not NoOpStatement);
    }

    private static void ApplyPrereqAndPostreq(List<LuauStatement> result, LuauScope scope, LuauStatement luauStatement)
    {
        result.AddRange(scope.PrereqStatements);
        result.Add(luauStatement);
        result.AddRange(scope.PostreqStatements);
    }

    private List<LuauExpression> WrapFunctionArgument(Statement body, LuauType? returnType = null)
    {
        var chunk = GenerateChunk(body);
        return chunk is { Statements: [Luau.AST.ExpressionStatement { Expression: Call { IsMethod: false } call }] }
            ? [call.Callee, ..call.Arguments]
            : [new AnonymousFunction(null, [], returnType ?? new UnitType(), chunk)];
    }

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