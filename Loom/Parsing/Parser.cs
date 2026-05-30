using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
        var statements = new List<Statement>();
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
        var name = Expect(SyntaxKind.Identifier, token => $"Expected identifier, got {SafeTokenText(token)}.");
        ColonTypeClause? colonTypeClause = null;
        EqualsValueClause? equalsValueClause = null;
        if (Match(SyntaxKind.Colon, out var colon))
        {
            var type = ParseType();
            colonTypeClause = new ColonTypeClause(colon, type);
        }

        if (Match(SyntaxKind.Equals, out var equals))
        {
            var initializer = ParseExpression();
            equalsValueClause = new EqualsValueClause(equals, initializer);
        }

        return new VariableDeclaration(keyword, name, colonTypeClause, equalsValueClause);
    }

    private Expression ParseExpression() => ParseAdditive();

    private Expression ParseAdditive()
    {
        var left = ParseMultiplicative();
        while (Match(out var op, SyntaxKind.Plus, SyntaxKind.Minus))
        {
            var right = ParseMultiplicative();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }

    private Expression ParseMultiplicative()
    {
        var left = ParseExponential();
        while (Match(out var op, SyntaxKind.Star, SyntaxKind.Slash, SyntaxKind.Percent))
        {
            var right = ParseExponential();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }

    private Expression ParseExponential()
    {
        var left = ParseUnary();
        while (Match(SyntaxKind.Carat, out var op))
        {
            var right = ParseExponential();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }

    private Expression ParseUnary() =>
        Match(SyntaxFacts.IsUnaryOperator, out var op)
            ? new UnaryOperator(op, ParseUnary())
            : ParsePrimary();

    private Expression ParsePrimary()
    {
        if (Match(SyntaxKind.LParen, out var leftParen))
        {
            var expression = ParseExpression();
            var rightParen = Expect(
                SyntaxKind.RParen,
                got => $"Expected ')' here to close '{leftParen.Text}' at character {leftParen.Span.Start.Character}, got {SafeTokenText(got)}."
            );

            return new Parenthesized(leftParen, rightParen, expression);
        }

        if (Match(SyntaxKind.Identifier, out var name))
            return new Identifier(name);

        if (Match(
                out var token,
                SyntaxKind.IntegerLiteral,
                SyntaxKind.FloatLiteral,
                SyntaxKind.StringLiteral,
                SyntaxKind.TrueLiteral,
                SyntaxKind.FalseLiteral,
                SyntaxKind.NoneLiteral
            ))
        {
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

        var last = Last();
        _diagnostics.Error(last.Span, InternalCodes.UnexpectedToken, "Unexpected token.");
        return new NullExpression(last);
    }

    private TypeExpression ParseType() => ParseParenthesizable(ParseUnionType);

    private TypeExpression ParseUnionType()
    {
        var types = new List<TypeExpression>();
        var pipes = new List<Token>();
        var left = ParseIntersectionType();
        types.Add(left);

        while (Match(SyntaxKind.Pipe))
        {
            pipes.Add(Last());
            types.Add(ParseIntersectionType());
        }

        return pipes.Count > 0 ? new UnionType(pipes, types) : left;
    }

    private TypeExpression ParseIntersectionType()
    {
        var types = new List<TypeExpression>();
        var ampersands = new List<Token>();
        var left = ParseOptionalType();
        types.Add(left);

        while (Match(SyntaxKind.Ampersand))
        {
            ampersands.Add(Last());
            types.Add(ParseOptionalType());
        }

        return ampersands.Count > 0 ? new IntersectionType(ampersands, types) : left;
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
        var name = Expect(SyntaxKind.Identifier, token => $"Expected type, got {SafeTokenText(token)}.");
        return SyntaxFacts.IsPrimitiveType(name.Text) ? new PrimitiveType(name) : new TypeName(name);
    }

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
            Expect(
                SyntaxKind.RParen,
                token => $"Expected ')' here to close '{opening.Text}' at character {opening.Span.Start.Character}, got {SafeTokenText(token)}."
            );
        }

        return node;
    }

    private bool Match(params SyntaxKind[] kinds) => Match(kinds.Contains);
    private bool Match([MaybeNullWhen(false)] out Token token, params SyntaxKind[] kinds) => Match(kinds.Contains, out token);
    private bool Match(SyntaxKind kind) => Match(otherKind => otherKind == kind, out var _);
    private bool Match(Predicate<SyntaxKind> predicate) => Match(predicate, out var _);
    private bool Match(SyntaxKind kind, [MaybeNullWhen(false)] out Token token) => Match(otherKind => otherKind == kind, out token);

    private bool Match(Predicate<SyntaxKind> predicate, [MaybeNullWhen(false)] out Token token)
    {
        if (IsEof())
        {
            token = null;
            return false;
        }

        token = Current();
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
            var last = Last();
            _diagnostics.Error(last.Span, InternalCodes.UnexpectedEof, message != null ? message(null) : "Unexpected end of file.");
            return last;
        }

        var token = Advance();
        if (token.Kind == kind)
            return token;

        _diagnostics.Error(token.Span, InternalCodes.UnexpectedToken, message != null ? message(token) : $"Unexpected token '{token.Text}'.");
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
    private static string SafeTokenText(Token? token) => token != null ? $"'{token.Text}'" : "EOF";
}