using Loom.Parsing.AST;
using Loom.Text;

namespace Loom.Parsing;

public sealed partial class Parser
{
    private Statement ParseStatement()
    {
        if (IsEof())
            return new ExpressionStatement(ParseExpression());

        var token = Advance();
        var statement = token.Kind switch
        {
            SyntaxKind.LBrace => ParseBlock(token),
            SyntaxKind.ReturnKeyword => new Return(token, ParseExpression()),
            SyntaxKind.FnKeyword => ParseFunctionDeclaration(token),
            SyntaxKind.LetKeyword or SyntaxKind.MutKeyword => ParseVariableDeclaration(token),
            SyntaxKind.TypeKeyword => ParseTypeAlias(token),
            SyntaxKind.EnumKeyword => ParseEnumDeclaration(token),
            SyntaxKind.DeclareKeyword => ParseDeclareStatement(token),
            SyntaxKind.InterfaceKeyword or SyntaxKind.SealedKeyword => ParseInterfaceDeclaration(token),
            SyntaxKind.IfKeyword => ParseIf(token),
            SyntaxKind.ForKeyword => ParseFor(token),
            SyntaxKind.AfterKeyword => ParseAfter(token),
            SyntaxKind.WhileKeyword => ParseWhile(token),
            SyntaxKind.BreakKeyword => ParseBreak(token),
            SyntaxKind.ContinueKeyword => ParseContinue(token),
            _ => null
        };

        if (statement != null)
            return statement;

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
    
    private Statement ParseFor(Token keyword)
    {
        var variableKeyword = Match(out var letKeyword, SyntaxKind.LetKeyword) ? letKeyword : Expect(SyntaxKind.MutKeyword, "variable signature");
        var declaration = ParseDeclareVariableSignature(variableKeyword);
        if (variableKeyword.Kind is not (SyntaxKind.LetKeyword or SyntaxKind.MutKeyword))
            return new NullStatement(variableKeyword);

        var inKeyword = Expect(SyntaxKind.InKeyword);
        var expression = ParseExpression();
        var body = ParseStatement();
        return new For(keyword, declaration, inKeyword, expression, body);
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