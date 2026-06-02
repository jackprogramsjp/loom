using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

        if (Match(out var typeKeyword, SyntaxKind.TypeKeyword))
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
        if (!Match(out var leftArrow, SyntaxKind.LArrow))
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
        if (Match(out var colon, SyntaxKind.Colon))
        {
            var type = ParseType();
            colonTypeClause = new ColonTypeClause(colon, type);
        }

        if (Match(out var equals, SyntaxKind.Equals))
        {
            var initializer = ParseExpression();
            equalsValueClause = new EqualsValueClause(equals, initializer);
        }

        return new VariableDeclaration(keyword, name, colonTypeClause, equalsValueClause);
    }

    private Expression ParseExpression() => ParseAssignment();
    private Expression ParseAssignment() => ParseBinaryRightAssociative<AssignmentOperator>(ParseNullCoalescing, ParseAssignment, SyntaxFacts.IsAssignmentOperator);
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

    private Expression ParseExponential() => ParseBinaryRightAssociative(ParseUnary, ParseExponential, SyntaxKind.Caret);

    private Expression ParseUnary() =>
        Match(out var op, SyntaxFacts.IsUnaryOperator)
            ? new UnaryOperator(op, ParseUnary())
            : ParsePrimary();

    private Expression ParsePrimary()
    {
        if (Match(out var leftParen, SyntaxKind.LParen))
        {
            var expression = ParseExpression();
            var rightParen = Expect(
                SyntaxKind.RParen,
                got => $"Expected ')' here to close '{leftParen.Text}' at character {leftParen.Span.Start.Character}, got {SafeTokenText(got)}."
            );

            return new Parenthesized(leftParen, rightParen, expression);
        }

        if (Match(out var name, SyntaxKind.Identifier))
            return new Identifier(name);

        if (Match(
                out var token,
                SyntaxKind.NumberLiteral,
                SyntaxKind.StringLiteral,
                SyntaxKind.TrueLiteral,
                SyntaxKind.FalseLiteral,
                SyntaxKind.NoneLiteral
            ))
        {
            if (token.Kind == SyntaxKind.NumberLiteral)
            {
                var floatingPoint = ParseNumberValue(token);
                var isInteger = Math.Abs(Math.Floor(floatingPoint) - floatingPoint) < 1e-7;
                return isInteger
                    ? new Literal(token, (long)floatingPoint)
                    : new Literal(token, floatingPoint);
            }

            object? value = token.Kind switch
            {
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

    private TypeExpression ParseUnionType() =>
        ParseChainedType(ParseIntersectionType, SyntaxKind.Pipe, (separators, types) => new UnionType(separators, types));

    private TypeExpression ParseIntersectionType() =>
        ParseChainedType(ParseOptionalType, SyntaxKind.Ampersand, (seps, types) => new IntersectionType(seps, types));

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
        if (Match(out var leftParen, SyntaxKind.LParen))
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

        if (!Match(out var leftArrow, SyntaxKind.LArrow))
            return new TypeName(name);

        var arguments = ParseDelimited(ParseType);
        var rightArrow = Expect(SyntaxKind.RArrow);
        var typeArguments = new TypeArguments(leftArrow, rightArrow, arguments);
        return new TypeName(name, typeArguments);
    }

    private TypeExpression ParseChainedType(Func<TypeExpression> parseInner,
        SyntaxKind separator,
        Func<List<Token>, List<TypeExpression>, TypeExpression> create)
    {
        var types = new List<TypeExpression> { parseInner() };
        var seps = new List<Token>();

        while (Match(separator))
        {
            seps.Add(Last());
            types.Add(parseInner());
        }

        return seps.Count > 0 ? create(seps, types) : types[0];
    }

    private Expression ParseBinaryRightAssociative(Func<Expression> parseLeft, Func<Expression> parseRight, params SyntaxKind[] kinds) =>
        ParseBinaryRightAssociative(parseLeft, parseRight, kinds.Contains);

    private Expression ParseBinaryRightAssociative(Func<Expression> parseLeft, Func<Expression> parseRight, Predicate<SyntaxKind> predicate) =>
        ParseBinaryRightAssociative<BinaryOperator>(parseLeft, parseRight, predicate);

    private Expression ParseBinaryRightAssociative<T>(Func<Expression> parseLeft, Func<Expression> parseRight, params SyntaxKind[] kinds)
        where T : BinaryOperator, new() =>
        ParseBinaryRightAssociative<T>(parseLeft, parseRight, kinds.Contains);

    private Expression ParseBinaryRightAssociative<T>(Func<Expression> parseLeft, Func<Expression> parseRight, Predicate<SyntaxKind> predicate)
        where T : BinaryOperator, new()
    {
        var left = parseLeft();
        while (Match(out var op, predicate))
        {
            var right = parseRight();
            var binary = new T { Operator = op, Left = left, Right = right };
            binary.Setup();
            left = binary;
        }

        return left;
    }

    private Expression ParseBinaryLeftAssociative(Func<Expression> parse, params SyntaxKind[] kinds) => ParseBinaryLeftAssociative(parse, kinds.Contains);

    private Expression ParseBinaryLeftAssociative(Func<Expression> parse, Predicate<SyntaxKind> predicate)
    {
        var left = parse();
        while (Match(out var op, predicate))
        {
            var right = parse();
            var binary = new BinaryOperator { Operator = op, Left = left, Right = right };
            binary.Setup();
            left = binary;
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

    private static double ParseNumberValue(Token token)
    {
        var text = token.Text.Replace("_", "");
        return text switch
        {
            _ when text.EndsWith("hz", StringComparison.OrdinalIgnoreCase) => 1 / double.Parse(text[..^2]),
            _ when text.EndsWith("ms", StringComparison.OrdinalIgnoreCase) => double.Parse(text[..^2]) / 1000,
            _ when text.EndsWith("s", StringComparison.OrdinalIgnoreCase) => double.Parse(text[..^1]),
            _ when text.EndsWith("m", StringComparison.OrdinalIgnoreCase) => 60 * double.Parse(text[..^1]),
            _ when text.EndsWith("h", StringComparison.OrdinalIgnoreCase) => 3600 * double.Parse(text[..^1]),
            _ when text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) => long.Parse(text[2..], NumberStyles.HexNumber),
            _ when text.StartsWith("0b", StringComparison.OrdinalIgnoreCase) => long.Parse(text[2..], NumberStyles.BinaryNumber),
            _ when text.StartsWith("0o", StringComparison.OrdinalIgnoreCase) => Convert.ToInt64(text[2..], 8),
            _ => double.Parse(text),
        };
    }

    private bool Match([MaybeNullWhen(false)] out Token token, params SyntaxKind[] kinds) => Match(out token, kinds.Contains);
    private bool Match(SyntaxKind kind) => Match(otherKind => otherKind == kind);
    private bool Match(Predicate<SyntaxKind> predicate) => Match(out var _, predicate);
    private bool Match([MaybeNullWhen(false)] out Token token, SyntaxKind kind) => Match(out token, otherKind => otherKind == kind);

    private bool Match([MaybeNullWhen(false)] out Token token, Predicate<SyntaxKind> predicate)
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