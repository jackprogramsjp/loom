using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;
using TypeName = Loom.Parsing.AST.TypeName;

namespace Loom.TypeChecking;

public class TypeChecker(SemanticModel semanticModel) : Visitor<Type>
{
    private readonly DiagnosticBag _diagnostics = new();
    private Type? _expectedType;

    public DiagnosedResult Check()
    {
        VisitTree(semanticModel.Tree);
        semanticModel.Types.SolveConstraints();
        return new DiagnosedResult(_diagnostics);
    }
    
    public override Type Visit(Node node) => node.Accept(this);

    public override Type VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        var type = base.VisitExpressionStatement(expressionStatement);
        _diagnostics.Info(expressionStatement.Span,$"Solved type '{type}' for expression: {expressionStatement.Expression}");
        return type;
    }
    
    public override Type VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        Type? declaredType = null;
        if (variableDeclaration.ColonTypeClause != null)
            declaredType = Visit(variableDeclaration.ColonTypeClause);
        
        var initializerType = declaredType ?? Types.PrimitiveType.Unknown;
        if (variableDeclaration.EqualsValueClause != null)
        {
            var previousExpected = _expectedType;
            _expectedType = declaredType;
            initializerType = Visit(variableDeclaration.EqualsValueClause);
            _expectedType = previousExpected;
        }

        Type finalType = Types.PrimitiveType.None;
        if (declaredType != null)
        {
            if (variableDeclaration.EqualsValueClause != null)
                semanticModel.Types.AddConstraint(initializerType, declaredType, variableDeclaration.EqualsValueClause.Value.Span);
            
            finalType = declaredType;
        }
        else if (variableDeclaration.EqualsValueClause != null)
        {
            finalType = initializerType;
        }

        if (variableDeclaration.Keyword.Kind == SyntaxKind.MutKeyword)
            finalType = Widen(finalType);
        
        AssertAssignability(finalType, initializerType, variableDeclaration.EqualsValueClause?.Value.Span ?? variableDeclaration.Span);
        semanticModel.Types.SetType(variableDeclaration, finalType);
        return finalType;
    }

    public override Type VisitLiteral(Literal literal) => new LiteralType(literal.Value);

    public override Type VisitIdentifier(Identifier identifier)
    {
        var symbol = semanticModel.GetSymbol(identifier);
        if (symbol != null)
            return semanticModel.Types.GetType(symbol.DeclaringNode);

        _diagnostics.Error(identifier.Span, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of '{identifier.Name.Text}'");
        return Types.PrimitiveType.Unknown;
    }

    public override Type VisitTypeName(TypeName typeName) => throw new NotImplementedException();

    public override Type VisitPrimitiveType(PrimitiveType primitiveType) => new Types.PrimitiveType(primitiveType.Kind);

    private void AssertAssignability(Type a, Type b, LocationSpan span)
    {
        if (a.IsAssignableTo(b)) return;

        _diagnostics.Error(span, InternalCodes.TypeMismatch, $"Type '{b}' is not assignable to type '{a}'.");
    }

    private static Type Widen(Type type) =>
        type switch
        {
            LiteralType literal => new Types.PrimitiveType(LiteralType.PrimitiveKindFromLiteralValue(literal.Value)),
            _ => type
        };
}