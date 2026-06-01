using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Loom.Diagnostics;
using Loom.Lexing;
using Loom.Parsing.AST;
using Loom.Syntax;

namespace Loom.Parsing;

public class Parser(LexerResult lexerResult)
{
    private readonly DiagnosticBag _diagnostics = new();
    private int _position;

    public ParserResult Parse()
    {
        var statements = new List<Statement>();
        while (!IsEof())
            statements.Add(ParseStatement());

        var tree = new Tree(lexerResult.File, statements);
        return new ParserResult(tree, _diagnostics);
    }

    private Statement ParseStatement()
    {
        if (Match(out var variableKeyword, SyntaxKind.LetKeyword, SyntaxKind.MutKeyword))
            return ParseVariableDeclaration(variableKeyword);

        if (Match(SyntaxKind.TypeKeyword, out var typeKeyword))
            return ParseTypeAlias(typeKeyword);

        var expression = ParseExpression();
        return new ExpressionStatement(expression);
    }

    private TypeAlias ParseTypeAlias(Token keyword)
    {
        var name = ExpectIdentifier();
        var typeParameters = ParseTypeParameters();
        var equalsTypeClause = ParseEqualsTypeClause(true)!;
        return new TypeAlias(keyword, name, typeParameters, equalsTypeClause);
    }

    private TypeParameters? ParseTypeParameters()
    {
        if (!Match(SyntaxKind.LArrow, out var leftArrow))
            return null;

        var parameters = ParseDelimited(ParseTypeParameter);
        var rightArrow = Expect(SyntaxKind.RArrow);
        return new TypeParameters(leftArrow, rightArrow, parameters);
    }

    private TypeParameter ParseTypeParameter()
    {
        var name = ExpectIdentifier("type parameter name");
        var equalsTypeClause = ParseEqualsTypeClause(false);
        return new TypeParameter(name, equalsTypeClause);
    }

    private EqualsTypeClause? ParseEqualsTypeClause(bool required)
    {
        if (required)
            Expect(SyntaxKind.Equals);
        else if (!Match(SyntaxKind.Equals))
            return null;

        var equals = Last();
        var type = ParseType();
        return new EqualsTypeClause(equals, type);
    }

    private VariableDeclaration ParseVariableDeclaration(Token keyword)
    {
        var name = ExpectIdentifier();
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

    private Expression ParseExpression() => ParseNullCoalescing();

    private Expression ParseNullCoalescing() => ParseBinaryRightAssociative(ParseLogicalOr, ParseNullCoalescing, SyntaxKind.QuestionQuestion);

    private Expression ParseLogicalOr() => ParseBinaryLeftAssociative(ParseLogicalAnd, SyntaxKind.PipePipe);

    private Expression ParseLogicalAnd() => ParseBinaryLeftAssociative(ParseBitwiseOr, SyntaxKind.AmpersandAmpersand);

    private Expression ParseBitwiseOr() => ParseBinaryLeftAssociative(ParseXor, SyntaxKind.Pipe);

    private Expression ParseXor() => ParseBinaryLeftAssociative(ParseBitwiseAnd, SyntaxKind.Tilde);

    private Expression ParseBitwiseAnd() => ParseBinaryLeftAssociative(ParseEquality, SyntaxKind.Ampersand);

    private Expression ParseEquality() => ParseBinaryLeftAssociative(ParseRelational, SyntaxKind.EqualsEquals, SyntaxKind.BangEquals);

    private Expression ParseRelational() =>
        ParseBinaryLeftAssociative(ParseShift, SyntaxKind.LArrow, SyntaxKind.LArrowEquals, SyntaxKind.RArrow, SyntaxKind.RArrowEquals);

    private Expression ParseShift() => ParseBinaryLeftAssociative(ParseAdditive, SyntaxKind.LArrowLArrow, SyntaxKind.RArrowRArrow, SyntaxKind.RArrowRArrowRArrow);

    private Expression ParseAdditive() => ParseBinaryLeftAssociative(ParseMultiplicative, SyntaxKind.Plus, SyntaxKind.Minus);

    private Expression ParseMultiplicative() =>
        ParseBinaryLeftAssociative(ParseExponential, SyntaxKind.Star, SyntaxKind.Slash, SyntaxKind.SlashSlash, SyntaxKind.Percent);

    private Expression ParseExponential() => ParseBinaryRightAssociative(ParseUnary, ParseExponential, SyntaxKind.Carat);

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
        _position++;
        return new NullExpression(last);
    }

    private TypeExpression ParseType() => ParseUnionType();

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
        if (Match(SyntaxKind.LParen, out var leftParen))
        {
            var type = ParseType();
            var rightParen = Expect(
                SyntaxKind.RParen,
                got => $"Expected ')' here to close '{leftParen.Text}' at character {leftParen.Span.Start.Character}, got {SafeTokenText(got)}."
            );

            return new ParenthesizedType(leftParen, rightParen, type);
        }

        var name = ExpectIdentifier("type");
        if (SyntaxFacts.IsPrimitiveType(name.Text))
            return new PrimitiveType(name);

        if (!Match(SyntaxKind.LArrow, out var leftArrow))
            return new TypeName(name);

        var arguments = ParseDelimited(ParseType);
        var rightArrow = Expect(SyntaxKind.RArrow);
        var typeArguments = new TypeArguments(leftArrow, rightArrow, arguments);
        return new TypeName(name, typeArguments);
    }

    private Expression ParseBinaryRightAssociative(Func<Expression> parseLeft, Func<Expression> parseRight, params SyntaxKind[] operators)
    {
        var left = parseLeft();
        while (Match(out var op, operators))
        {
            var right = parseRight();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }

    private Expression ParseBinaryLeftAssociative(Func<Expression> parse, params SyntaxKind[] operators)
    {
        var left = parse();
        while (Match(out var op, operators))
        {
            var right = parse();
            left = new BinaryOperator(op, left, right);
        }

        return left;
    }

    private List<T> ParseDelimited<T>(Func<T> parse, SyntaxKind delimiter = SyntaxKind.Comma)
        where T : Node
    {
        var nodes = new List<T>([parse()]);
        while (Match(delimiter))
            nodes.Add(parse());

        return nodes;
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

    private Token ExpectIdentifier(string expected = "identifier") => Expect(SyntaxKind.Identifier, token => $"Expected {expected}, got {SafeTokenText(token)}.");

    private Token Expect(SyntaxKind kind, string message) => Expect(kind, _ => message);

    private Token Expect(SyntaxKind kind, Func<Token?, string>? message = null)
    {
        if (IsEof())
        {
            var last = Last();
            var text = SyntaxFacts.GetText(kind) ?? kind.ToString();
            _diagnostics.Error(last.Span, InternalCodes.UnexpectedEof, message != null ? message(null) : $"Expected '{text}', got end of file.");
            return last;
        }

        var token = Advance();
        if (token.Kind == kind)
            return token;

        var expected = SyntaxFacts.GetText(kind) ?? kind.ToString();
        _diagnostics.Error(
            token.Span,
            InternalCodes.UnexpectedToken,
            message != null ? message(token) : $"Expected '{expected}', got '{token.Text}'."
        );
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
    private Token Peek(int offset) => lexerResult.Tokens.ElementAt(_position + offset);
    private bool IsEof() => _position >= lexerResult.Tokens.Count;
    private static string SafeTokenText(Token? token) => token != null ? $"'{token.Text}'" : "EOF";
}