using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Parsing;

public class Parser(IEnumerable<Token> tokens) : Diagnosable
{
    private int _position;

    public ParserResult Parse()
    {
        var statements = new List<ASTNode>();
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
        while (Match(SyntaxKind.Plus, SyntaxKind.Minus))
        {
            var op = Last();
            var right = ParseMultiplicative();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }
    
    private Expression ParseMultiplicative()
    {
        var left = ParseExponential();
        while (Match(SyntaxKind.Star, SyntaxKind.Slash, SyntaxKind.Percent))
        {
            var op = Last();
            var right = ParseExponential();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }
    
    private Expression ParseExponential()
    {
        var left = ParseUnary();
        while (Match(SyntaxKind.Carat))
        {
            var op = Last();
            var right = ParseExponential();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }

    private Expression ParseUnary()
    { 
        var operand = ParsePrimary();
        while (Match(SyntaxFacts.IsUnaryOperator))
        {
            var op = Last();
            operand = new UnaryOperator(op, operand);
        }

        return operand;
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

    private TypeExpression ParseType() => ParseOptionalType();

    private TypeExpression ParseOptionalType()
    {
        var inner = ParsePrimaryType();
        if (Current().Kind == SyntaxKind.QuestionQuestion)
        {
            Diagnostics.Error(inner.Span, InternalCodes.RedundantOptionalType, "Cannot make already optional type optional.");
            Advance();
        }

        if (!Match(SyntaxKind.Question))
            return inner;

        var question = Last();
        return new OptionalType(question, inner);
    }

    private TypeExpression ParsePrimaryType()
    {
        if (Match(SyntaxKind.Identifier))
        {
            var name = Last();
            return SyntaxFacts.IsPrimitiveType(name.Text) ? new PrimitiveType(name) : new TypeName(name);
        }

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