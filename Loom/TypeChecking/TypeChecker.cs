using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.SemanticAnalysis;
using Loom.Syntax;
using Loom.TypeChecking.Types;
using PrimitiveType = Loom.Parsing.AST.PrimitiveType;
using Type = Loom.TypeChecking.Types.Type;
using TypeName = Loom.Parsing.AST.TypeName;

namespace Loom.TypeChecking;

public class TypeChecker : Visitor<Type>
{
    public TypeSolver TypeSolver { get; }

    private readonly DiagnosticBag _diagnostics = new();
    private readonly SemanticModel _semanticModel;
    private Type? _expectedType;

    public TypeChecker(SemanticModel semanticModel)
    {
        TypeSolver = new TypeSolver(_diagnostics);
        _semanticModel = semanticModel;
    }

    public DiagnosedResult Check()
    {
        var type = VisitTree(_semanticModel.Tree);
        BindType(_semanticModel.Tree, type);
        TypeSolver.SolveConstraints();
        return new DiagnosedResult(_diagnostics);
    }
    
    public override Type Visit(Node node) => node.Accept(this);

    public override Type VisitExpressionStatement(ExpressionStatement expressionStatement)
    {
        var type = base.VisitExpressionStatement(expressionStatement);
        _diagnostics.Info(expressionStatement.Span,$"Solved type '{TypeSimplifier.Simplify(type)}' for expression: {expressionStatement.Expression}");
        return BindType(expressionStatement, type);
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
                TypeSolver.AddConstraint(initializerType, declaredType, variableDeclaration.EqualsValueClause.Value.Span);
            
            finalType = declaredType;
        }
        else if (variableDeclaration.EqualsValueClause != null)
        {
            finalType = initializerType;
        }

        if (variableDeclaration.Keyword.Kind == SyntaxKind.MutKeyword)
            finalType = finalType.Widen();
        
        AssertAssignability(finalType, initializerType, variableDeclaration.EqualsValueClause?.Value.Span ?? variableDeclaration.Span);
        return BindType(variableDeclaration, finalType);
    }

    public override Type VisitLiteral(Literal literal) => BindType(literal, new LiteralType(literal.Value));

    public override Type VisitIdentifier(Identifier identifier)
    {
        var symbol = _semanticModel.GetSymbol(identifier);
        if (symbol != null)
        {
            var type = TypeSolver.GetType(symbol.DeclaringNode);
            return BindType(identifier, type);
        }

        _diagnostics.Error(identifier.Span, InternalCodes.CannotFindSymbol, $"Cannot find symbol for declaration of '{identifier.Name.Text}'.");
        return BindType(identifier, Types.PrimitiveType.Never);
    }

    public override Type VisitTypeName(TypeName typeName)
    {
        _diagnostics.NotImplemented(typeName.Span);
        return BindType(typeName, Types.PrimitiveType.Never);
    }

    public override Type VisitPrimitiveType(PrimitiveType primitiveType) => BindType(primitiveType, new Types.PrimitiveType(primitiveType.Kind));

    private void AssertAssignability(Type a, Type b, LocationSpan span)
    {
        if (a.IsAssignableTo(b)) return;

        _diagnostics.Error(span, InternalCodes.TypeMismatch, $"Type '{b}' is not assignable to type '{a}'.");
    }
    
    private Type BindType(Node node, Type type)
    {
        TypeSolver.SetType(node, type);
        return type;
    }
}