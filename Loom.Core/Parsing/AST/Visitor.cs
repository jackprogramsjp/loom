// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Loom.Parsing.AST;

public abstract class Visitor<T>(T defaultValue)
{
    protected T DefaultValue { get; } = defaultValue;

    protected abstract T Visit(Node node);

    protected TResult Visit<TResult>(Node node)
        where TResult : T =>
        (TResult)Visit(node)!;

    public virtual T VisitTree(Tree tree) => VisitList(tree.Statements);
    public virtual T VisitFor(For @for) => CombineResults([VisitList(@for.Names), Visit(@for.CollectionExpression), Visit(@for.Body)]);
    public virtual T VisitAfter(After after) => CombineResults([Visit(after.Duration), Visit(after.Body)]);
    public virtual T VisitBreak(Break @break) => DefaultValue;
    public virtual T VisitContinue(Continue @continue) => DefaultValue;
    public virtual T VisitWhile(While @while) => CombineResults([Visit(@while.Condition), Visit(@while.Body)]);
    public virtual T VisitIf(If @if) => CombineResults([Visit(@if.Condition), Visit(@if.ThenBranch), MaybeVisit(@if.ElseBranch) ?? DefaultValue]);
    public virtual T VisitElseBranch(ElseBranch elseBranch) => Visit(elseBranch.Branch);

    public virtual T VisitInterfaceInvocation(InterfaceInvocation interfaceInvocation) =>
        CombineResults([Visit(interfaceInvocation.Name), MaybeVisit(interfaceInvocation.TypeArguments) ?? DefaultValue, Visit(interfaceInvocation.Body)]);

    public virtual T VisitInterfaceInvocationBody(InterfaceInvocationBody interfaceInvocationBody) => VisitList(interfaceInvocationBody.Initializers);

    public virtual T VisitInterfaceInvocationIndexInitializer(InterfaceInvocationIndexInitializer indexInitializer) =>
        CombineResults([Visit(indexInitializer.IndexExpression), Visit(indexInitializer.Expression)]);

    public virtual T VisitInterfaceInvocationPropertyInitializer(InterfaceInvocationPropertyInitializer propertyInitializer) =>
        Visit(propertyInitializer.Expression);

    public virtual T VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration) =>
        CombineResults([Visit(indexerDeclaration.IndexType), Visit(indexerDeclaration.ColonTypeClause)]);

    public virtual T VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration) => Visit(propertyDeclaration.ColonTypeClause);
    public virtual T VisitInterfaceBody(InterfaceBody interfaceBody) => VisitList(interfaceBody.Members);

    public virtual T VisitInterfaceDeclaration(InterfaceDeclaration interfaceDeclaration) =>
        CombineResults(
            [
                MaybeVisit(interfaceDeclaration.TypeParameters) ?? DefaultValue,
                MaybeVisit(interfaceDeclaration.ColonTypeListClause) ?? DefaultValue,
                MaybeVisit(interfaceDeclaration.Body) ?? DefaultValue
            ]
        );

    public virtual T VisitFunctionDeclaration(FunctionDeclaration functionDeclaration) =>
        CombineResults(
            [
                MaybeVisit(functionDeclaration.TypeParameters) ?? DefaultValue,
                MaybeVisit(functionDeclaration.Parameters) ?? DefaultValue,
                MaybeVisit(functionDeclaration.ReturnType) ?? DefaultValue,
                Visit(functionDeclaration.Body)
            ]
        );

    public virtual T VisitDeclare(Declare declare) => Visit(declare.Signature);

    public virtual T VisitDeclareVariableSignature(DeclareVariableSignature declareVariableSignature) =>
        MaybeVisit(declareVariableSignature.ColonTypeClause) ?? DefaultValue;

    public virtual T VisitDeclareFunctionSignature(DeclareFunctionSignature declareFunctionSignature) =>
        CombineResults(
            [
                MaybeVisit(declareFunctionSignature.TypeParameters) ?? DefaultValue,
                MaybeVisit(declareFunctionSignature.Parameters) ?? DefaultValue,
                Visit(declareFunctionSignature.ReturnType)
            ]
        );

    public virtual T VisitTypeAlias(TypeAlias typeAlias) =>
        CombineResults([MaybeVisit(typeAlias.TypeParameters) ?? DefaultValue, Visit(typeAlias.EqualsTypeClause)]);

    public virtual T VisitVariableDeclaration(VariableDeclaration variableDeclaration) =>
        CombineResults([MaybeVisit(variableDeclaration.ColonTypeClause) ?? DefaultValue, MaybeVisit(variableDeclaration.EqualsValueClause) ?? DefaultValue]);

    public virtual T VisitEnumDeclaration(EnumDeclaration enumDeclaration) => VisitList(enumDeclaration.Members);
    public virtual T VisitEnumMember(EnumMember enumMember) => MaybeVisit(enumMember.EqualsValueClause) ?? DefaultValue;
    public virtual T VisitParameters(Parameters parameters) => VisitList(parameters.ParameterList);

    public virtual T VisitParameter(Parameter parameter) =>
        CombineResults([MaybeVisit(parameter.ColonTypeClause) ?? DefaultValue, MaybeVisit(parameter.EqualsValueClause) ?? DefaultValue]);

    public virtual T VisitBlock(Block block) => VisitList(block.Statements);
    public virtual T VisitExpressionStatement(ExpressionStatement expressionStatement) => Visit(expressionStatement.Expression);
    public virtual T VisitReturn(Return @return) => MaybeVisit(@return.Expression) ?? DefaultValue;
    public virtual T VisitExpressionBody(ExpressionBody expressionBody) => Visit(expressionBody.Expression);

    public virtual T VisitRangeLiteral(RangeLiteral rangeLiteral) => CombineResults([Visit(rangeLiteral.Minimum), Visit(rangeLiteral.Maximum)]);
    public virtual T VisitArrayLiteral(ArrayLiteral arrayLiteral) => VisitList(arrayLiteral.Expressions);
    public virtual T VisitLiteral(Literal literal) => DefaultValue;
    public virtual T VisitIdentifier(Identifier identifier) => DefaultValue;
    public virtual T VisitParenthesized(Parenthesized parenthesized) => Visit(parenthesized.Expression);
    public virtual T VisitNameOf(NameOf nameOf) => Visit(nameOf.Name);
    public virtual T VisitArguments(Arguments arguments) => VisitList(arguments.ArgumentList);

    public virtual T VisitInvocation(Invocation invocation) =>
        CombineResults([Visit(invocation.Expression), MaybeVisit(invocation.TypeArguments) ?? DefaultValue, Visit(invocation.Arguments)]);

    public virtual T VisitQualifiedName(QualifiedName qualifiedName) => Visit(qualifiedName.Identifier);
    public virtual T VisitPropertyAccess(PropertyAccess propertyAccess) => Visit(propertyAccess.Expression);
    public virtual T VisitElementAccess(ElementAccess elementAccess) => CombineResults([Visit(elementAccess.Expression), Visit(elementAccess.IndexExpression)]);

    public virtual T VisitAsExpression(AsExpression asExpression) => CombineResults([Visit(asExpression.Expression), Visit(asExpression.Type)]);

    public virtual T VisitAssignmentOperator(AssignmentOperator assignmentOperator) =>
        CombineResults([Visit(assignmentOperator.Left), Visit(assignmentOperator.Right)]);
    
    public virtual T VisitTernaryOperator(TernaryOperator ternaryOperator) =>
        CombineResults([Visit(ternaryOperator.Condition), Visit(ternaryOperator.ThenBranch), Visit(ternaryOperator.ElseBranch)]);

    public virtual T VisitBinaryOperator(BinaryOperator binaryOperator) => CombineResults([Visit(binaryOperator.Left), Visit(binaryOperator.Right)]);
    public virtual T VisitUnaryOperator(UnaryOperator unaryOperator) => Visit(unaryOperator.Operand);
    public virtual T VisitLiteralType(LiteralType literalType) => DefaultValue;
    public virtual T VisitPrimitiveType(PrimitiveType primitiveType) => DefaultValue;
    public virtual T VisitTypeName(TypeName typeName) => MaybeVisit(typeName.TypeArguments) ?? DefaultValue;
    public virtual T VisitParenthesizedType(ParenthesizedType parenthesized) => Visit(parenthesized.Type);
    public virtual T VisitIndexedType(IndexedType indexedType) => CombineResults([Visit(indexedType.Type), Visit(indexedType.IndexType)]);
    public virtual T VisitKeyOf(KeyOf keyOf) => Visit(keyOf.Type);

    public virtual T VisitFunctionType(FunctionType functionType) =>
        CombineResults(
            [MaybeVisit(functionType.TypeParameters) ?? DefaultValue, MaybeVisit(functionType.Parameters) ?? DefaultValue, Visit(functionType.ReturnType)]
        );

    public virtual T VisitArrayType(ArrayType arrayType) => Visit(arrayType.ElementType);
    public virtual T VisitOptionalType(OptionalType optionalType) => Visit(optionalType.NonNullableType);
    public virtual T VisitUnionType(UnionType unionType) => VisitList(unionType.Types);
    public virtual T VisitIntersectionType(IntersectionType intersectionType) => VisitList(intersectionType.Types);
    public virtual T VisitTypeParameter(TypeParameter typeParameter) => CombineResults([MaybeVisit(typeParameter.ColonTypeClause) ?? DefaultValue, MaybeVisit(typeParameter.EqualsTypeClause) ?? DefaultValue]);
    public virtual T VisitTypeParameters(TypeParameters typeParameters) => VisitList(typeParameters.ParameterList);
    public virtual T VisitTypeArguments(TypeArguments typeArguments) => VisitList(typeArguments.ArgumentsList);
    public virtual T VisitColonTypeListClause(ColonTypeListClause colonTypeListClause) => VisitList(colonTypeListClause.Types);
    public virtual T VisitColonTypeClause(ColonTypeClause colonTypeClause) => Visit(colonTypeClause.Type);
    public virtual T VisitEqualsTypeClause(EqualsTypeClause equalsTypeClause) => Visit(equalsTypeClause.Type);
    public virtual T VisitEqualsValueClause(EqualsValueClause equalsValueClause) => Visit(equalsValueClause.Value);

    public virtual T VisitNullExpression(NullExpression _) => DefaultValue;
    public virtual T VisitNullStatement(NullStatement _) => DefaultValue;
    public virtual T VisitNullTypeExpression(NullTypeExpression _) => DefaultValue;

    protected virtual T CombineResults(IEnumerable<T?> results) => results.LastOrDefault(r => r != null)!;

    protected TResult? MaybeVisit<TResult>(Node? node)
        where TResult : T =>
        node is null ? default : Visit<TResult>(node);

    protected T? MaybeVisit(Node? node, T? defaultValue = default) => node is null ? defaultValue : Visit(node);

    protected T VisitList<TNode>(List<TNode> nodes)
        where TNode : Node =>
        CombineResults(nodes.ConvertAll(Visit));
}