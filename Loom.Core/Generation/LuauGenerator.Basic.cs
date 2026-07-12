using Loom.Core.Parsing.AST;
using Loom.Core.Resolving;
using Loom.Core.Text;
using Loom.Core.TypeChecking.Types;
using Loom.Luau;
using Loom.Luau.AST;
using ArrayType = Loom.Core.Parsing.AST.ArrayType;
using Break = Loom.Core.Parsing.AST.Break;
using Continue = Loom.Core.Parsing.AST.Continue;
using ExpressionStatement = Loom.Core.Parsing.AST.ExpressionStatement;
using FunctionType = Loom.Core.Parsing.AST.FunctionType;
using Identifier = Loom.Core.Parsing.AST.Identifier;
using IntersectionType = Loom.Core.Parsing.AST.IntersectionType;
using LiteralType = Loom.Core.Parsing.AST.LiteralType;
using OptionalType = Loom.Core.Parsing.AST.OptionalType;
using Parameter = Loom.Core.Parsing.AST.Parameter;
using Parenthesized = Loom.Core.Parsing.AST.Parenthesized;
using ParenthesizedType = Loom.Core.Parsing.AST.ParenthesizedType;
using PrimitiveType = Loom.Core.Parsing.AST.PrimitiveType;
using PrimitiveTypeKind = Loom.Core.TypeChecking.Types.PrimitiveTypeKind;
using Return = Loom.Core.Parsing.AST.Return;
using TypeParameter = Loom.Core.Parsing.AST.TypeParameter;
using TypeParameters = Loom.Core.Parsing.AST.TypeParameters;
using UnionType = Loom.Core.Parsing.AST.UnionType;

namespace Loom.Core.Generation;

public sealed partial class LuauGenerator
{
    public override LuauTree VisitTree(Tree tree) => new(GenerateStatements(tree.Statements));
    public override Chunk VisitBlock(Block block) => new(GenerateStatements(block.Statements));
    public override LuauNode VisitBreak(Break @break) => new Luau.AST.Break();
    public override LuauNode VisitContinue(Continue @continue) => new Luau.AST.Continue();
    public override LuauNode VisitWhile(While @while) => new WhileStatement(Visit(@while.Condition), GenerateChunk(@while.Body));

    public override LuauNode VisitAfter(After after) =>
        new Luau.AST.ExpressionStatement(LuauFactory.TaskCall("delay", [Visit(after.Duration), ..WrapFunctionArgument(after.Body)]));

    public override LuauNode VisitParameter(Parameter parameter) => new Luau.AST.Parameter(parameter.Name.Text, MaybeVisit<LuauType>(parameter.ColonTypeClause));
    public override LuauNode VisitReturn(Return @return) => new Luau.AST.Return(MaybeVisit<LuauExpression>(@return.Expression));
    public override LuauNode VisitDeclare(Declare declare) => declare.Signature is InterfaceDeclaration ? Visit(declare.Signature) : new NoOpStatement();
    public override LuauNode VisitExpressionStatement(ExpressionStatement expressionStatement) => WrapExpressionAsStatement(Visit(expressionStatement.Expression));
    public override LuauNode VisitNameOf(NameOf nameOf) => new StringLiteral(nameOf.Name.ToString());

    public override LuauNode VisitAsExpression(AsExpression asExpression) => new TypeCast(Visit(asExpression.Expression), Visit(asExpression.Type));

    public override LuauNode VisitTernaryOperator(TernaryOperator ternaryOperator) =>
        new IfExpression(Visit(ternaryOperator.Condition), Visit(ternaryOperator.ThenBranch), [], Visit(ternaryOperator.ElseBranch));

    public override LuauNode VisitParenthesized(Parenthesized parenthesized) => new Luau.AST.Parenthesized(Visit(parenthesized.Expression));

    public override LuauNode VisitRangeLiteral(RangeLiteral rangeLiteral) =>
        new Table([new PropertyTableInitializer("minimum", Visit(rangeLiteral.Minimum)), new PropertyTableInitializer("maximum", Visit(rangeLiteral.Maximum))]);

    public override LuauNode VisitArrayLiteral(ArrayLiteral arrayLiteral) => new Table(arrayLiteral.Expressions.ConvertAll(e => new TableInitializer(Visit(e))));

    public override LuauNode VisitLiteral(Literal literal) =>
        literal.Value switch
        {
            long l => new NumberLiteral(l),
            int i => new NumberLiteral(i),
            double d => new NumberLiteral(d),
            string s => new StringLiteral(s),
            bool b => new BooleanLiteral(b),
            _ => new NilLiteral()
        };

    public override LuauNode VisitIdentifier(Identifier identifier) =>
        _semanticModel.GetSymbol(identifier) is { Kind: SymbolKind.PropertyVariable }
            ? new Luau.AST.PropertyAccess(LuauFactory.Self, [identifier.Name.Text])
            : new Luau.AST.Identifier(identifier.Name.Text);

    public override LuauNode VisitFunctionType(FunctionType functionType) =>
        new Luau.AST.FunctionType(
            MaybeVisit<Luau.AST.TypeParameters>(functionType.TypeParameters),
            functionType.Parameters?.ParameterList.ConvertAll(p => Visit(p.ColonTypeClause!)) ?? [],
            Visit(functionType.ReturnType)
        );

    public override LuauNode VisitIntersectionType(IntersectionType intersectionType) => new Luau.AST.IntersectionType(intersectionType.Types.ConvertAll(Visit));
    public override LuauNode VisitUnionType(UnionType unionType) => new Luau.AST.UnionType(unionType.Types.ConvertAll(Visit));
    public override LuauNode VisitArrayType(ArrayType arrayType) => new TableType(new TableTypeIndexer(null, null, Visit(arrayType.ElementType)), []);
    public override LuauNode VisitOptionalType(OptionalType optionalType) => new Luau.AST.OptionalType(Visit(optionalType.NonNullableType));
    public override LuauNode VisitParenthesizedType(ParenthesizedType parenthesized) => new Luau.AST.ParenthesizedType(Visit(parenthesized.Type));
    public override LuauNode VisitIndexedType(IndexedType indexedType) => new Luau.AST.TypeName("index", [Visit(indexedType.Type), Visit(indexedType.IndexType)]);
    public override LuauNode VisitKeyOf(KeyOf keyOf) => new Luau.AST.TypeName("keyof", [Visit(keyOf.Type)]);

    public override LuauNode VisitTypeParameters(TypeParameters typeParameters) =>
        new Luau.AST.TypeParameters(typeParameters.ParameterList.ConvertAll(VisitTypeParameter));

    public override Luau.AST.TypeParameter VisitTypeParameter(TypeParameter typeParameter) =>
        new(typeParameter.Name.Text, typeParameter.EqualsTypeClause != null ? Visit(typeParameter.EqualsTypeClause.Type) : null);

    public override LuauNode VisitPrimitiveType(PrimitiveType primitiveType) =>
        primitiveType is { Kind: PrimitiveTypeKind.Void or PrimitiveTypeKind.None, Parent: ColonTypeClause { Parent: DeclareFunctionSignature or FunctionType } }
            ? new UnitType()
            : new Luau.AST.PrimitiveType(MapLuau.PrimitiveTypeKind(primitiveType.Kind));

    public override LuauNode VisitLiteralType(LiteralType literalType) =>
        literalType.Value switch
        {
            long or int or double => Luau.AST.PrimitiveType.Number,
            bool b => new BooleanLiteralType(b),
            string s => new StringLiteralType(s),
            _ when literalType.Parent is ColonTypeClause { Parent: DeclareFunctionSignature or FunctionType } => new UnitType(),
            _ => Luau.AST.PrimitiveType.Nil
        };
}