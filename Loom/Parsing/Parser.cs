using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Parsing;

public class Parser(IEnumerable<Token> tokens) : Diagnosable
{
    private int _position;

    public ParserResult Parse()
    {
        var statements = new List<Statement>();
        while (!IsEof())
            statements.Add(ParseStatement());
        
        var tree = new Tree(statements);
        return new ParserResult(tree, Diagnostics);
    }

    private Statement ParseStatement()
    {
        if (MatchAny(SyntaxKind.LetKeyword, SyntaxKind.MutKeyword))
            return ParseVariableDeclaration();

        var expression = ParseExpression();
        return new ExpressionStatement(expression);
    }

    private Statement ParseVariableDeclaration()
    {
        var keyword = Last();
        var name = Expect(SyntaxKind.Identifier);
        ColonTypeClause? colonTypeClause = null;
        EqualsValueClause? equalsValueClause = null;
        if (Match(SyntaxKind.Colon))
        {
            var colon = Last();
            var type = ParseType();
            colonTypeClause = new ColonTypeClause(colon, type);
        }
        if (Match(SyntaxKind.Equals))
        {
            var equals = Last();
            var initializer = ParseExpression();
            equalsValueClause = new EqualsValueClause(equals, initializer);
        }
        
        return new VariableDeclaration(keyword, name, colonTypeClause, equalsValueClause);
    }

    private Expression ParseExpression() => ParsePrimary();

    private Expression ParsePrimary()
    {
        if (Match(SyntaxKind.Identifier))
        {
            var name = Last();
            return new Identifier(name);
        }

        if (MatchAny(SyntaxKind.IntegerLiteral, SyntaxKind.FloatLiteral, SyntaxKind.StringLiteral, SyntaxKind.TrueLiteral, SyntaxKind.FalseLiteral, SyntaxKind.NoneLiteral))
        {
            var token = Last();
            return new Literal(token);
        }
        
        Diagnostics.Error(Current().Span, InternalCodes.UnexpectedToken, "Unexpected token.");
        return new NullExpression();   
    }

    private TypeExpression ParseType()
    {
        Diagnostics.Error(Current().Span, InternalCodes.UnexpectedType, "Unexpected type.");
        return new NullTypeExpression();
    }
    
    private bool MatchAny(params SyntaxKind[] kinds)
    {
        var token = Current();
        var match = kinds.Contains(token.Kind);
        if (match)
            Advance();
        
        return match;
    }

    private bool Match(SyntaxKind kind)
    {
        var token = Current();
        var match = token.Kind == kind;
        if (match)
            Advance();
        
        return match;
    }

    private Token Expect(SyntaxKind kind)
    {
        var token = Advance();
        if (token.Kind == kind)
            return token;

        Diagnostics.Error(token.Span, InternalCodes.UnexpectedToken, $"Unexpected token '{token.Text}'");
        return token;
    }

    private Token Advance()
    {
        var current = Current();
        _position++;
        return current;
    }
    
    private Token Current() => Peek(0);
    private Token Last() => Peek(-1);
    private Token Peek(int offset) => tokens.ElementAt(_position + offset);
    private bool IsEof() => _position >= tokens.Count();
}