using Loom.Parsing.AST;
using Loom.Text;

namespace Loom.Parsing;

public sealed partial class Parser
{
    private Dictionary<SyntaxKind, Func<Token, Statement>> StatementParsers =>
        new()
        {
            [SyntaxKind.LBrace] = ParseBlock,
            [SyntaxKind.ReturnKeyword] = ParseReturn,
            [SyntaxKind.FnKeyword] = ParseFunctionDeclaration,
            [SyntaxKind.LetKeyword] = ParseVariableDeclaration,
            [SyntaxKind.MutKeyword] = ParseVariableDeclaration,
            [SyntaxKind.TypeKeyword] = ParseTypeAlias,
            [SyntaxKind.EnumKeyword] = ParseEnumDeclaration,
            [SyntaxKind.DeclareKeyword] = ParseDeclareStatement,
            [SyntaxKind.InterfaceKeyword] = ParseInterfaceDeclaration,
            [SyntaxKind.SealedKeyword] = ParseInterfaceDeclaration,
            [SyntaxKind.IfKeyword] = ParseIf,
            [SyntaxKind.ForKeyword] = ParseFor,
            [SyntaxKind.AfterKeyword] = ParseAfter,
            [SyntaxKind.WhileKeyword] = ParseWhile,
            [SyntaxKind.BreakKeyword] = ParseBreak,
            [SyntaxKind.ContinueKeyword] = ParseContinue,
        };

    private Statement ParseStatement()
    {
        if (IsEof())
            return new ExpressionStatement(ParseExpression());

        var token = Advance();
        var statementParser = StatementParsers.GetValueOrDefault(token.Kind);
        if (statementParser != null)
            return statementParser(token);

        _position--;
        return new ExpressionStatement(ParseExpression());
    }

    private Block ParseBlock(Token leftBrace)
    {
        var statements = new List<Statement>();
        while (!Match(SyntaxKind.RBrace))
            statements.Add(ParseStatement());

        var rightBrace = Last();
        return new Block(leftBrace, rightBrace, statements);
    }

    private Return ParseReturn(Token keyword)
    {
        if (IsEof() || Current().Kind == SyntaxKind.RBrace || AtStatementKeyword())
            return new Return(keyword, null);

        var expression = ParseExpression();
        return new Return(keyword, expression);
    }

    private bool AtStatementKeyword() => !IsEof() && StatementParsers.ContainsKey(Current().Kind);

    private For ParseFor(Token keyword)
    {
        var names = ParseDelimited(() => new Identifier(ExpectIdentifier()));
        var colon = Expect(SyntaxKind.Colon);
        var expression = ParseExpression();
        var body = ParseStatement();
        return new For(keyword, names, colon, expression, body);
    }

    private After ParseAfter(Token keyword)
    {
        var condition = ParseExpression();
        var body = ParseControlFlowBody(keyword);
        return new After(keyword, condition, body);
    }

    private static Break ParseBreak(Token keyword) => new(keyword);
    private static Continue ParseContinue(Token keyword) => new(keyword);

    private While ParseWhile(Token keyword)
    {
        var condition = ParseExpression();
        var body = ParseControlFlowBody(keyword);
        return new While(keyword, condition, body);
    }

    private If ParseIf(Token keyword)
    {
        var condition = ParseExpression();
        var thenBranch = ParseControlFlowBody(keyword);
        var elseBranch = Match(out var elseKeyword, SyntaxKind.ElseKeyword) ? new ElseBranch(elseKeyword, ParseControlFlowBody(keyword)) : null;
        return new If(keyword, condition, thenBranch, elseBranch);
    }

    private Statement ParseControlFlowBody(Token keyword)
    {
        var statement = ParseStatement();
        return AssertDeclarationInsideOfBlock(statement) ? statement : new NullStatement(keyword);
    }
}