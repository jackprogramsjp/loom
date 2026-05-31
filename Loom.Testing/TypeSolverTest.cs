using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.TypeChecking;
using Loom.TypeChecking.Types;
using IntersectionType = Loom.TypeChecking.Types.IntersectionType;
using PrimitiveType = Loom.TypeChecking.Types.PrimitiveType;
using UnionType = Loom.TypeChecking.Types.UnionType;

namespace Loom.Testing;

[Collection("Assembly")]
public class TypeSolverTest
{
    private static DiagnosticBag CreateDiagnostics() => new();

    [Fact]
    public void Unify_TwoVariables_SetEqual()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var node = Identifier("x");
        var node2 = Identifier("y");

        var type1 = solver.GetType(node);
        var type2 = solver.GetType(node2);
        solver.AddConstraint(type1, type2, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
        Assert.True(solver.GetType(node).Equals(solver.GetType(node2)), "Both variables should resolve to the same type after unification");
    }

    [Fact]
    public void Unify_VariableWithPrimitive_BindsVariable()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var node = Identifier("x");

        var variable = solver.GetType(node);
        solver.AddConstraint(variable, PrimitiveType.Number, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
        Assert.True(solver.GetType(node).Equals(PrimitiveType.Number), $"Expected 'number', got '{solver.GetType(node)}'");
    }

    [Fact]
    public void Unify_PrimitiveWithVariable_BindsVariable()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var node = Identifier("x");

        var variable = solver.GetType(node);
        solver.AddConstraint(PrimitiveType.String, variable, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
        Assert.True(solver.GetType(node).Equals(PrimitiveType.String), $"Expected 'string', got '{solver.GetType(node)}'");
    }

    [Fact]
    public void Unify_CompatiblePrimitives_Succeeds()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        solver.AddConstraint(PrimitiveType.Number, PrimitiveType.Number, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
    }

    [Fact]
    public void Unify_IncompatiblePrimitives_ReportsError()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        solver.AddConstraint(PrimitiveType.Number, PrimitiveType.String, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.False(success);
        Assert.NotEmpty(diagnostics.Errors().Set);
    }

    [Fact]
    public void Unify_VariableWithUnion_TransitiveBindings()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var nodeX = Identifier("x");
        var nodeY = Identifier("y");
        var nodeZ = Identifier("z");
        var x = solver.GetType(nodeX);
        var y = solver.GetType(nodeY);
        var z = solver.GetType(nodeZ);
        solver.AddConstraint(x, y, Utility.Span);
        solver.AddConstraint(y, PrimitiveType.Number, Utility.Span);
        solver.AddConstraint(z, x, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
        Assert.True(solver.GetType(nodeX).Equals(PrimitiveType.Number),$"Expected 'number', got '{solver.GetType(nodeX)}'");
        Assert.True(solver.GetType(nodeZ).Equals(PrimitiveType.Number),$"Expected 'number', got '{solver.GetType(nodeZ)}'");
    }

    [Fact]
    public void Unify_AssignableUnion_Succeeds()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var boolOrString = new UnionType([PrimitiveType.Bool, PrimitiveType.String]);
        solver.AddConstraint(PrimitiveType.Bool, boolOrString, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
    }

    [Fact]
    public void Unify_NotAssignableToUnion_ReportsError()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var boolOrString = new UnionType([PrimitiveType.Bool, PrimitiveType.String]);
        solver.AddConstraint(PrimitiveType.Number, boolOrString, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.False(success);
        Assert.NotEmpty(diagnostics.Errors().Set);
    }

    [Fact]
    public void Unify_AssignableIntersection_Succeeds()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        solver.AddConstraint(PrimitiveType.Bool, new IntersectionType([PrimitiveType.Bool, PrimitiveType.Bool]), Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
    }

    [Fact]
    public void Unify_VariableWithSelf_Succeeds()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var node = Identifier("x");

        var variable = solver.GetType(node);
        solver.AddConstraint(variable, variable, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
    }

    [Fact]
    public void Unify_InfiniteType_ReportsError()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var node = Identifier("x");
        var variable = solver.GetType(node);
        solver.AddConstraint(variable, variable, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success, "x = x should succeed (same variable, not infinite)");

        var solver2 = new TypeSolver(CreateDiagnostics());
        var nodeA = Identifier("a");
        var nodeB = Identifier("b");
        var a = solver2.GetType(nodeA);
        var b = solver2.GetType(nodeB);
        solver2.AddConstraint(a, b, Utility.Span);
        solver2.AddConstraint(b, a, Utility.Span);

        var success2 = solver2.SolveConstraints();
        Assert.True(success2, "Mutual variable binding should unify them without error");
    }
    
    [Fact]
    public void SetType_OverwritesVariable()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var node = Identifier("x");

        var variable = solver.GetType(node);
        Assert.IsType<TypeVariable>(variable);

        solver.SetType(node, PrimitiveType.Bool);
        Assert.True(solver.GetType(node).Equals(PrimitiveType.Bool),
            "SetType should replace the variable with the given type");
    }

    [Fact]
    public void Unify_ChainOfVariables_AllResolveToSame()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var nodes = Enumerable.Range(0, 5).Select(_ => Identifier("x")).ToList();
        var variables = nodes.Select(n => solver.GetType(n)).ToList();
        for (var i = 0; i < variables.Count - 1; i++)
            solver.AddConstraint(variables[i], variables[i + 1], Utility.Span);

        solver.AddConstraint(variables.Last(), PrimitiveType.Bool, Utility.Span);
        var success = solver.SolveConstraints();
        Assert.True(success);

        foreach (var node in nodes)
            Assert.True(solver.GetType(node).Equals(PrimitiveType.Bool), $"Expected all variables to resolve to 'bool', got '{solver.GetType(node)}'");
    }

    [Fact]
    public void Unify_MultipleConstraints_AllSatisfied()
    {
        var diagnostics = CreateDiagnostics();
        var solver = new TypeSolver(diagnostics);
        var nodeX = Identifier("x");
        var nodeY = Identifier("y");

        var x = solver.GetType(nodeX);
        var y = solver.GetType(nodeY);
        solver.AddConstraint(x, PrimitiveType.Number, Utility.Span);
        solver.AddConstraint(y, PrimitiveType.Number, Utility.Span);
        solver.AddConstraint(x, y, Utility.Span);

        var success = solver.SolveConstraints();
        Assert.True(success);
    }
    
    private static Identifier Identifier(string name) => new(Utility.IdentifierToken(name));
}