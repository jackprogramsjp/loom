namespace Loom.Parsing.AST;

public abstract class Visitor<T>
{
    public abstract T Visit(Node node);

    protected TResult Visit<TResult>(Node node)
        where TResult : T =>
        (TResult)Visit(node)!;

    public virtual T VisitTree(Tree tree) => VisitList(tree.Statements);

    public virtual T VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var results = new List<T>();
        if (functionDeclaration.TypeParameters != null)
            results.Add(Visit(functionDeclaration.TypeParameters));

        if (functionDeclaration.Parameters != null)
            results.Add(Visit(functionDeclaration.Parameters));

        if (functionDeclaration.ReturnType != null)
            results.Add(Visit(functionDeclaration.ReturnType));

        results.Add(Visit(functionDeclaration.Body));
        return CombineResults(results);
    }

    public virtual T VisitTypeAlias(TypeAlias typeAlias)
    {
        var results = new List<T>();
        if (typeAlias.TypeParameters != null)
            results.Add(Visit(typeAlias.TypeParameters));

        results.Add(Visit(typeAlias.EqualsTypeClause.Type));
        return CombineResults(results);
    }

    public virtual T VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var results = new List<T>();
        if (variableDeclaration.ColonTypeClause != null)
            results.Add(Visit(variableDeclaration.ColonTypeClause));

        if (variableDeclaration.EqualsValueClause != null)
            results.Add(Visit(variableDeclaration.EqualsValueClause));

        return CombineResults(results);
    }

    public virtual T VisitParameter(Parameter parameter)
    {
        var results = new List<T>();
        if (parameter.ColonTypeClause != null)
            results.Add(Visit(parameter.ColonTypeClause));

        if (parameter.EqualsValueClause != null)
            results.Add(Visit(parameter.EqualsValueClause));

        return CombineResults(results);
    }
    
    public virtual T VisitEnumDeclaration(EnumDeclaration enumDeclaration) => VisitList(enumDeclaration.Members);
    public virtual T VisitEnumMember(EnumMember enumMember) => MaybeVisit(enumMember.EqualsValueClause)!;

    public virtual T VisitParameters(Parameters parameters) => CombineResults([VisitList(parameters.ParameterList)]);
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
    public virtual T VisitInvocation(Invocation invocation) => CombineResults([Visit(invocation.Expression), Visit(invocation.Arguments)]);
    
    public virtual T VisitQualifiedName(QualifiedName qualifiedName) => Visit(qualifiedName.Identifier);
    public virtual T VisitPropertyAccess(PropertyAccess propertyAccess) => Visit(propertyAccess.Expression);
    public virtual T VisitElementAccess(ElementAccess elementAccess) => CombineResults([Visit(elementAccess.Expression), Visit(elementAccess.IndexExpression)]);

    public virtual T VisitAssignmentOperator(AssignmentOperator assignmentOperator) =>
        CombineResults([Visit(assignmentOperator.Left), Visit(assignmentOperator.Right)]);

    public virtual T VisitBinaryOperator(BinaryOperator binaryOperator) => CombineResults([Visit(binaryOperator.Left), Visit(binaryOperator.Right)]);
    public virtual T VisitUnaryOperator(UnaryOperator unaryOperator) => Visit(unaryOperator.Operand);

    public abstract T VisitLiteralType(LiteralType literalType);
    public abstract T VisitPrimitiveType(PrimitiveType primitiveType);
    public abstract T VisitTypeName(TypeName typeName);
    public virtual T VisitParenthesizedType(ParenthesizedType parenthesized) => Visit(parenthesized.Type);
    public virtual T VisitArrayType(ArrayType arrayType) => Visit(arrayType.ElementType);
    public virtual T VisitOptionalType(OptionalType optionalType) => Visit(optionalType.NonNullableType);
    public virtual T VisitUnionType(UnionType unionType) => VisitList(unionType.Types);
    public virtual T VisitIntersectionType(IntersectionType intersectionType) => VisitList(intersectionType.Types);

    public virtual T VisitTypeParameter(TypeParameter typeParameter) =>
        typeParameter.EqualsTypeClause != null ? Visit(typeParameter.EqualsTypeClause.Type) : default!;

    public virtual T VisitTypeParameters(TypeParameters typeParameters) => VisitList(typeParameters.ParameterList);
    public virtual T VisitTypeArguments(TypeArguments typeArguments) => VisitList(typeArguments.ArgumentsList);
    public virtual T VisitColonTypeClause(ColonTypeClause colonTypeClause) => Visit(colonTypeClause.Type);
    public virtual T VisitEqualsTypeClause(EqualsTypeClause equalsTypeClause) => Visit(equalsTypeClause.Type);
    public virtual T VisitEqualsValueClause(EqualsValueClause equalsValueClause) => Visit(equalsValueClause.Value);

    public virtual T VisitNullExpression(NullExpression nullExpression) => default!;
    public virtual T VisitNullStatement(NullStatement nullStatement) => default!;
    public virtual T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;

    protected virtual T CombineResults(IEnumerable<T> results) => results.LastOrDefault()!;

    protected TResult? MaybeVisit<TResult>(Node? node)
        where TResult : T =>
        node is null ? default : Visit<TResult>(node);

    protected T? MaybeVisit(Node? node) => node is null ? default : Visit(node);

    private T VisitList<TNode>(List<TNode> nodes)
        where TNode : Node =>
        CombineResults(nodes.ConvertAll(Visit));

}