using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.SemanticAnalysis;

public class ResolverScope
{
    public Dictionary<string, bool> Variables { get; } = [];
    public Dictionary<string, bool> TypeNames { get; } = [];
}

public class Resolver(Tree ast) : Diagnosable, IVisitor<bool>
{
    private readonly Stack<ResolverScope> _scopes = new();

    public DiagnosedResult Resolve()
    {
        VisitTree(ast);
        return new DiagnosedResult(Diagnostics);
    }

    public bool VisitTree(Tree tree)
    {
        PushScope();
        var result = VisitList(tree.Statements);
        PopScope();
        return result;
    }

    public bool VisitVariableDeclaration(VariableDeclaration variableDeclaration)
    {
        var scope = CurrentScope().Variables;
        var name = variableDeclaration.Name.Text;
        Declare(name, scope);
        if (variableDeclaration.ColonTypeClause != null)
        {
            Visit(variableDeclaration.ColonTypeClause);
        }
        if (variableDeclaration.EqualsValueClause != null)
        {
            Define(name, scope);
            Visit(variableDeclaration.EqualsValueClause);
        } else if (variableDeclaration.Keyword.Kind == SyntaxKind.LetKeyword)
        {
            Diagnostics.Error(variableDeclaration.Span, InternalCodes.MustHaveInitializer, "Immutable declarations must be initialized.");
            return false;
        }

        return true;
    }

    public bool VisitLiteral(Literal literal) => true;

    public bool VisitIdentifier(Identifier identifier)
    {
        var scope = CurrentScope().Variables;
        var name = identifier.Name.Text;
        if (!IsDeclared(name, scope) || IsDefined(name, scope))
            return CheckNameExists(identifier.Span, name, scope);

        Diagnostics.Error(identifier.Span, InternalCodes.UseOfUnassigned, "Attempt to use unassigned variable.");
        return false;
    }
    
    public bool VisitBinaryOperator(BinaryOperator binaryOperator) => Visit(binaryOperator.Left) && Visit(binaryOperator.Right);
    
    public bool VisitTypeName(TypeName typeName)
    {
        var scope = CurrentScope().TypeNames;
        var name = typeName.Name.Text;
        return CheckNameExists(typeName.Span, name, scope);
    }

    public bool VisitPrimitiveType(PrimitiveType primitiveType) => true;

    public bool Visit(ASTNode node) => node.Accept(this);
    
    private bool VisitList(IEnumerable<ASTNode> nodes) => nodes.Select(Visit).All(n => n);

    private bool IsDeclared(string name, Dictionary<string, bool> scope) => scope.ContainsKey(name);
    private bool IsDefined(string name, Dictionary<string, bool> scope) => IsDeclared(name, scope) && scope[name];
    private void Declare(string name, Dictionary<string, bool> scope) => scope.Add(name, false);
    private void Define(string name, Dictionary<string, bool> scope) => scope[name] = true;
    
    private bool CheckNameExists(LocationSpan span, string name, Dictionary<string, bool> scope)
    {
        var declared = IsDeclared(name, scope);
        if (!declared)
            Diagnostics.Error(span, InternalCodes.CannotFindName, $"Cannot find name '{name}'.");

        return declared;
    }
    
    private ResolverScope CurrentScope() => _scopes.Peek();
    private ResolverScope PopScope() => _scopes.Pop();
    private ResolverScope PushScope()
    {
        var scope = new ResolverScope();
        _scopes.Push(scope);
        return scope;
    }
}