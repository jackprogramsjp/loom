// ReSharper disable VirtualMemberNeverOverridden.Global
namespace Loom.Parsing.AST;

public abstract class Visitor<T>
{
    protected abstract T Visit(Node node);

    protected TResult Visit<TResult>(Node node)
        where TResult : T =>
        (TResult)Visit(node)!;

    public virtual T VisitTree(Tree tree) => VisitList(tree.Statements);
    public virtual T VisitIf(If @if) => CombineResults([Visit(@if.Condition), Visit(@if.ThenBranch), MaybeVisit(@if.ElseBranch)]);
    public virtual T VisitElseBranch(ElseBranch elseBranch) => Visit(elseBranch.Branch);

    public virtual T VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration) => Visit(propertyDeclaration.ColonTypeClause);

    public virtual T VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration) =>
        CombineResults([Visit(indexerDeclaration.IndexType), Visit(indexerDeclaration.ColonTypeClause)]);

    public virtual T VisitInterfaceBody(InterfaceBody interfaceBody) => VisitList(interfaceBody.Members);
    public virtual T VisitInterfaceDeclaration(InterfaceDeclaration interfaceDeclaration) =>
        CombineResults(
            [MaybeVisit(interfaceDeclaration.TypeParameters), MaybeVisit(interfaceDeclaration.ColonTypeListClause), MaybeVisit(interfaceDeclaration.Body)]
        );

    public virtual T VisitFunctionDeclaration(FunctionDeclaration functionDeclaration) =>
        CombineResults(
            [
                MaybeVisit(functionDeclaration.TypeParameters),
                MaybeVisit(functionDeclaration.Parameters),
                MaybeVisit(functionDeclaration.ReturnType),
                Visit(functionDeclaration.Body)
            ]
        );

    public virtual T VisitDeclare(Declare declare) => Visit(declare.Signature);
    public virtual T VisitDeclareVariableSignature(DeclareVariableSignature declareVariableSignature) => MaybeVisit(declareVariableSignature.ColonTypeClause)!;

    public virtual T VisitDeclareFunctionSignature(DeclareFunctionSignature declareFunctionSignature) =>
        CombineResults(
            [MaybeVisit(declareFunctionSignature.TypeParameters), MaybeVisit(declareFunctionSignature.Parameters), Visit(declareFunctionSignature.ReturnType)]
        );

    public virtual T VisitTypeAlias(TypeAlias typeAlias) => CombineResults([MaybeVisit(typeAlias.TypeParameters), Visit(typeAlias.EqualsTypeClause)]);

    public virtual T VisitVariableDeclaration(VariableDeclaration variableDeclaration) =>
        CombineResults([MaybeVisit(variableDeclaration.ColonTypeClause), MaybeVisit(variableDeclaration.EqualsValueClause)]);

    public virtual T VisitEnumDeclaration(EnumDeclaration enumDeclaration) => VisitList(enumDeclaration.Members);
    public virtual T VisitEnumMember(EnumMember enumMember) => MaybeVisit(enumMember.EqualsValueClause)!;
    public virtual T VisitParameters(Parameters parameters) => VisitList(parameters.ParameterList);
    public virtual T VisitParameter(Parameter parameter) => CombineResults([MaybeVisit(parameter.ColonTypeClause), MaybeVisit(parameter.EqualsValueClause)]);
    public virtual T VisitBlock(Block block) => VisitList(block.Statements);
    public virtual T VisitExpressionStatement(ExpressionStatement expressionStatement) => Visit(expressionStatement.Expression);
    public virtual T VisitReturn(Return @return) => Visit(@return.Expression);
    public virtual T VisitExpressionBody(ExpressionBody expressionBody) => Visit(new ExpressionStatement(expressionBody.Expression));

    public virtual T VisitRangeLiteral(RangeLiteral rangeLiteral) => CombineResults([Visit(rangeLiteral.Minimum), Visit(rangeLiteral.Maximum)]);
    public virtual T VisitArrayLiteral(ArrayLiteral arrayLiteral) => VisitList(arrayLiteral.Expressions);
    public abstract T VisitLiteral(Literal literal);
    public abstract T VisitIdentifier(Identifier identifier);
    public virtual T VisitParenthesized(Parenthesized parenthesized) => Visit(parenthesized.Expression);
    public virtual T VisitNameOf(NameOf nameOf) => Visit(nameOf.Name);
    public virtual T VisitArguments(Arguments arguments) => VisitList(arguments.ArgumentList);

    public virtual T VisitInvocation(Invocation invocation) =>
        CombineResults([Visit(invocation.Expression), MaybeVisit(invocation.TypeArguments), Visit(invocation.Arguments)]);

    public virtual T VisitQualifiedName(QualifiedName qualifiedName) => Visit(qualifiedName.Identifier);
    public virtual T VisitPropertyAccess(PropertyAccess propertyAccess) => Visit(propertyAccess.Expression);
    public virtual T VisitElementAccess(ElementAccess elementAccess) => CombineResults([Visit(elementAccess.Expression), Visit(elementAccess.IndexExpression)]);

    public virtual T VisitAsExpression(AsExpression asExpression) => CombineResults([Visit(asExpression.Expression), Visit(asExpression.Type)]);

    public virtual T VisitAssignmentOperator(AssignmentOperator assignmentOperator) =>
        CombineResults([Visit(assignmentOperator.Left), Visit(assignmentOperator.Right)]);

    public virtual T VisitBinaryOperator(BinaryOperator binaryOperator) => CombineResults([Visit(binaryOperator.Left), Visit(binaryOperator.Right)]);
    public virtual T VisitUnaryOperator(UnaryOperator unaryOperator) => Visit(unaryOperator.Operand);
    public abstract T VisitLiteralType(LiteralType literalType);
    public abstract T VisitPrimitiveType(PrimitiveType primitiveType);
    public virtual T VisitTypeName(TypeName typeName) => MaybeVisit(typeName.TypeArguments)!;
    public virtual T VisitParenthesizedType(ParenthesizedType parenthesized) => Visit(parenthesized.Type);
    public virtual T VisitIndexedType(IndexedType indexedType) => CombineResults([Visit(indexedType.Type), Visit(indexedType.IndexType)]);

    public virtual T VisitFunctionType(FunctionType functionType) =>
        CombineResults([MaybeVisit(functionType.TypeParameters), MaybeVisit(functionType.Parameters), Visit(functionType.ReturnType)]);

    public virtual T VisitArrayType(ArrayType arrayType) => Visit(arrayType.ElementType);
    public virtual T VisitOptionalType(OptionalType optionalType) => Visit(optionalType.NonNullableType);
    public virtual T VisitUnionType(UnionType unionType) => VisitList(unionType.Types);
    public virtual T VisitIntersectionType(IntersectionType intersectionType) => VisitList(intersectionType.Types);
    public virtual T VisitTypeParameter(TypeParameter typeParameter) => MaybeVisit(typeParameter.EqualsTypeClause)!;
    public virtual T VisitTypeParameters(TypeParameters typeParameters) => VisitList(typeParameters.ParameterList);
    public virtual T VisitTypeArguments(TypeArguments typeArguments) => VisitList(typeArguments.ArgumentsList);
    public virtual T VisitColonTypeListClause(ColonTypeListClause colonTypeListClause) => VisitList(colonTypeListClause.Types);
    public virtual T VisitColonTypeClause(ColonTypeClause colonTypeClause) => Visit(colonTypeClause.Type);
    public virtual T VisitEqualsTypeClause(EqualsTypeClause equalsTypeClause) => Visit(equalsTypeClause.Type);
    public virtual T VisitEqualsValueClause(EqualsValueClause equalsValueClause) => Visit(equalsValueClause.Value);

    public virtual T VisitNullExpression(NullExpression nullExpression) => default!;
    public virtual T VisitNullStatement(NullStatement nullStatement) => default!;
    public virtual T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;

    protected virtual T CombineResults(IEnumerable<T?> results) => results.LastOrDefault(r => r != null)!;

    protected TResult? MaybeVisit<TResult>(Node? node)
        where TResult : T =>
        node is null ? default : Visit<TResult>(node);

    protected T? MaybeVisit(Node? node) => node is null ? default : Visit(node);

    private T VisitList<TNode>(List<TNode> nodes)
        where TNode : Node =>
        CombineResults(nodes.ConvertAll(Visit));
}