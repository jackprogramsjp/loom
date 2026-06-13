using Loom.Parsing.AST;

namespace Loom.Testing;

[Collection("Assembly")]
public class VisitorTraversalTest
{
    private sealed class RecordingVisitor : Visitor<bool>
    {
        public List<string> Log { get; } = [];

        public void Record(Tree tree) => Visit(tree);
        protected override bool Visit(Node node) => LogAndVisit(node.GetType().Name, () => node.Accept(this));

        private bool LogAndVisit(string name, Func<bool> visitChildren)
        {
            Log.Add(name);
            return visitChildren();
        }

        public override bool VisitLiteralType(LiteralType lt) => true;
        public override bool VisitPrimitiveType(PrimitiveType pt) => true;
        public override bool VisitTypeName(TypeName tn) => true;
        public override bool VisitLiteral(Literal lit) => true;
        public override bool VisitIdentifier(Identifier id) => true;
    }

    private static void AssertVisitOrder(string source, params string[] expectedOrder)
    {
        var tree = Utility.GetAST(source);
        var recorder = new RecordingVisitor();
        recorder.Record(tree);
        Assert.Equal(expectedOrder.Prepend("Tree"), recorder.Log);
    }

    [Fact]
    public void ExpressionStatement_VisitsExpression() => AssertVisitOrder("42", "ExpressionStatement", "Literal");

    [Fact]
    public void Return_VisitsExpression() =>
        AssertVisitOrder(
            "fn f() -> 1",
            "FunctionDeclaration",
            "Parameters",
            "ExpressionBody",
            "ExpressionStatement",
            "Literal"
        );

    [Fact]
    public void ReturnInsideFunction_VisitsExpression() =>
        AssertVisitOrder(
            "fn f() { return 1 }",
            "FunctionDeclaration",
            "Parameters",
            "Block",
            "Return",
            "Literal"
        );

    [Fact]
    public void Block_VisitsStatements() =>
        AssertVisitOrder(
            "{ 1; 2; }",
            "Block",
            "ExpressionStatement",
            "Literal",
            "ExpressionStatement",
            "Literal"
        );

    [Fact]
    public void If_VisitsConditionThenElse() =>
        AssertVisitOrder(
            "if true { 1 } else { 2 }",
            "If",
            "Literal",
            "Block",
            "ExpressionStatement",
            "Literal",
            "ElseBranch",
            "Block",
            "ExpressionStatement",
            "Literal"
        );

    [Fact]
    public void FunctionDeclaration_VisitsAllParts() =>
        AssertVisitOrder(
            "fn f<T>(x: number): void { }",
            "FunctionDeclaration",
            "TypeParameters",
            "TypeParameter",
            "Parameters",
            "Parameter",
            "ColonTypeClause",
            "PrimitiveType",
            "ColonTypeClause",
            "PrimitiveType",
            "Block"
        );

    [Fact]
    public void VariableDeclaration_VisitsColonAndInit() =>
        AssertVisitOrder(
            "let x: number = 1;",
            "VariableDeclaration",
            "ColonTypeClause",
            "PrimitiveType",
            "EqualsValueClause",
            "Literal"
        );

    [Fact]
    public void DeclareVariable_VisitsColonType() =>
        AssertVisitOrder(
            "declare let x: number;",
            "Declare",
            "DeclareVariableSignature",
            "ColonTypeClause",
            "PrimitiveType"
        );

    [Fact]
    public void DeclareFunction_VisitsReturnTypeAndParams() =>
        AssertVisitOrder(
            "declare fn foo(x: number): void;",
            "Declare",
            "DeclareFunctionSignature",
            "Parameters",
            "Parameter",
            "ColonTypeClause",
            "PrimitiveType",
            "ColonTypeClause",
            "PrimitiveType"
        );

    [Fact]
    public void TypeAlias_VisitsTypeAndTypeParams() =>
        AssertVisitOrder(
            "type A<T> = T;",
            "TypeAlias",
            "TypeParameters",
            "TypeParameter",
            "EqualsTypeClause",
            "TypeName"
        );

    [Fact]
    public void EnumDeclaration_VisitsMembers() =>
        AssertVisitOrder(
            "enum E { A, B }",
            "EnumDeclaration",
            "EnumMember",
            "EnumMember"
        );

    [Fact]
    public void BinaryOperator_VisitsLeftRight() =>
        AssertVisitOrder(
            "1 + 2",
            "ExpressionStatement",
            "BinaryOperator",
            "Literal",
            "Literal"
        );

    [Fact]
    public void UnaryOperator_VisitsOperand() =>
        AssertVisitOrder(
            "!true",
            "ExpressionStatement",
            "UnaryOperator",
            "Literal"
        );

    [Fact]
    public void AssignmentOperator_VisitsLeftRight() =>
        AssertVisitOrder(
            "x = 1",
            "ExpressionStatement",
            "AssignmentOperator",
            "Identifier",
            "Literal"
        );

    [Fact]
    public void Invocation_VisitsExpressionAndArguments() =>
        AssertVisitOrder(
            "foo(1, 2)",
            "ExpressionStatement",
            "Invocation",
            "Identifier",
            "Arguments",
            "Literal",
            "Literal"
        );

    [Fact]
    public void QualifiedName_VisitsIdentifier() =>
        AssertVisitOrder(
            "a.b",
            "ExpressionStatement",
            "QualifiedName",
            "Identifier"
        );

    [Fact]
    public void ElementAccess_VisitsExpressionAndIndex() =>
        AssertVisitOrder(
            "a[0]",
            "ExpressionStatement",
            "ElementAccess",
            "Identifier",
            "Literal"
        );

    [Fact]
    public void RangeLiteral_VisitsMinMax() =>
        AssertVisitOrder(
            "1..5",
            "ExpressionStatement",
            "RangeLiteral",
            "Literal",
            "Literal"
        );

    [Fact]
    public void ArrayLiteral_VisitsExpressions() =>
        AssertVisitOrder(
            "[1, 2]",
            "ExpressionStatement",
            "ArrayLiteral",
            "Literal",
            "Literal"
        );

    [Fact]
    public void Parenthesized_VisitsInner() =>
        AssertVisitOrder(
            "(1)",
            "ExpressionStatement",
            "Parenthesized",
            "Literal"
        );

    [Fact]
    public void NameOf_VisitsName() =>
        AssertVisitOrder(
            "nameof(x)",
            "ExpressionStatement",
            "NameOf",
            "Identifier"
        );
    
    [Fact]
    public void As_VisitsExpressionAndType() =>
        AssertVisitOrder(
            "x as number",
            "ExpressionStatement",
            "AsExpression",
            "Identifier",
            "PrimitiveType"
        );

    [Fact]
    public void ArrayType_VisitsElementType() =>
        AssertVisitOrder(
            "let x: number[];",
            "VariableDeclaration",
            "ColonTypeClause",
            "ArrayType",
            "PrimitiveType"
        );

    [Fact]
    public void OptionalType_VisitsNonNullable() =>
        AssertVisitOrder(
            "let x: number?;",
            "VariableDeclaration",
            "ColonTypeClause",
            "OptionalType",
            "PrimitiveType"
        );

    [Fact]
    public void UnionType_VisitsTypes() =>
        AssertVisitOrder(
            "let x: number | string;",
            "VariableDeclaration",
            "ColonTypeClause",
            "UnionType",
            "PrimitiveType",
            "PrimitiveType"
        );

    [Fact]
    public void IntersectionType_VisitsTypes() =>
        AssertVisitOrder(
            "let x: number & string;",
            "VariableDeclaration",
            "ColonTypeClause",
            "IntersectionType",
            "PrimitiveType",
            "PrimitiveType"
        );

    [Fact]
    public void TypeArguments_VisitsTypeList() =>
        AssertVisitOrder(
            "fn id<T>(v: T) {} id::<number>();",
            "FunctionDeclaration",
            "TypeParameters",
            "TypeParameter",
            "Parameters",
            "Parameter",
            "ColonTypeClause",
            "TypeName",
            "Block",
            "ExpressionStatement",
            "Invocation",
            "Identifier",
            "TypeArguments",
            "PrimitiveType",
            "Arguments"
        );
}