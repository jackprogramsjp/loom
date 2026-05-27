namespace Loom.Parsing.AST;

public interface IVisitor<out T>
{
    T VisitTree(Tree tree);
    T VisitVariableDeclaration(VariableDeclaration variableDeclaration);
    T VisitLiteral(Literal literal);
    T VisitIdentifier(Identifier literal);
    T VisitExpressionStatement(ExpressionStatement expressionStatement);
    T VisitNullExpression(NullExpression nullExpression) => default!;
    T VisitNullStatement(NullStatement nullStatement) => default!;
    T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;
    T Visit(ASTNode node);
}