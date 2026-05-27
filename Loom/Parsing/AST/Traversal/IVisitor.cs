namespace Loom.Parsing.AST.Traversal;

public interface IVisitor<out T>
{
    T VisitTree(Tree tree);
    T VisitVariableDeclaration(VariableDeclaration variableDeclaration);
    
    T VisitLiteral(Literal literal);
    T VisitIdentifier(Identifier identifier);
    T VisitExpressionStatement(ExpressionStatement expressionStatement);
    
    T VisitNullExpression(NullExpression nullExpression) => default!;
    T VisitNullStatement(NullStatement nullStatement) => default!;
    T VisitNullTypeExpression(NullTypeExpression nullTypeExpression) => default!;
    T Visit(ASTNode node);
}