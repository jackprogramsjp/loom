namespace Loom.Parsing.AST;

public interface IVisitor<out T>
{
    T VisitTree(Tree tree);
    T VisitVariableDeclaration(VariableDeclaration variableDeclaration);
    T VisitExpressionStatement(ExpressionStatement expressionStatement) => Visit(expressionStatement.Expression);
    
    T VisitLiteral(Literal literal);
    T VisitIdentifier(Identifier identifier);
    T VisitParenthesized(Parenthesized parenthesized) => Visit(parenthesized.Expression);
    T VisitBinaryOperator(BinaryOperator binaryOperator);
    T VisitUnaryOperator(UnaryOperator unaryOperator) => Visit(unaryOperator.Operand);
    
    T VisitTypeName(TypeName typeName);
    T VisitPrimitiveType(PrimitiveType primitiveType);
    T VisitOptionalType(OptionalType optionalType) => Visit(optionalType.RequiredType);
    
    T VisitColonTypeClause(ColonTypeClause colonTypeClause) => Visit(colonTypeClause.Type);
    T VisitEqualsValueClause(EqualsValueClause equalsValueClause) => Visit(equalsValueClause.Value);
    
    T VisitNullExpression(NullExpression nullExpression) => default!;
    T VisitNullStatement(NullStatement nullStatement) => default!;
    T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;
    T Visit(ASTNode node);
}