using Loom.Diagnostics;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Parsing;

public class Parser(SourceFile file, IEnumerable<Token> tokens)
{
    private readonly DiagnosticBag _diagnostics = new();
    private int _position;

    public ParserResult Parse()
    {
        var statements = new List<Node>();
        while (!IsEof())
            statements.Add(ParseStatement());

        var tree = new Tree(file, statements);
        return new ParserResult(tree, _diagnostics);
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
        var name = Expect(SyntaxKind.Identifier, token => $"Expected identifier, got {(token != null ? $"'{token.Text}'" : "EOF")}");
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
        if (!Match(SyntaxFacts.IsUnaryOperator))
            return ParsePrimary();

        var op = Last();
        return new UnaryOperator(op, ParseUnary());
    }

    private Expression ParsePrimary()
    {
        if (Match(SyntaxKind.LParen))
        {
            var leftParen = Last();
            var expression = ParseExpression();
            var rightParen = Expect(SyntaxKind.RParen, token => $"Expected ')' here to close '{leftParen.Text}' at character {leftParen.Span.Start.Character}, got {(token != null ? $"'{token.Text}'" : "EOF")}");
            return new Parenthesized(leftParen, rightParen, expression);
        }

        if (Match(SyntaxKind.Identifier))
        {
            var name = Last();
            return new Identifier(name);
        }

        if (Match(SyntaxKind.IntegerLiteral,
                  SyntaxKind.FloatLiteral,
                  SyntaxKind.StringLiteral,
                  SyntaxKind.TrueLiteral,
                  SyntaxKind.FalseLiteral,
                  SyntaxKind.NoneLiteral))
        {
            var token = Last();
            object? value = token.Kind switch
            {
                SyntaxKind.IntegerLiteral => int.Parse(token.Text),
                SyntaxKind.FloatLiteral => double.Parse(token.Text),
                SyntaxKind.StringLiteral => token.Text.Substring(1, token.Text.Length - 2),
                SyntaxKind.TrueLiteral => true,
                SyntaxKind.FalseLiteral => false,
                _ => null
            };
            
            return new Literal(token, value);
        }

        _diagnostics.Error(Current().Span, InternalCodes.UnexpectedToken, "Unexpected token.");
        return new NullExpression(Advance());
    }

    private TypeExpression ParseType() => ParseParenthesizable(ParseOptionalType);

    private T ParseParenthesizable<T>(Func<T> parseInner)
        where T : Node
    {
        var parens = 0;
        while (Match(SyntaxKind.LParen)) parens++;
        
        var start = _position;
        var node = parseInner();
        for (var i = 0; i < parens; i++)
        {
            var opening = tokens.ElementAt(start - i);
            Expect(SyntaxKind.RParen, token => $"Expected ')' here to close '{opening.Text}' at character {opening.Span.Start.Character}, got {(token != null ? $"'{token.Text}'" : "EOF")}");
        }

        return node;
    }

    private TypeExpression ParseOptionalType()
    {
        var inner = ParsePrimaryType();
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

        _diagnostics.Error(Current().Span, InternalCodes.ExpectedType, $"Expected type, got '{Current().Text}'");
        return new NullTypeExpression(Advance());
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

    private Token Expect(SyntaxKind kind, Func<Token?, string>? message = null)
    {
        if (IsEof())
        {
            _diagnostics.Error(Last().Span + 1, InternalCodes.UnexpectedEof, message != null ? message(null) : "Unexpected end of file.");
            return Last();
        }

        var token = Advance();
        if (token.Kind == kind)
            return token;

        _diagnostics.Error(token.Span, InternalCodes.UnexpectedToken, message != null ? message(token) : $"Unexpected token '{token.Text}'");
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