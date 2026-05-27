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
        if (Match(SyntaxKind.LetKeyword, SyntaxKind.MutKeyword))
            return ParseVariableDeclaration();

        var expression = ParseExpression();
        return new ExpressionStatement(expression);
    }

    private VariableDeclaration ParseVariableDeclaration()
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
    private Expression ParseExpression() => ParseAdditive();

    private Expression ParseAdditive()
    {
        var left = ParseMultiplicative();
        if (!Match(SyntaxKind.Plus, SyntaxKind.Minus))
            return left;

        var op = Last();
        var right = ParseMultiplicative();
        return new BinaryOperator(op, left, right);
    }
    
    private Expression ParseMultiplicative()
    {
        var left = ParseExponential();
        if (!Match(SyntaxKind.Star, SyntaxKind.Slash, SyntaxKind.Percent))
            return left;

        var op = Last();
        var right = ParseExponential();
        return new BinaryOperator(op, left, right);
    }
    
    private Expression ParseExponential()
    {
        var left = ParseUnary();
        if (!Match(SyntaxKind.Carat))
            return left;
        
        var op = Last();
        var right = ParseExponential();
        return new BinaryOperator(op, left, right);
    }

    private Expression ParseUnary()
    {
        if (!Match(SyntaxFacts.IsUnaryOperator))
            return ParsePrimary();

        var op = Last();
        var operand = ParseExpression();
        return new UnaryOperator(op, operand);
    }

    private Expression ParsePrimary()
    {
        if (Match(SyntaxKind.LParen))
        {
            var leftParen = Last();
            var expression = ParseExpression();
            var rightParen = Expect(SyntaxKind.RParen, token => $"Expected ')', got '{token.Text}'");
            return new Parenthesized(leftParen, rightParen, expression);
        }
        if (Match(SyntaxKind.Identifier))
        {
            var name = Last();
            return new Identifier(name);
        }
        if (Match(SyntaxKind.IntegerLiteral, SyntaxKind.FloatLiteral, SyntaxKind.StringLiteral, SyntaxKind.TrueLiteral, SyntaxKind.FalseLiteral, SyntaxKind.NoneLiteral))
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

    private bool Match(params SyntaxKind[] kinds) => Match(kinds.Contains);
    private bool Match(SyntaxKind kind) => Match(otherKind => otherKind == kind);
    private bool Match(Predicate<SyntaxKind> predicate)
    {
        if (IsEof())
            return false;
        
        var token = Current();
        var match = predicate(token.Kind);
        if (match)
            Advance();
        
        return match;
    }

    private Token Expect(SyntaxKind kind, string message) => Expect(kind, _ => message);
    private Token Expect(SyntaxKind kind, Func<Token, string>? message = null)
    {
        var token = Advance();
        if (token.Kind == kind)
            return token;

        Diagnostics.Error(token.Span, InternalCodes.UnexpectedToken, message != null ? message(token) : $"Unexpected token '{token.Text}'");
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